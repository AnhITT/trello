﻿using AutoMapper;
using BusinessLogic_Layer.Common;
using BusinessLogic_Layer.Entity;
using BusinessLogic_Layer.Enums;
using DataAccess_Layer.Interfaces;
using DataAccess_Layer.Models;
using Microsoft.Extensions.Localization;

namespace BusinessLogic_Layer.Service
{
    public class WorkflowService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly ElasticsearchService _elasticsearchService;
        private readonly DeleteChild _deleteChild;

        public WorkflowService(IUnitOfWork unitOfWork, IMapper mapper, IStringLocalizer<SharedResource> localizer,
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
                var workflows = _unitOfWork.WorkflowRepository.GetAll();
                return new ResultObject
                {
                    Data = workflows,
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
        public async Task<ResultObject> GetByBoardID(Guid boardId)
        {
            try
            {
                #region Check board exists
                var board = _unitOfWork.BoardRepository.FirstOrDefault(n => n.Id == boardId);
                if (board == null)
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.Board]} {_localizer[SharedResourceKeys.NotFound]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.Success
                    };
                #endregion

                var workflows = _unitOfWork.WorkflowRepository.GetAll().Where(n => n.BoardId == boardId);
                return new ResultObject
                {
                    Data = workflows,
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
        public async Task<ResultObject> Create(ApiWorkflow apiWorkflow)
        {
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    #region Check board exists
                    var board = _unitOfWork.BoardRepository.FirstOrDefault(n => n.Id == apiWorkflow.BoardId);
                    if (board == null)
                    {
                        return new ResultObject
                        {
                            Message = $"{_localizer[SharedResourceKeys.Board]} {_localizer[SharedResourceKeys.NotFound]}",
                            Success = true,
                            StatusCode = EnumStatusCodesResult.Success
                        };
                    }
                    #endregion

                    var checkPositionWorkflows = _unitOfWork.WorkflowRepository.Context()
                                .Where(w => w.BoardId == board.Id && w.IsDeleted == false)
                                .OrderBy(w => w.Position)
                                .ToList();

                    int newPosition = 0;
                    if (checkPositionWorkflows.Any())
                    {
                        newPosition = checkPositionWorkflows.Last().Position + 1;
                    }

                    apiWorkflow.Id = Guid.NewGuid();
                    var mapApiWorkflow = _mapper.Map<ApiWorkflow, Workflow>(apiWorkflow);
                    mapApiWorkflow.Position = newPosition;
                    _unitOfWork.WorkflowRepository.Add(mapApiWorkflow);
                    var result = _unitOfWork.SaveChangesBool();
                    if (!result)
                    {
                        transaction.Rollback();
                        return new ResultObject
                        {
                            Message = $"{_localizer[SharedResourceKeys.SaveChanges]} {_localizer[SharedResourceKeys.Failde]}",
                            Success = true,
                            StatusCode = EnumStatusCodesResult.Success
                        };
                    }

                    #region Add Document to Elasticsearch
                    apiWorkflow.Type = ElasticsearchKeys.WorkflowType;
                    var response = await _elasticsearchService.CreateDocumentAsync(apiWorkflow, ElasticsearchKeys.WorkspaceIndex);
                    if (!response)
                    {
                        transaction.Rollback();
                        return new ResultObject
                        {
                            Message = $"{_localizer[SharedResourceKeys.CreateFailde]} {_localizer[SharedResourceKeys.Document]} Elasticsearch",
                            Success = true,
                            StatusCode = EnumStatusCodesResult.Success
                        };
                    }
                    #endregion

                    transaction.Commit();
                    return new ResultObject
                    {
                        Data = mapApiWorkflow,
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
        public async Task<ResultObject> Update(ApiWorkflow apiWorkflow)
        {
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    #region Check Workflow exists
                    var workflow = _unitOfWork.WorkflowRepository.FirstOrDefault(n => n.Id == apiWorkflow.Id);
                    if (workflow == null)
                    {
                        return new ResultObject
                        {
                            Message = $"Workflow {_localizer[SharedResourceKeys.NotFound]}",
                            Success = true,
                            StatusCode = EnumStatusCodesResult.Success
                        };
                    }
                    #endregion

                    _mapper.Map(apiWorkflow, workflow);

                    _unitOfWork.WorkflowRepository.Update(workflow);
                    var result = _unitOfWork.SaveChangesBool();
                    if (!result)
                    {
                        transaction.Rollback();
                        return new ResultObject
                        {
                            Message = $"{_localizer[SharedResourceKeys.SaveChanges]} {_localizer[SharedResourceKeys.Failde]}",
                            Success = true,
                            StatusCode = EnumStatusCodesResult.Success
                        };
                    }

                    #region Update Document to Elasticsearch
                    apiWorkflow.Type = ElasticsearchKeys.WorkflowType;
                    var response = await _elasticsearchService.UpdateDocumentAsync(apiWorkflow, ElasticsearchKeys.WorkspaceIndex, workflow.Id);
                    if (!response)
                    {
                        transaction.Rollback();
                        return new ResultObject
                        {
                            Message = $"{_localizer[SharedResourceKeys.Update]} {_localizer[SharedResourceKeys.Document]} Elasticsearch",
                            Success = true,
                            StatusCode = EnumStatusCodesResult.Success
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
        public async Task<ResultObject> UpdateWorkflowPosition(UpdatePositionRequest request)
        {
            try
            {
                // Lấy danh sách các workflow trong cùng bảng, không bao gồm các workflow đã bị xóa
                var checkPositionWorkflows = _unitOfWork.WorkflowRepository.Find(w => w.BoardId == request.SpaceId)
                    .OrderBy(w => w.Position)
                    .ToList();

                // Lấy workflow cần di chuyển
                var workflowToMove = _unitOfWork.WorkflowRepository.FirstOrDefault(w => w.Id == request.MoveId);
                if (workflowToMove == null)
                {
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.Workflow]} {_localizer[SharedResourceKeys.NotFound]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.Success
                    };
                }

                // Kiểm tra xem vị trí mới có hợp lệ hay không
                if (request.NewPosition < 0 || request.NewPosition >= checkPositionWorkflows.Count)
                {
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.Incorrect]}",
                        Success = false,
                        StatusCode = EnumStatusCodesResult.BadRequest
                    };
                }

                // Xóa workflow cần di chuyển khỏi danh sách
                checkPositionWorkflows.Remove(workflowToMove);

                // Thêm workflow vào vị trí mới
                checkPositionWorkflows.Insert(request.NewPosition, workflowToMove);

                // Cập nhật lại vị trí cho tất cả các workflow
                for (int i = 0; i < checkPositionWorkflows.Count; i++)
                {
                    checkPositionWorkflows[i].Position = i;
                }

                // Lưu thay đổi vào cơ sở dữ liệu
                _unitOfWork.WorkflowRepository.UpdateRange(checkPositionWorkflows);
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
        public async Task<ResultObject> Delete(Guid idWorkflow)
        {
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    #region Check Workflow exists
                    var workflow = _unitOfWork.WorkflowRepository.FirstOrDefault(n => n.Id == idWorkflow);
                    if (workflow == null)
                    {
                        return new ResultObject
                        {
                            Message = $"Workflow {_localizer[SharedResourceKeys.NotFound]}",
                            Success = true,
                            StatusCode = EnumStatusCodesResult.Success
                        };
                    }
                    #endregion

                    _unitOfWork.WorkflowRepository.Remove(workflow);

                    var result = _unitOfWork.SaveChangesBool();
                    if (!result)
                    {
                        transaction.Rollback();
                        return new ResultObject
                        {
                            Message = $"{_localizer[SharedResourceKeys.SaveChanges]} {_localizer[SharedResourceKeys.Failde]}",
                            Success = true,
                            StatusCode = EnumStatusCodesResult.Success
                        };
                    }

                    var checkPositionWorkflows = _unitOfWork.WorkflowRepository.Context()
                              .Where(w => w.BoardId == workflow.BoardId && w.IsDeleted == false)
                              .OrderBy(w => w.Position)
                              .ToList();

                    // Cập nhật vị trí của các workflow khác
                    for (int i = 0; i < checkPositionWorkflows.Count; i++)
                    {
                        checkPositionWorkflows[i].Position = i;
                    }

                    var deleteChildTask = _deleteChild.DeleteChildWorkflow(idWorkflow);
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

                    #region Delete Document from Elasticsearch
                    var response = await _elasticsearchService.DeleteDocumentAsync(idWorkflow, ElasticsearchKeys.WorkspaceIndex);
                    if (!response)
                    {
                        transaction.Rollback();
                        return new ResultObject
                        {
                            Message = $"{_localizer[SharedResourceKeys.DeleteFailde]} {_localizer[SharedResourceKeys.Document]} Elasticsearch",
                            Success = true,
                            StatusCode = EnumStatusCodesResult.Success
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
