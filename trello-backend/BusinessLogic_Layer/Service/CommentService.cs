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
                var taskCard = _unitOfWork.TaskCardRepository.FirstOrDefault(n => n.Id == apiComment.TaskId);
                if (taskCard == null)
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.TaskCard]} {_localizer[SharedResourceKeys.NotFound]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.NotFound,
                    };

                var user = _unitOfWork.UserRepository.FirstOrDefault(n => n.Id == apiComment.UserId);
                if (user == null)
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.User]} {_localizer[SharedResourceKeys.NotFound]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.NotFound,
                    };
                #endregion

                #region Add to Databasee
                var comment = _mapper.Map<ApiComment, Comment>(apiComment);
                comment.TaskCards = taskCard;
                _unitOfWork.CommentRepository.Add(comment);
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
                var user = _unitOfWork.UserRepository.FirstOrDefault(n => n.Id == apiComment.UserId);
                if (user == null)
                {
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.User]} {_localizer[SharedResourceKeys.NotFound]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.NotFound
                    };
                }

                var parentComment = _unitOfWork.CommentRepository.FirstOrDefault(n => n.Id == apiComment.ParentCommentId);
                if (parentComment == null)
                {
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.TaskCard]} {_localizer[SharedResourceKeys.NotFound]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.NotFound
                    };
                }
                var taskCard = _unitOfWork.TaskCardRepository.FirstOrDefault(t => t.Id == parentComment.TaskId);
                if (taskCard == null)
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
                mapApiComment.TaskCards = taskCard;
                mapApiComment.Users = user;

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
                var taskCard = _unitOfWork.TaskCardRepository.FirstOrDefault(t => t.Id == TaskId);
                if (taskCard == null)
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
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    #region Check comment exists
                    var comment = _unitOfWork.CommentRepository.FirstOrDefault(t => t.Id == idComment);
                    if (comment == null)
                    {
                        transaction.Rollback();
                        return new ResultObject
                        {
                            Data = false,
                            Success = true,
                            StatusCode = EnumStatusCodesResult.Success
                        };
                    }
                    #endregion

                    #region Delete from Database
                    _unitOfWork.CommentRepository.Remove(comment);
                    var response = _unitOfWork.SaveChangesBool();
                    if (!response)
                    {
                        transaction.Rollback();
                        return new ResultObject
                        {
                            Message = $"{_localizer[SharedResourceKeys.SaveChanges]} {_localizer[SharedResourceKeys.Error]}",
                            Success = true,
                            StatusCode = EnumStatusCodesResult.InternalServerError
                        };
                    }

                    var deleteChildTask = _deleteChild.DeleteChildComment(idComment);
                    await deleteChildTask;

                    var saveChangesResult = _unitOfWork.SaveChangesBool();
                    if (!saveChangesResult)
                    {
                        transaction.Rollback();
                        return new ResultObject
                        {
                            Message = $"{_localizer[SharedResourceKeys.SaveChanges]} {_localizer[SharedResourceKeys.Error]}",
                            Success = true,
                            StatusCode = EnumStatusCodesResult.InternalServerError
                        };
                    }
                    #endregion

                    transaction.Commit();

                    return new ResultObject
                    {
                        Data = true,
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
    }
}
