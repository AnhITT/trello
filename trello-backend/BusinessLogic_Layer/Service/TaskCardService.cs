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
                var data = _unitOfWork.TaskCardRepository.GetAll();
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
        public async Task<ResultObject> GetAllCheckList()
        {
            try
            {
                var data = _unitOfWork.CheckListRepository.GetAll();
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
        public async Task<ResultObject> GetAllCheckListItem()
        {
            try
            {
                var data = _unitOfWork.CheckListItemRepository.GetAll();
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
        public async Task<ResultObject> Create(ApiTaskCard apiTaskCard)
        {
            try
            {
                #region Check workflow exists
                var checkWorkflow = _unitOfWork.WorkflowRepository.FirstOrDefault(n => n.Id == apiTaskCard.WorkflowId);
                if (checkWorkflow == null)
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.Workflow]} {_localizer[SharedResourceKeys.NotFound]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.NotFound
                    };
                #endregion

                var checkPositionTask = _unitOfWork.TaskCardRepository.Context()
                           .Where(w => w.WorkflowId == checkWorkflow.Id && w.IsDeleted == false)
                           .OrderBy(w => w.Position)
                           .ToList();

                int newPosition = 0;
                if (checkPositionTask.Any())
                {
                    newPosition = checkPositionTask.Last().Position + 1;
                }

                #region Add data to Database
                apiTaskCard.Id = Guid.NewGuid();
                var mapApiTaskCard = _mapper.Map<ApiTaskCard, TaskCard>(apiTaskCard);
                mapApiTaskCard.Position = newPosition;
                _unitOfWork.TaskCardRepository.Add(mapApiTaskCard);
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
                apiTaskCard.Type = ElasticsearchKeys.TaskCardType;
                var response = await _elasticsearchService.CreateDocumentAsync(apiTaskCard, ElasticsearchKeys.WorkspaceIndex);
                if (!response)
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.CreateFailde]} {_localizer[SharedResourceKeys.Document]} Elasticsearch",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.InternalServerError
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
        public async Task<ResultObject> Update(ApiTaskCard apiTaskCard)
        {
            try
            {
                #region Check Task Card exists
                var checkTaskCard = _unitOfWork.TaskCardRepository.FirstOrDefault(n => n.Id == apiTaskCard.Id);
                if (checkTaskCard == null)
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.TaskCard]} {_localizer[SharedResourceKeys.NotFound]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.NotFound
                    };
                #endregion

                #region Update data to Database
                _mapper.Map(apiTaskCard, checkTaskCard);

                _unitOfWork.TaskCardRepository.Update(checkTaskCard);
                var result = _unitOfWork.SaveChangesBool();
                if (!result)
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.SaveChanges]} {_localizer[SharedResourceKeys.Failde]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.InternalServerError
                    };
                #endregion

                #region Update Document Elasticsearch
                apiTaskCard.Type = ElasticsearchKeys.TaskCardType;
                var response = await _elasticsearchService.UpdateDocumentAsync(apiTaskCard, ElasticsearchKeys.WorkspaceIndex, checkTaskCard.Id);
                if (!response)
                {
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.UploadFailde]} {_localizer[SharedResourceKeys.Document]} Elasticsearch ",
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
        public async Task<ResultObject> Delete(Guid idTaskCard)
        {
            try
            {
                #region Check Task Card exists
                var checkTaskCard = _unitOfWork.TaskCardRepository.FirstOrDefault(n => n.Id == idTaskCard);
                if (checkTaskCard == null)
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.TaskCard]} {_localizer[SharedResourceKeys.NotFound]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.NotFound
                    };
                #endregion

                #region Delete data to Database
                _unitOfWork.TaskCardRepository.Remove(checkTaskCard);
                var result = _unitOfWork.SaveChangesBool();
                if (!result)
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.SaveChanges]} {_localizer[SharedResourceKeys.Failde]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.InternalServerError
                    };

                var checkPositionTask = _unitOfWork.TaskCardRepository.Context()
                   .Where(w => w.WorkflowId == checkTaskCard.WorkflowId && w.IsDeleted == false)
                   .OrderBy(w => w.Position)
                   .ToList();

                // Cập nhật vị trí của các workflow khác
                for (int i = 0; i < checkPositionTask.Count; i++)
                {
                    checkPositionTask[i].Position = i;
                }

                var deleteChildTask = _deleteChild.DeleteChildTaskCard(idTaskCard);
                await deleteChildTask;
                _unitOfWork.SaveChanges();
                #endregion

                #region Delete Document Elasticsearch
                var response = await _elasticsearchService.DeleteDocumentAsync(idTaskCard, ElasticsearchKeys.WorkspaceIndex);
                if (!response)
                {
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.DeleteFailde]} {_localizer[SharedResourceKeys.Document]} Elasticsearch ",
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
        public async Task<ResultObject> AddUserToTask(ApiUserTask apiUserTask)
        {
            try
            {
                #region Check Task Card and User exists
                var checkUser = _unitOfWork.UserRepository
                    .FirstOrDefault(n => n.Id == apiUserTask.UserId);
                if (checkUser == null)
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.User]} {_localizer[SharedResourceKeys.NotFound]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.NotFound
                    };

                var checkTaskCard = _unitOfWork.TaskCardRepository
                    .FirstOrDefault(n => n.Id == apiUserTask.TaskId);
                if (checkTaskCard == null)
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.TaskCard]} {_localizer[SharedResourceKeys.NotFound]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.NotFound
                    };

                //User already exists
                var checkExitst = _unitOfWork.TaskCardUserRepository
                    .FirstOrDefault(n => n.UserId == checkUser.Id && n.TaskId == checkTaskCard.Id);
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
                checkTaskCard.TaskCardUsers = new List<TaskCardUser>();
                checkTaskCard.TaskCardUsers.Add(new TaskCardUser
                {
                    UserId = checkUser.Id,
                    TaskId = checkTaskCard.Id
                });
                _unitOfWork.TaskCardRepository.Update(checkTaskCard);
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
                var checkTaskCard = _unitOfWork.TaskCardRepository
                    .FirstOrDefault(n => n.Id == apiCheckList.TaskId);
                if (checkTaskCard == null)
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.TaskCard]} {_localizer[SharedResourceKeys.NotFound]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.NotFound
                    };

                #endregion

                var mapApiCheckList = _mapper.Map<ApiCheckList, CheckList>(apiCheckList);
                mapApiCheckList.TaskCards = checkTaskCard;
                _unitOfWork.CheckListRepository.Add(mapApiCheckList);
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
                #region Check Task Card exists
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
            try
            {
                #region Check CheckList exists
                var checkListDelete = _unitOfWork.CheckListRepository.FirstOrDefault(t => t.Id == idCheckList);
                if (checkListDelete == null)
                {
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.CheckList]} {_localizer[SharedResourceKeys.NotFound]}",
                        Success = false,
                        StatusCode = EnumStatusCodesResult.NotFound
                    };
                }
                #endregion
                
                _unitOfWork.CheckListRepository.Remove(checkListDelete);
                var response = _unitOfWork.SaveChangesBool();
                if (!response)
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.SaveChanges]} {_localizer[SharedResourceKeys.Error]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.InternalServerError
                    };

                var deleteChildCheckList = _deleteChild.DeleteChildCheckList(idCheckList);
                await deleteChildCheckList;
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
        public async Task<ResultObject> DeleteCheckListItem(Guid idCheckListItem)
        {
            try
            {
                #region Check CheckListItem exists
                var checkListItemDelete = _unitOfWork.CheckListItemRepository.FirstOrDefault(t => t.Id == idCheckListItem);
                if (checkListItemDelete == null)
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.CheckListItem]} {_localizer[SharedResourceKeys.NotFound]}",
                        Success = false,
                        StatusCode = EnumStatusCodesResult.NotFound
                    };
                #endregion

                _unitOfWork.CheckListItemRepository.Remove(checkListItemDelete);
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
                var data = _unitOfWork.TaskCardRepository.FirstOrDefault(n => n.Id == idTask);
                if (data != null)
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
                var checkCard = _unitOfWork.TaskCardRepository.FirstOrDefault(n => n.Id == request.MoveId);
                if (checkCard == null)
                {
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.TaskCard]} {_localizer[SharedResourceKeys.NotFound]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.Success
                    };
                }

                if (checkCard.WorkflowId == request.SpaceId)
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
                    checkPositionCards.Remove(checkCard);

                    // Thêm workflow vào vị trí mới
                    checkPositionCards.Insert(request.NewPosition, checkCard);

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
                    var getCardInWorkflowOld = _unitOfWork.TaskCardRepository.Find(w => w.WorkflowId == checkCard.WorkflowId)
                        .OrderBy(w => w.Position)
                        .ToList();

                    var getCardInWorkflowNew = _unitOfWork.TaskCardRepository.Find(w => w.WorkflowId == request.SpaceId)
                       .OrderBy(w => w.Position)
                       .ToList();

                    // Xóa workflow cần di chuyển khỏi danh sách
                    getCardInWorkflowOld.Remove(checkCard);

                    for (int i = 0; i < getCardInWorkflowOld.Count; i++)
                    {
                        getCardInWorkflowOld[i].Position = i;
                    }

                    // Thêm workflow vào vị trí mới
                    
                    checkCard.WorkflowId = request.SpaceId;
                    getCardInWorkflowNew.Insert(request.NewPosition, checkCard);
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
