using AutoMapper;
using BusinessLogic_Layer.Common;
using BusinessLogic_Layer.Entity;
using BusinessLogic_Layer.Enums;
using DataAccess_Layer.Interfaces;
using DataAccess_Layer.Models;
using Microsoft.Extensions.Localization;
using Org.BouncyCastle.Asn1.Ocsp;

namespace BusinessLogic_Layer.Service
{
    public class TaskCardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly ElasticsearchService _elasticsearchService;
        private readonly DeleteChild _deleteChild;

        public TaskCardService(IUnitOfWork unitOfWork, IMapper mapper, IStringLocalizer<SharedResource> localizer,
            ElasticsearchService elasticsearchService, DeleteChild deleteChild)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _localizer = localizer;
            _elasticsearchService = elasticsearchService;
            _deleteChild = deleteChild;
        }
        public async Task<ResultObject> GetAll()
        {
            try
            {
                var taskCards = _unitOfWork.TaskCardRepository.GetAll();
                return new ResultObject
                {
                    Data = taskCards,
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
        public async Task<ResultObject> GetAllCheckList()
        {
            try
            {
                var checkLists = _unitOfWork.CheckListRepository.GetAll();
                return new ResultObject
                {
                    Data = checkLists,
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
        public async Task<ResultObject> GetAllCheckListItem()
        {
            try
            {
                var checkListItems = _unitOfWork.CheckListItemRepository.GetAll();
                return new ResultObject
                {
                    Data = checkListItems,
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
        public async Task<ResultObject> Create(ApiTaskCard apiTaskCard)
        {
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    #region Check workflow exists
                    var workflow = _unitOfWork.WorkflowRepository.FirstOrDefault(n => n.Id == apiTaskCard.WorkflowId);
                    if (workflow == null)
                    {
                        return new ResultObject
                        {
                            Message = $"{_localizer[SharedResourceKeys.Workflow]} {_localizer[SharedResourceKeys.NotFound]}",
                            Success = true,
                            StatusCode = EnumStatusCodesResult.NotFound
                        };
                    }
                    #endregion

                    var checkPositionTask = _unitOfWork.TaskCardRepository.Context()
                               .Where(w => w.WorkflowId == workflow.Id && w.IsDeleted == false)
                               .OrderBy(w => w.Position)
                               .ToList();

                    int newPosition = 0;
                    if (checkPositionTask.Any())
                    {
                        newPosition = checkPositionTask.Last().Position + 1;
                    }

                    #region Add data to Database
                    apiTaskCard.Id = Guid.NewGuid();
                    var taskCard = _mapper.Map<ApiTaskCard, TaskCard>(apiTaskCard);
                    taskCard.Position = newPosition;
                    _unitOfWork.TaskCardRepository.Add(taskCard);
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

                    #region Add Document to Elasticsearch
                    apiTaskCard.Type = ElasticsearchKeys.TaskCardType;
                    var response = await _elasticsearchService.CreateDocumentAsync(apiTaskCard, ElasticsearchKeys.WorkspaceIndex);
                    if (!response)
                    {
                        transaction.Rollback();
                        return new ResultObject
                        {
                            Message = $"{_localizer[SharedResourceKeys.CreateFailde]} {_localizer[SharedResourceKeys.Document]} Elasticsearch",
                            Success = true,
                            StatusCode = EnumStatusCodesResult.InternalServerError
                        };
                    }
                    #endregion

                    transaction.Commit();

                    return new ResultObject
                    {
                        Data = taskCard,
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
        public async Task<ResultObject> Update(ApiTaskCard apiTaskCard)
        {
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    #region Check Task Card exists
                    var taskCard = _unitOfWork.TaskCardRepository.FirstOrDefault(n => n.Id == apiTaskCard.Id);
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

                    #region Update data to Database
                    _mapper.Map(apiTaskCard, taskCard);
                    _unitOfWork.TaskCardRepository.Update(taskCard);
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

                    #region Update Document in Elasticsearch
                    apiTaskCard.Type = ElasticsearchKeys.TaskCardType;
                    var response = await _elasticsearchService.UpdateDocumentAsync(apiTaskCard, ElasticsearchKeys.WorkspaceIndex, taskCard.Id);
                    if (!response)
                    {
                        transaction.Rollback();
                        return new ResultObject
                        {
                            Message = $"{_localizer[SharedResourceKeys.UploadFailde]} {_localizer[SharedResourceKeys.Document]} Elasticsearch",
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
        public async Task<ResultObject> Delete(Guid idTaskCard)
        {
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    #region Check Task Card exists
                    var taskCard = _unitOfWork.TaskCardRepository.FirstOrDefault(n => n.Id == idTaskCard);
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

                    #region Delete data from Database
                    _unitOfWork.TaskCardRepository.Remove(taskCard);
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

                    var checkPositionTask = _unitOfWork.TaskCardRepository.Context()
                        .Where(w => w.WorkflowId == taskCard.WorkflowId && w.IsDeleted == false)
                        .OrderBy(w => w.Position)
                        .ToList();

                    // Cập nhật vị trí của các task card khác
                    for (int i = 0; i < checkPositionTask.Count; i++)
                    {
                        checkPositionTask[i].Position = i;
                    }

                    _unitOfWork.TaskCardRepository.UpdateRange(checkPositionTask); // Cập nhật vị trí mới cho các task card
                    result = _unitOfWork.SaveChangesBool(); 
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

                    var deleteChildTask = _deleteChild.DeleteChildTaskCard(idTaskCard);
                    await deleteChildTask;

                    var resultDelete = _unitOfWork.SaveChangesBool();
                    if (!resultDelete)
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

                    #region Delete Document from Elasticsearch
                    var response = await _elasticsearchService.DeleteDocumentAsync(idTaskCard, ElasticsearchKeys.WorkspaceIndex);
                    if (!response)
                    {
                        transaction.Rollback();
                        return new ResultObject
                        {
                            Message = $"{_localizer[SharedResourceKeys.DeleteFailde]} {_localizer[SharedResourceKeys.Document]} Elasticsearch",
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
        public async Task<ResultObject> AddUserToTask(ApiUserTask apiUserTask)
        {
            try
            {
                #region Check Task Card and User exists
                var user = _unitOfWork.UserRepository
                    .FirstOrDefault(n => n.Id == apiUserTask.UserId);
                if (user == null)
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.User]} {_localizer[SharedResourceKeys.NotFound]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.NotFound
                    };

                var taskCard = _unitOfWork.TaskCardRepository
                    .FirstOrDefault(n => n.Id == apiUserTask.TaskId);
                if (taskCard == null)
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.TaskCard]} {_localizer[SharedResourceKeys.NotFound]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.NotFound
                    };

                //User already exists
                var checkExitst = _unitOfWork.TaskCardUserRepository
                    .FirstOrDefault(n => n.UserId == user.Id && n.TaskId == taskCard.Id);
                if (checkExitst != null)
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.User]} {_localizer[SharedResourceKeys.AlreadyExists]} " +
                        $"{_localizer[SharedResourceKeys.TaskCard]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.Unauthorized
                    };

                #endregion

                #region Add data to Database
                taskCard.TaskCardUsers = new List<TaskCardUser>();
                taskCard.TaskCardUsers.Add(new TaskCardUser
                {
                    UserId = user.Id,
                    TaskId = taskCard.Id
                });
                _unitOfWork.TaskCardRepository.Update(taskCard);
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
        public async Task<ResultObject> AddCheckListToTask(ApiCheckList apiCheckList)
        {
            try
            {
                #region Check Task Card exists
                var taskCard = _unitOfWork.TaskCardRepository
                    .FirstOrDefault(n => n.Id == apiCheckList.TaskId);
                if (taskCard == null)
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.TaskCard]} {_localizer[SharedResourceKeys.NotFound]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.NotFound
                    };

                #endregion

                var checkList = _mapper.Map<ApiCheckList, CheckList>(apiCheckList);
                checkList.TaskCards = taskCard;
                _unitOfWork.CheckListRepository.Add(checkList);
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
        public async Task<ResultObject> AddItemToCheckList(ApiCheckListItem apiCheckListItem)
        {
            try
            {
                #region Check Check List exists
                var checkList = _unitOfWork.CheckListRepository
                    .FirstOrDefault(n => n.Id == apiCheckListItem.CheckListId);
                if (checkList == null)
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.CheckList]} {_localizer[SharedResourceKeys.NotFound]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.NotFound
                    };
                #endregion

                var mapApiCheckList = _mapper.Map<ApiCheckListItem, CheckListItem>(apiCheckListItem);
                mapApiCheckList.TaskId = checkList.TaskId;
                _unitOfWork.CheckListItemRepository.Add(mapApiCheckList);
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
        public async Task<ResultObject> DeleteCheckList(Guid idCheckList)
        {
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    #region Check CheckList exists
                    var checkList = _unitOfWork.CheckListRepository.FirstOrDefault(t => t.Id == idCheckList);
                    if (checkList == null)
                    {
                        return new ResultObject
                        {
                            Message = $"{_localizer[SharedResourceKeys.CheckList]} {_localizer[SharedResourceKeys.NotFound]}",
                            Success = false,
                            StatusCode = EnumStatusCodesResult.NotFound
                        };
                    }
                    #endregion

                    #region Delete CheckList from Database
                    _unitOfWork.CheckListRepository.Remove(checkList);
                    var response = _unitOfWork.SaveChangesBool();
                    if (!response)
                    {
                        transaction.Rollback();
                        return new ResultObject
                        {
                            Message = $"{_localizer[SharedResourceKeys.SaveChanges]} {_localizer[SharedResourceKeys.Error]}",
                            Success = false,
                            StatusCode = EnumStatusCodesResult.InternalServerError
                        };
                    }

                    var deleteChildCheckList = _deleteChild.DeleteChildCheckList(idCheckList);
                    await deleteChildCheckList;
                    _unitOfWork.SaveChanges();
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
        public async Task<ResultObject> DeleteCheckListItem(Guid idCheckListItem)
        {
            try
            {
                #region Check CheckListItem exists
                var checkListItem = _unitOfWork.CheckListItemRepository.FirstOrDefault(t => t.Id == idCheckListItem);
                if (checkListItem == null)
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.CheckListItem]} {_localizer[SharedResourceKeys.NotFound]}",
                        Success = false,
                        StatusCode = EnumStatusCodesResult.NotFound
                    };
                #endregion

                _unitOfWork.CheckListItemRepository.Remove(checkListItem);
                var response = _unitOfWork.SaveChangesBool();
                if (!response)
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.SaveChanges]} {_localizer[SharedResourceKeys.Error]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.InternalServerError
                    };
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
        public async Task<bool> CheckTaskCard(Guid idTask)
        {
            try
            {
                var taskCard = _unitOfWork.TaskCardRepository.FirstOrDefault(n => n.Id == idTask);
                if (taskCard != null)
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                _unitOfWork.Dispose();
            }
        }
        public async Task<ResultObject> UpdateTaskCardPosition(UpdatePositionRequest request)
        {
            try
            {
                #region Check taskCard exists
                var taskCard = _unitOfWork.TaskCardRepository.FirstOrDefault(n => n.Id == request.MoveId);
                if (taskCard == null)
                {
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.TaskCard]} {_localizer[SharedResourceKeys.NotFound]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.Success
                    };
                }
                #endregion

                if (taskCard.WorkflowId == request.SpaceId)
                {
                    // Lấy danh sách các task card trong cùng bảng
                    var checkPositionCards = _unitOfWork.TaskCardRepository.Find(w => w.WorkflowId == request.SpaceId)
                         .OrderBy(w => w.Position)
                         .ToList();
                    if (request.NewPosition < 0 || request.NewPosition >= checkPositionCards.Count)
                    {
                        return new ResultObject
                        {
                            Message = $"{_localizer[SharedResourceKeys.Incorrect]}",
                            Success = false,
                            StatusCode = EnumStatusCodesResult.BadRequest
                        };
                    }

                    // Xóa workflow cần di chuyển khỏi danh sách
                    checkPositionCards.Remove(taskCard);

                    // Thêm workflow vào vị trí mới
                    checkPositionCards.Insert(request.NewPosition, taskCard);

                    // Cập nhật lại vị trí cho tất cả các workflow
                    for (int i = 0; i < checkPositionCards.Count; i++)
                    {
                        checkPositionCards[i].Position = i;
                    }

                    // Lưu thay đổi vào cơ sở dữ liệu
                    _unitOfWork.TaskCardRepository.UpdateRange(checkPositionCards);
                    var result = _unitOfWork.SaveChangesBool();
                    if (!result)
                    {
                        return new ResultObject
                        {
                            Message = $"{_localizer[SharedResourceKeys.SaveChanges]} {_localizer[SharedResourceKeys.Failde]}",
                            Success = true,
                            StatusCode = EnumStatusCodesResult.Success
                        };
                    }

                    return new ResultObject
                    {
                        Data = true,
                        Success = true,
                        StatusCode = EnumStatusCodesResult.Success
                    };
                }
                else
                {
                    var getCardInWorkflowOld = _unitOfWork.TaskCardRepository.Find(w => w.WorkflowId == taskCard.WorkflowId)
                        .OrderBy(w => w.Position)
                        .ToList();

                    var getCardInWorkflowNew = _unitOfWork.TaskCardRepository.Find(w => w.WorkflowId == request.SpaceId)
                       .OrderBy(w => w.Position)
                       .ToList();

                    // Xóa workflow cần di chuyển khỏi danh sách
                    getCardInWorkflowOld.Remove(taskCard);

                    for (int i = 0; i < getCardInWorkflowOld.Count; i++)
                    {
                        getCardInWorkflowOld[i].Position = i;
                    }

                    // Thêm workflow vào vị trí mới

                    taskCard.WorkflowId = request.SpaceId;
                    getCardInWorkflowNew.Insert(request.NewPosition, taskCard);
                    // Cập nhật lại vị trí cho tất cả các workflow
                    for (int i = 0; i < getCardInWorkflowNew.Count; i++)
                    {
                        getCardInWorkflowNew[i].Position = i;
                    }
                    // Lưu thay đổi vào cơ sở dữ liệu
                    _unitOfWork.TaskCardRepository.UpdateRange(getCardInWorkflowOld);
                    _unitOfWork.TaskCardRepository.UpdateRange(getCardInWorkflowNew);
                    var result = _unitOfWork.SaveChangesBool();
                    if (!result)
                    {
                        return new ResultObject
                        {
                            Message = $"{_localizer[SharedResourceKeys.SaveChanges]} {_localizer[SharedResourceKeys.Failde]}",
                            Success = true,
                            StatusCode = EnumStatusCodesResult.Success
                        };
                    }

                    return new ResultObject
                    {
                        Data = true,
                        Success = true,
                        StatusCode = EnumStatusCodesResult.Success
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
    }
}
