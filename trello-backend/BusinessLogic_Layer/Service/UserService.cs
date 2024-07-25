using AutoMapper;
using BusinessLogic_Layer.Entity;
using BusinessLogic_Layer.Enums;
using BusinessLogic_Layer.Model;
using DataAccess_Layer.Interfaces;
using DataAccess_Layer.Models;
using System.Linq.Expressions;
using Microsoft.Extensions.Localization;

namespace BusinessLogic_Layer.Service
{
    public class UserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly ElasticsearchService _elasticsearchService;
        public UserService(IUnitOfWork unitOfWork, IMapper mapper, IStringLocalizer<SharedResource> localizer,
            ElasticsearchService elasticsearchService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _localizer = localizer;
            _elasticsearchService = elasticsearchService;
        }
        public async Task<ResultObject> GetAll()
        {
            try
            {
                var data = _unitOfWork.UserRepository.GetAll().Select(s => new
                {
                    s.Id,
                    s.FirstName,
                    s.LastName,
                    s.Email,
                    s.PhoneNumber,
                });
                return new ResultObject
                {
                    Data = data,
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
        public async Task<ApiPageList<ApiUser>> GetPage(ApiPageModel apiPageModel)
        {
            try
            {
                Expression<Func<User, bool>> fillter = c => true;

                if (!string.IsNullOrEmpty(apiPageModel.Fillter))
                {
                    Expression<Func<User, bool>> lambdaKeyword = c => c.VerifyEmail == true;
                    var combined = Expression.AndAlso(fillter.Body, Expression.Invoke(lambdaKeyword, fillter.Parameters));
                    fillter = Expression.Lambda<Func<User, bool>>(combined, fillter.Parameters);
                }
                Expression<Func<User, string>> orderBy = c => "";
                if (apiPageModel.OrderEmail == true)
                {
                    orderBy = c => c.Email;
                }
                var data = _unitOfWork.UserRepository.GetPage(apiPageModel.Skip, apiPageModel.Take, fillter, orderBy);
                return new ApiPageList<ApiUser>()
                {
                    Items = _mapper.Map<List<User>, List<ApiUser>>(data.Items),
                    TotalCount = data.TotalCount,
                    CurrentPage = apiPageModel.CurrentPage,
                    PageSize = apiPageModel.PageSize,
                    StatusCode = EnumStatusCodesResult.Success,
                    Success = true
                };
            }
            catch (Exception ex)
            {
                return new ApiPageList<ApiUser>()
                {
                    CurrentPage = apiPageModel.CurrentPage,
                    PageSize = apiPageModel.PageSize,
                    StatusCode = EnumStatusCodesResult.InternalServerError,
                    Success = false
                };
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }
        public async Task<ResultObject> Update(ApiUser apiUser)
        {
            try
            {
                #region Check User exists
                var checkUser = _unitOfWork.UserRepository.FirstOrDefault(n => n.Id == apiUser.Id);
                if (checkUser == null)
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.User]} {_localizer[SharedResourceKeys.NotFound]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.Success
                    };
                #endregion

                apiUser.Email = checkUser.Email;
                apiUser.VerifyEmail = checkUser.VerifyEmail;
                _mapper.Map(apiUser, checkUser);
                _unitOfWork.UserRepository.Update(checkUser);
                var result = _unitOfWork.SaveChangesBool();
                if (!result)
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.SaveChanges]} {_localizer[SharedResourceKeys.Failde]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.Success
                    };

                #region Update Document Elasticsearch
                var response = await _elasticsearchService.UpdateDocumentAsync(new
                {
                    Id = checkUser.Id,
                    FirstName = checkUser.FirstName,
                    LastName = checkUser.LastName,
                    Email = checkUser.Email,
                    PhoneNumber = checkUser.PhoneNumber
                }, ElasticsearchKeys.UserIndex, checkUser.Id);
                if (!response)
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.CreateFailde]} {_localizer[SharedResourceKeys.Document]} Elasticsearch ",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.Success
                    };
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
        public async Task<ResultObject> Delete(Guid idUser)
        {
            try
            {
                #region Check User exists
                var checkUser = _unitOfWork.UserRepository.FirstOrDefault(n => n.Id == idUser);
                if (checkUser == null)
                    return new ResultObject
                    {
                        Message = $"User {_localizer[SharedResourceKeys.NotFound]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.Success
                    };
                #endregion

                _unitOfWork.UserRepository.Remove(checkUser);
                var result = _unitOfWork.SaveChangesBool();
                if (!result)
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.SaveChanges]} {_localizer[SharedResourceKeys.Failde]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.Success
                    };

                #region Delete Document Elasticsearch
                var response = await _elasticsearchService.DeleteDocumentAsync(idUser, ElasticsearchKeys.UserIndex);
                if (!response)
                {
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.DeleteFailde]} {_localizer[SharedResourceKeys.Document]} Elasticsearch ",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.Success
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
        public async Task<ResultObject> GetUsersByIds(List<string> userIds)
        {
            try
            {
                var users = _unitOfWork.UserRepository.GetAll()
                    .Where(u => userIds.Contains(u.Id.ToString()))
                    .Select(u => new
                    {
                        u.Id,
                        u.FirstName,
                        u.LastName,
                        u.Email,
                        u.PhoneNumber
                    }).ToList();

                return new ResultObject
                {
                    Data = users,
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
        }

    }
}
