using AutoMapper;
using BusinessLogic_Layer.Common;
using BusinessLogic_Layer.Entity;
using BusinessLogic_Layer.Enums;
using DataAccess_Layer.Interfaces;
using DataAccess_Layer.Models;
using DataAccess_Layer.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace BusinessLogic_Layer.Service
{
    public class AuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly MailService _mailService;
        private readonly Generate _generate;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly ElasticsearchService _elasticsearchService;

        public AuthService(IUnitOfWork unitOfWork, IMapper mapper, MailService mailService,
            Generate generate, IStringLocalizer<SharedResource> localizer, ElasticsearchService elasticsearchService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _mailService = mailService;
            _generate = generate;
            _localizer = localizer;
            _elasticsearchService = elasticsearchService;
        }
        public async Task<ResultObject> Login(LoginRequest loginRequest)
        {
            try
            {
                var checkUser = _unitOfWork.UserRepository.FirstOrDefault(u => u.Email == loginRequest.Username);
                if (checkUser == null)
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.Account]} {_localizer[SharedResourceKeys.NotFound]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.NotFound,
                    };

                // Check verify email
                if (!checkUser.VerifyEmail)
                    return new ResultObject
                    {
                        Message = _localizer[SharedResourceKeys.EmailVerify],
                        Success = true,
                        StatusCode = EnumStatusCodesResult.Success,
                    };

                // Check password
                var passwordHasher = new PasswordHasher<User>();
                var result = passwordHasher.VerifyHashedPassword(checkUser, checkUser.Password, loginRequest.Password);

                if (result == PasswordVerificationResult.Success)
                {
                    var token = await _generate.GenerateJwtToken(checkUser);
                    return new ResultObject
                    {
                        Data = token,
                        Success = true,
                        StatusCode = EnumStatusCodesResult.Success
                    };
                }
                else
                {
                    return new ResultObject
                    {
                        Message = _localizer[SharedResourceKeys.PasswordIncorrect],
                        Success = true,
                        StatusCode = EnumStatusCodesResult.Unauthorized,
                    };
                }
            }
            catch (Exception ex)
            {
                return new ResultObject
                {
                    Message = ex.Message,
                    Success = false,
                    StatusCode = EnumStatusCodesResult.InternalServerError
                };
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }
        public async Task<ResultObject> Register(ApiUser apiUser)
        {
            try
            {
                #region Check email exists
                var checkEmail = _unitOfWork.UserRepository.FirstOrDefault(u => u.Email == apiUser.Email);
                if (checkEmail != null)
                    return new ResultObject
                    {
                        Message = _localizer[SharedResourceKeys.EmailAlready],
                        Success = true,
                        StatusCode = EnumStatusCodesResult.Unauthorized
                    };
                #endregion

                var mapApiUser = _mapper.Map<ApiUser, User>(apiUser);
                mapApiUser.Id = Guid.NewGuid();
                mapApiUser.VerifyEmail = false;

                // Create Email Verification Token
                var newEmailVerify = new EmailVerificationToken()
                {
                    UserId = mapApiUser.Id,
                    TokenEmail = _generate.CreateRandomToken(),
                };
                #region Add data to Database
                _unitOfWork.UserRepository.Add(mapApiUser);
                _unitOfWork.EmailVerificationTokenRepository.Add(newEmailVerify);
                var result = _unitOfWork.SaveChangesBool();
                if (!result)
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.SaveChanges]} {_localizer[SharedResourceKeys.Failde]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.InternalServerError
                    };
                #endregion

                // Send email verify to User
                var emailSent = await _mailService.SendEmailVerify(mapApiUser, newEmailVerify);
                if (emailSent != true)
                    return new ResultObject
                    {
                        Message = _localizer[SharedResourceKeys.EmailSendFailde],
                        Success = true,
                        StatusCode = EnumStatusCodesResult.InternalServerError
                    };

                return new ResultObject
                {
                    Data = emailSent,
                    Success = true,
                    StatusCode = EnumStatusCodesResult.Create
                };
            }
            catch (Exception ex)
            {
                return new ResultObject
                {
                    Message = ex.Message,
                    Success = false,
                    StatusCode = EnumStatusCodesResult.InternalServerError
                };
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }
        public async Task<ResultObject> VerifyEmail(string tokenEncode)
        {
            try
            {
                // Decode token and check token
                var checkTokenEmail = _generate.DecodeFromBase64(tokenEncode);
                if (checkTokenEmail == null || checkTokenEmail.UserId == null || checkTokenEmail.Token == null)
                    return new ResultObject
                    {
                        Message = $"Token {_localizer[SharedResourceKeys.Error]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.Unauthorized
                    };

                #region Check user, token exists and check token
                Guid.TryParse(checkTokenEmail.UserId, out Guid userGuid);
                var checkUser = _unitOfWork.UserRepository.GetById(userGuid);
                if (checkUser == null)
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.User]} {_localizer[SharedResourceKeys.NotFound]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.NotFound
                    };

                var checkToken = _unitOfWork.EmailVerificationTokenRepository.FirstOrDefaultIncludeDelete(n => n.UserId == checkUser.Id);
                if (checkToken == null || checkToken.TokenEmail != checkTokenEmail.Token)
                    return new ResultObject
                    {
                        Message = $"Token {_localizer[SharedResourceKeys.Incorrect]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.Unauthorized
                    };
                #endregion

                #region Create password and save data to Database
                checkUser.Password = _generate.CreateRandomPassword();
                var statusMail = await _mailService.SendEmailPassword(checkUser);
                var passwordHasher = new PasswordHasher<User>();
                checkUser.Password = passwordHasher.HashPassword(checkUser, checkUser.Password);
                checkUser.VerifyEmail = true;
                _unitOfWork.UserRepository.Update(checkUser);
                _unitOfWork.EmailVerificationTokenRepository.Remove(checkToken);

                var result = _unitOfWork.SaveChangesBool();
                if (!result)
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.SaveChanges]} {_localizer[SharedResourceKeys.Failde]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.InternalServerError
                    };
                #endregion

                #region Add Document Elasticsearch
                var response = await _elasticsearchService.CreateDocumentAsync(new
                {
                    Id = checkUser.Id,
                    FirstName = checkUser.FirstName,
                    LastName = checkUser.LastName,
                    Email = checkUser.Email,
                    PhoneNumber = checkUser.PhoneNumber
                }, "users");
                if (!response)
                {
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.CreateFailde]} {_localizer[SharedResourceKeys.Document]} Elasticsearch ",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.InternalServerError
                    };
                }
                #endregion

                return new ResultObject
                {
                    Data = true,
                    Success = true,
                    StatusCode = EnumStatusCodesResult.Success
                };
            }
            catch (Exception ex)
            {
                return new ResultObject
                {
                    Message = ex.Message,
                    Success = false,
                    StatusCode = EnumStatusCodesResult.InternalServerError
                };
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }
        public async Task<ResultObject> ForgotPassword(string email)
        {
            try
            {
                #region Check email and otp
                var checkUser = _unitOfWork.UserRepository.FirstOrDefault(u => u.Email == email);
                if (checkUser == null)
                    return new ResultObject
                    {
                        Message = $"Email {_localizer[SharedResourceKeys.NotFound]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.NotFound
                    };
                var checkOTP = _unitOfWork.OTPResetPasswordRepository.FirstOrDefaultIncludeDelete(u => u.Email == email);
                if (checkOTP != null)
                {
                    // if exist => update
                    checkOTP.OTP = _generate.GenerateOTP();
                    checkOTP.ExpiredTime = DateTime.Now.AddMinutes(5);
                    _unitOfWork.OTPResetPasswordRepository.Update(checkOTP);
                    _unitOfWork.SaveChanges();

                    var statusSendMail = await _mailService.SendPasswordResetEmail(checkOTP);
                    return new ResultObject
                    {
                        Data = statusSendMail,
                        Success = true,
                        StatusCode = EnumStatusCodesResult.Success
                    };
                }
                #endregion

                var otp = new OTPResetPassword()
                {
                    Email = checkUser.Email,
                    OTP = _generate.GenerateOTP(),
                    ExpiredTime = DateTime.Now.AddMinutes(5),
                };
                _unitOfWork.OTPResetPasswordRepository.Add(otp);
                var result = _unitOfWork.SaveChangesBool();
                if (!result)
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.SaveChanges]} {_localizer[SharedResourceKeys.Failde]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.InternalServerError
                    };

                // Send email password to user
                var statusMail = await _mailService.SendPasswordResetEmail(otp);
                if (!statusMail)
                    return new ResultObject
                    {
                        Message = _localizer[SharedResourceKeys.EmailSendFailde],
                        Success = true,
                        StatusCode = EnumStatusCodesResult.InternalServerError
                    };

                return new ResultObject
                {
                    Data = statusMail,
                    Success = true,
                    StatusCode = EnumStatusCodesResult.Success
                };
            }
            catch (Exception ex)
            {
                return new ResultObject
                {
                    Message = ex.Message,
                    Success = false,
                    StatusCode = EnumStatusCodesResult.InternalServerError
                };
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }
        public async Task<ResultObject> CornfirmOTPChangePassword(PasswordConfirm passwordConfirm)
        {
            try
            {
                #region Check email and otp
                var checkUser = _unitOfWork.UserRepository.FirstOrDefault(u => u.Email == passwordConfirm.Email);
                var checkOtp = _unitOfWork.OTPResetPasswordRepository.FirstOrDefaultIncludeDelete(u => u.Email == passwordConfirm.Email);
                if (checkUser == null || checkOtp == null)
                    return new ResultObject
                    {
                        Message = $"Email {_localizer[SharedResourceKeys.Incorrect]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.Unauthorized
                    };

                if (passwordConfirm.OTP != checkOtp.OTP)
                    return new ResultObject
                    {
                        Message = $"OTP {_localizer[SharedResourceKeys.Incorrect]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.Unauthorized
                    };

                if (DateTime.Now > checkOtp.ExpiredTime)
                    return new ResultObject
                    {
                        Message = $"OTP {_localizer[SharedResourceKeys.ExpiredTime]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.Unauthorized
                    };
                #endregion

                var passwordHasher = new PasswordHasher<User>();
                checkUser.Password = passwordHasher.HashPassword(checkUser, passwordConfirm.NewPassword);
                _unitOfWork.UserRepository.Update(checkUser);
                _unitOfWork.OTPResetPasswordRepository.Remove(checkOtp);
                _unitOfWork.SaveChanges();

                return new ResultObject
                {
                    Data = true,
                    Success = true,
                    StatusCode = EnumStatusCodesResult.Success
                };
            }
            catch (Exception ex)
            {
                return new ResultObject
                {
                    Message = ex.Message,
                    Success = false,
                    StatusCode = EnumStatusCodesResult.InternalServerError
                };
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }
    }
}
