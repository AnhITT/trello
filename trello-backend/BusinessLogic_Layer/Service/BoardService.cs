using AutoMapper;
using BusinessLogic_Layer.Common;
using BusinessLogic_Layer.Entity;
using BusinessLogic_Layer.Enums;
using BusinessLogic_LayerDataAccess_Layer.Common;
using DataAccess_Layer.Interfaces;
using DataAccess_Layer.Models;
using Microsoft.Extensions.Localization;


namespace BusinessLogic_Layer.Service
{
    public class BoardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly ElasticsearchService _elasticsearchService;
        private readonly DeleteChild _deleteChild;
        public readonly CallApi _callApi;

        public BoardService(IUnitOfWork unitOfWork, IMapper mapper, IStringLocalizer<SharedResource> localizer,
            ElasticsearchService elasticsearchService, DeleteChild deleteChild, CallApi callApi)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _localizer = localizer;
            _elasticsearchService = elasticsearchService;
            _deleteChild = deleteChild;
            _callApi = callApi;
        }
        public async Task<ResultObject> GetAll()
        {
            try
            {
                var boards = _unitOfWork.BoardRepository.GetAll();
                return new ResultObject
                {
                    Data = boards,
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
        public async Task<ResultObject> GetById(Guid idBoard)
        {
            try
            {
                var board = _unitOfWork.BoardRepository.FirstOrDefault(n => n.Id == idBoard);
                if (board == null)
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.Board]} {_localizer[SharedResourceKeys.NotFound]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.Success
                    };
                return new ResultObject
                {
                    Data = board,
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
        public async Task<ResultObject> GetAllPropertiesFromBoard(Guid idBoard)
        {
            try
            {
                var board = _unitOfWork.BoardRepository.FirstOrDefault(b => b.Id == idBoard);

                if (board == null)
                {
                    return new ResultObject
                    {
                        Message = "Board not found.",
                        Success = false,
                        StatusCode = EnumStatusCodesResult.NotFound
                    };
                }

                var workflows = (from workflow in _unitOfWork.WorkflowRepository.Context()
                                 where workflow.BoardId == idBoard
                                 where workflow.IsDeleted == false
                                 orderby workflow.Position
                                 select new
                                 {
                                     Id = workflow.Id,
                                     Name = workflow.Name,
                                     Position = workflow.Position,
                                     Cards = (from card in _unitOfWork.TaskCardRepository.Context()
                                              where card.WorkflowId == workflow.Id
                                              where card.IsDeleted == false
                                              orderby card.Position
                                              select new
                                              {
                                                  Id = card.Id,
                                                  Title = card.Title,
                                                  Description = card.Description,
                                                  Cover = card.Cover,
                                                  Position = card.Position,
                                                  workflowId = workflow.Id,
                                                  UserCount = card.TaskCardUsers.Count(),
                                                  CompletedChecklistItems = _unitOfWork.CheckListItemRepository.Context()
                                                  .Count(cli => cli.TaskId == card.Id && cli.IsCompleted),
                                                      TotalChecklistItems = _unitOfWork.CheckListItemRepository.Context()
                                                  .Count(cli => cli.TaskId == card.Id),
                                                  Files = _callApi.GetFilesForTask(card.Id).Result.Count,
                                              }).ToList()
                                 }).ToList();

                var result = new
                {
                    Id = board.Id,
                    Name = board.Name,
                    Workflows = workflows
                };

                return new ResultObject
                {
                    Data = result,
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
        public async Task<ResultObject> Create(ApiBoard apiBoard)
        {
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    #region Check board exists
                    var board = _unitOfWork.WorkspaceRepository.FirstOrDefault(n => n.Id == apiBoard.WorkspaceId);
                    if (board == null)
                    {
                        return new ResultObject
                        {
                            Message = $"{_localizer[SharedResourceKeys.Board]} {_localizer[SharedResourceKeys.NotFound]}",
                            Success = true,
                            StatusCode = EnumStatusCodesResult.NotFound
                        };
                    }
                    #endregion

                    #region Add data to Database
                    apiBoard.Id = Guid.NewGuid();
                    var mapApiBoard = _mapper.Map<ApiBoard, Board>(apiBoard);
                    _unitOfWork.BoardRepository.Add(mapApiBoard);
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

                    #region Add document to Elasticsearch
                    apiBoard.Type = ElasticsearchKeys.BoardType;
                    var response = await _elasticsearchService.CreateDocumentAsync(apiBoard, ElasticsearchKeys.WorkspaceIndex);
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
        public async Task<ResultObject> Update(ApiBoard apiBoard)
        {
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    #region Check Board exists
                    var board = _unitOfWork.BoardRepository.FirstOrDefault(n => n.Id == apiBoard.Id);
                    if (board == null)
                    {
                        return new ResultObject
                        {
                            Message = $"{_localizer[SharedResourceKeys.Board]} {_localizer[SharedResourceKeys.NotFound]}",
                            Success = true,
                            StatusCode = EnumStatusCodesResult.NotFound
                        };
                    }
                    #endregion

                    #region Update to Database
                    _mapper.Map(apiBoard, board);
                    _unitOfWork.BoardRepository.Update(board);
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

                    #region Update document to Elasticsearch
                    apiBoard.Type = ElasticsearchKeys.BoardType;
                    var response = await _elasticsearchService.UpdateDocumentAsync(apiBoard, ElasticsearchKeys.WorkspaceIndex, board.Id);
                    if (!response)
                    {
                        transaction.Rollback();
                        return new ResultObject
                        {
                            Message = $"{_localizer[SharedResourceKeys.UploadFailde]} {_localizer[SharedResourceKeys.Document]} Elasticsearch ",
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
        public async Task<ResultObject> Delete(Guid idBoard)
        {
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    #region Check Board exists
                    var board = _unitOfWork.BoardRepository.FirstOrDefault(n => n.Id == idBoard);
                    if (board == null)
                    {
                        return new ResultObject
                        {
                            Message = $"{_localizer[SharedResourceKeys.Board]} {_localizer[SharedResourceKeys.NotFound]}",
                            Success = true,
                            StatusCode = EnumStatusCodesResult.NotFound
                        };
                    }
                    #endregion

                    #region Delete from Database
                    _unitOfWork.BoardRepository.Remove(board);
                    var result = _unitOfWork.SaveChangesBool();
                    if (!result)
                    {
                        transaction.Rollback();
                        return new ResultObject
                        {
                            Message = $"{_localizer[SharedResourceKeys.SaveChanges]} {_localizer[SharedResourceKeys.Error]}",
                            Success = true,
                            StatusCode = EnumStatusCodesResult.InternalServerError
                        };
                    }

                    // Delete child tasks asynchronously
                    var deleteChildTask = _deleteChild.DeleteChildBoard(idBoard);
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

                    #region Delete document from Elasticsearch
                    var resultElastic = await _elasticsearchService.DeleteDocumentAsync(idBoard, ElasticsearchKeys.WorkspaceIndex);
                    if (!resultElastic)
                    {
                        transaction.Rollback();
                        return new ResultObject
                        {
                            Message = $"{_localizer[SharedResourceKeys.DeleteFailde]} {_localizer[SharedResourceKeys.Document]} Elasticsearch ",
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
