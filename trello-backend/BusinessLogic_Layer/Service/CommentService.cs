using AutoMapper;
using BusinessLogic_Layer.Common;
using BusinessLogic_Layer.Entity;
using BusinessLogic_Layer.Enums;
using DataAccess_Layer.Interfaces;
using DataAccess_Layer.Models;
using Microsoft.Extensions.Localization;

namespace BusinessLogic_Layer.Service
{
    public class CommentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly DeleteChild _deleteChild;
        public CommentService(IUnitOfWork unitOfWork, IMapper mapper, IStringLocalizer<SharedResource> localizer, DeleteChild deleteChild)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _localizer = localizer;
            _deleteChild = deleteChild;
        }
        public async Task<ResultObject> CreateComment(ApiComment apiComment)
        {
            try
            {
                #region Check task card and user exists
                var checkTaskCard = _unitOfWork.TaskCardRepository.FirstOrDefault(n => n.Id == apiComment.TaskId);
                if (checkTaskCard == null)
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.TaskCard]} {_localizer[SharedResourceKeys.NotFound]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.NotFound,
                    };

                var checkUser = _unitOfWork.UserRepository.FirstOrDefault(n => n.Id == apiComment.UserId);
                if (checkUser == null)
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.User]} {_localizer[SharedResourceKeys.NotFound]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.NotFound,
                    };
                #endregion

                #region Add to Databasee
                var mapApiComment = _mapper.Map<ApiComment, Comment>(apiComment);
                mapApiComment.TaskCards = checkTaskCard;
                _unitOfWork.CommentRepository.Add(mapApiComment);
                _unitOfWork.SaveChanges();
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
        public async Task<ResultObject> CreateCommentReply(ApiComment apiComment)
        {
            try
            {
                #region Check Parent Comment and user exists
                var checkUser = _unitOfWork.UserRepository.FirstOrDefault(n => n.Id == apiComment.UserId);
                if (checkUser == null)
                {
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.User]} {_localizer[SharedResourceKeys.NotFound]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.NotFound
                    };
                }

                var checkParentComment = _unitOfWork.CommentRepository.FirstOrDefault(n => n.Id == apiComment.ParentCommentId);
                if (checkParentComment == null)
                {
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.TaskCard]} {_localizer[SharedResourceKeys.NotFound]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.NotFound
                    };
                }
                var checkTaskCard = _unitOfWork.TaskCardRepository.FirstOrDefault(t => t.Id == checkParentComment.TaskId);
                if (checkTaskCard == null)
                {
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.TaskCard]} {_localizer[SharedResourceKeys.NotFound]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.NotFound
                    };
                }
                #endregion

                #region Add Comment to Database
                var mapApiComment = _mapper.Map<ApiComment, Comment>(apiComment);
                mapApiComment.TaskCards = checkTaskCard;
                mapApiComment.Users = checkUser;

                _unitOfWork.CommentRepository.Add(mapApiComment);
                _unitOfWork.SaveChanges();
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
        public async Task<ResultObject> GetAllCommentFromTask(Guid TaskId)
        {
            try
            {
                #region Check task card exists
                var checkTaskCard = _unitOfWork.TaskCardRepository.FirstOrDefault(t => t.Id == TaskId);
                if (checkTaskCard == null)
                {
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.TaskCard]} {_localizer[SharedResourceKeys.NotFound]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.NotFound
                    };
                }
                #endregion

                var allComments = (from c in _unitOfWork.CommentRepository.Context()
                                   where c.TaskId == TaskId && !c.IsDeleted
                                   select new ApiComment
                                   {
                                       Id = c.Id,
                                       Description = c.Description,
                                       TaskId = c.TaskId,
                                       UserId = c.UserId,
                                       ParentCommentId = c.ParentCommentId,
                                       Replies = new List<ApiComment>()
                                   }).ToList();

                var commentDict = allComments.ToDictionary(c => c.Id);
                var rootComments = new List<ApiComment>();

                foreach (var comment in allComments)
                {
                    if (comment.ParentCommentId == null)
                    {
                        rootComments.Add(comment);
                    }
                    else if (commentDict.TryGetValue(comment.ParentCommentId.Value, out var parentComment))
                    {
                        parentComment.Replies.Add(comment);
                    }
                }

                return new ResultObject
                {
                    Data = rootComments,
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
        public async Task<ResultObject> Delete(Guid idComment)
        {
            try
            {
                #region Check comment exists
                var checkCommentDelete = _unitOfWork.CommentRepository.FirstOrDefault(t => t.Id == idComment);
                if (checkCommentDelete == null)
                {
                    return new ResultObject
                    {
                        Data = false,
                        Success = true,
                        StatusCode = EnumStatusCodesResult.Success
                    };
                }
                #endregion

                _unitOfWork.CommentRepository.Remove(checkCommentDelete);
                var response = _unitOfWork.SaveChangesBool();
                if (!response)
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.SaveChanges]} {_localizer[SharedResourceKeys.Error]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.InternalServerError
                    };
                var deleteChildTask = _deleteChild.DeleteChildComment(idComment);
                await deleteChildTask;
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
