using AutoMapper;
using BusinessLogic_Layer.Common;
using BusinessLogic_Layer.Entity;
using BusinessLogic_Layer.Enums;
using DataAccess_Layer.Helpers;
using DataAccess_Layer.Interfaces;
using DataAccess_Layer.Models;
using DataAccess_Layer.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using System.IdentityModel.Tokens.Jwt;

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
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(IUnitOfWork unitOfWork, IMapper mapper, MailService mailService,
            Generate generate, IStringLocalizer<SharedResource> localizer, ElasticsearchService elasticsearchService,
            IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _mailService = mailService;
            _generate = generate;
            _localizer = localizer;
            _elasticsearchService = elasticsearchService;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<ResultObject> Login(LoginRequest loginRequest)
        {
            try
            {
                var user = _unitOfWork.UserRepository.FirstOrDefault(u => u.Email == loginRequest.Username);
                if (user == null)
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.Account]} {_localizer[SharedResourceKeys.NotFound]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.NotFound,
                    };

                // Check verify email
                if (!user.VerifyEmail)
                    return new ResultObject
                    {
                        Message = _localizer[SharedResourceKeys.EmailVerify],
                        Success = true,
                        StatusCode = EnumStatusCodesResult.Success,
                    };

                // Check password
                var passwordHasher = new PasswordHasher<User>();
                var result = passwordHasher.VerifyHashedPassword(user, user.Password, loginRequest.Password);

                if (result == PasswordVerificationResult.Success)
                {
                    var token = await _generate.GenerateJwtToken(user);
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
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    #region Check email exists
                    var findUserExists = _unitOfWork.UserRepository.FirstOrDefault(u => u.Email == apiUser.Email);
                    if (findUserExists != null)
                        return new ResultObject
                        {
                            Message = _localizer[SharedResourceKeys.EmailAlready],
                            Success = true,
                            StatusCode = EnumStatusCodesResult.Unauthorized
                        };
                    #endregion

                    var user = _mapper.Map<ApiUser, User>(apiUser);
                    user.Id = Guid.NewGuid();
                    user.VerifyEmail = false;

                    // Create Email Verification Token
                    var newEmailVerify = new EmailVerificationToken()
                    {
                        UserId = user.Id,
                        TokenEmail = _generate.CreateRandomToken(),
                    };

                    #region Add data to Database
                    _unitOfWork.UserRepository.Add(user);
                    _unitOfWork.EmailVerificationTokenRepository.Add(newEmailVerify);
                    var result = _unitOfWork.SaveChangesBool();
                    if (!result)
                    {
                        transaction.Rollback(); // Rollback if saving data fails
                        return new ResultObject
                        {
                            Message = $"{_localizer[SharedResourceKeys.SaveChanges]} {_localizer[SharedResourceKeys.Failde]}",
                            Success = true,
                            StatusCode = EnumStatusCodesResult.InternalServerError
                        };
                    }
                    #endregion

                    // Send email verify to User
                    var emailSent = await _mailService.SendEmailVerify(user, newEmailVerify);
                    if (emailSent != true)
                    {
                        transaction.Rollback(); // Rollback if sending email fails
                        return new ResultObject
                        {
                            Message = _localizer[SharedResourceKeys.EmailSendFailde],
                            Success = true,
                            StatusCode = EnumStatusCodesResult.InternalServerError
                        };
                    }

                    transaction.Commit(); // Commit if both operations succeed
                    return new ResultObject
                    {
                        Data = emailSent,
                        Success = true,
                        StatusCode = EnumStatusCodesResult.Create
                    };
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
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
        public async Task<ResultObject> VerifyEmail(string tokenEncode)
        {
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
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
                    var user = _unitOfWork.UserRepository.GetById(userGuid);
                    if (user == null)
                        return new ResultObject
                        {
                            Message = $"{_localizer[SharedResourceKeys.User]} {_localizer[SharedResourceKeys.NotFound]}",
                            Success = true,
                            StatusCode = EnumStatusCodesResult.NotFound
                        };

                    var token = _unitOfWork.EmailVerificationTokenRepository.FirstOrDefaultIncludeDelete(n => n.UserId == user.Id);
                    if (token == null || token.TokenEmail != checkTokenEmail.Token)
                        return new ResultObject
                        {
                            Message = $"Token {_localizer[SharedResourceKeys.Incorrect]}",
                            Success = true,
                            StatusCode = EnumStatusCodesResult.Unauthorized
                        };
                    #endregion

                    #region Create password and save data to Database
                    user.Password = _generate.CreateRandomPassword();

                    var statusMail = await _mailService.SendEmailPassword(user);
                    if (!statusMail)
                    {
                        transaction.Rollback();
                        return new ResultObject
                        {
                            Message = $"{_localizer[SharedResourceKeys.EmailSendFailde]}",
                            Success = true,
                            StatusCode = EnumStatusCodesResult.InternalServerError
                        };
                    }

                    var passwordHasher = new PasswordHasher<User>();
                    user.Password = passwordHasher.HashPassword(user, user.Password);
                    user.VerifyEmail = true;
                    _unitOfWork.UserRepository.Update(user);
                    _unitOfWork.EmailVerificationTokenRepository.Remove(token);

                    var result = _unitOfWork.SaveChangesBool();
                    if (!result)
                    {
                        transaction.Rollback(); 
                        return new ResultObject
                        {
                            Message = $"{_localizer[SharedResourceKeys.SaveChanges]} {_localizer[SharedResourceKeys.Failde]}",
                            Success = true,
                            StatusCode = EnumStatusCodesResult.InternalServerError
                        };
                    }
                    #endregion

                    #region Add Document Elasticsearch
                    var response = await _elasticsearchService.CreateDocumentAsync(new
                    {
                        Id = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber
                    }, "users");
                    if (!response)
                    {
                        transaction.Rollback(); 
                        return new ResultObject
                        {
                            Message = $"{_localizer[SharedResourceKeys.CreateFailde]} {_localizer[SharedResourceKeys.Document]} Elasticsearch ",
                            Success = true,
                            StatusCode = EnumStatusCodesResult.InternalServerError
                        };
                    }
                    #endregion

                    transaction.Commit();
                    return new ResultObject
                    {
                        Data = $"{_localizer[SharedResourceKeys.ConfirmEmailSuccessful]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.Success
                    };
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
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
        public async Task<ResultObject> ForgotPassword(string email)
        {
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    #region Check email and otp
                    var user = _unitOfWork.UserRepository.FirstOrDefault(u => u.Email == email);
                    if (user == null)
                        return new ResultObject
                        {
                            Message = $"Email {_localizer[SharedResourceKeys.NotFound]}",
                            Success = true,
                            StatusCode = EnumStatusCodesResult.NotFound
                        };

                    var otp = _unitOfWork.OTPResetPasswordRepository.FirstOrDefaultIncludeDelete(u => u.Email == email);
                    if (otp != null)
                    {
                        // if exist => update
                        otp.OTP = _generate.GenerateOTP();
                        otp.ExpiredTime = DateTime.Now.AddMinutes(5);
                        _unitOfWork.OTPResetPasswordRepository.Update(otp);
                        var resultUpdate = _unitOfWork.SaveChangesBool();
                        if (!resultUpdate)
                        {
                            transaction.Rollback();
                            return new ResultObject
                            {
                                Message = $"{_localizer[SharedResourceKeys.SaveChanges]} {_localizer[SharedResourceKeys.Failde]}",
                                Success = true,
                                StatusCode = EnumStatusCodesResult.InternalServerError
                            };
                        }

                        var statusSendMail = await _mailService.SendPasswordResetEmail(otp);
                        if (!statusSendMail)
                        {
                            transaction.Rollback();
                            return new ResultObject
                            {
                                Message = _localizer[SharedResourceKeys.EmailSendFailde],
                                Success = true,
                                StatusCode = EnumStatusCodesResult.InternalServerError
                            };
                        }

                        transaction.Commit();
                        return new ResultObject
                        {
                            Data = statusSendMail,
                            Success = true,
                            StatusCode = EnumStatusCodesResult.Success
                        };
                    }
                    #endregion

                    var newOtp = new OTPResetPassword()
                    {
                        Email = user.Email,
                        OTP = _generate.GenerateOTP(),
                        ExpiredTime = DateTime.Now.AddMinutes(5),
                    };
                    _unitOfWork.OTPResetPasswordRepository.Add(newOtp);
                    var resultAdd = _unitOfWork.SaveChangesBool();
                    if (!resultAdd)
                    {
                        transaction.Rollback();
                        return new ResultObject
                        {
                            Message = $"{_localizer[SharedResourceKeys.SaveChanges]} {_localizer[SharedResourceKeys.Failde]}",
                            Success = true,
                            StatusCode = EnumStatusCodesResult.InternalServerError
                        };
                    }

                    var statusMail = await _mailService.SendPasswordResetEmail(newOtp);
                    if (!statusMail)
                    {
                        transaction.Rollback();
                        return new ResultObject
                        {
                            Message = _localizer[SharedResourceKeys.EmailSendFailde],
                            Success = true,
                            StatusCode = EnumStatusCodesResult.InternalServerError
                        };
                    }

                    transaction.Commit();
                    return new ResultObject
                    {
                        Data = statusMail,
                        Success = true,
                        StatusCode = EnumStatusCodesResult.Success
                    };
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
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
        public async Task<ResultObject> CornfirmOTPChangePassword(PasswordConfirm passwordConfirm)
        {
            try
            {
                #region Check email and otp
                var user = _unitOfWork.UserRepository.FirstOrDefault(u => u.Email == passwordConfirm.Email);
                var otp = _unitOfWork.OTPResetPasswordRepository.FirstOrDefaultIncludeDelete(u => u.Email == passwordConfirm.Email);
                if (user == null || otp == null)
                    return new ResultObject
                    {
                        Message = $"Email {_localizer[SharedResourceKeys.Incorrect]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.Unauthorized
                    };

                if (passwordConfirm.OTP != otp.OTP)
                    return new ResultObject
                    {
                        Message = $"OTP {_localizer[SharedResourceKeys.Incorrect]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.Unauthorized
                    };

                if (DateTime.Now > otp.ExpiredTime)
                    return new ResultObject
                    {
                        Message = $"OTP {_localizer[SharedResourceKeys.ExpiredTime]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.Unauthorized
                    };
                #endregion

                var passwordHasher = new PasswordHasher<User>();
                user.Password = passwordHasher.HashPassword(user, passwordConfirm.NewPassword);
                _unitOfWork.UserRepository.Update(user);
                _unitOfWork.OTPResetPasswordRepository.Remove(otp);
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
        public async Task<ResultObject> GetCurrentUser()
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext?.GetCurrentUserId();
                return new ResultObject
                {
                    Data = userId,
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
