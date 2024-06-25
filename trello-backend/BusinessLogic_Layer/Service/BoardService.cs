using AutoMapper;
using BusinessLogic_Layer.Common;
using BusinessLogic_Layer.Entity;
using BusinessLogic_Layer.Enums;
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

        public BoardService(IUnitOfWork unitOfWork, IMapper mapper, IStringLocalizer<SharedResource> localizer,
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
                var data = _unitOfWork.BoardRepository.GetAll();
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
        public async Task<ResultObject> Create(ApiBoard apiBoard)
        {
            try
            {
                #region Check workspace exists
                var workspace = _unitOfWork.WorkspaceRepository.FirstOrDefault(n => n.Id == apiBoard.WorkspaceId);
                if (workspace == null)
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.Workspace]} {_localizer[SharedResourceKeys.NotFound]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.NotFound
                    };
                #endregion

                #region Add data to Databasee
                apiBoard.Id = Guid.NewGuid();
                var mapApiBoard = _mapper.Map<ApiBoard, Board>(apiBoard);
                _unitOfWork.BoardRepository.Add(mapApiBoard);
                var result = _unitOfWork.SaveChangesBool();
                if (!result)
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.SaveChanges]} {_localizer[SharedResourceKeys.Failde]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.InternalServerError
                    };
                #endregion

                #region Add document to Elasticsearch
                apiBoard.Type = ElasticsearchKeys.BoardType;
                var response = await _elasticsearchService.CreateDocumentAsync(apiBoard, ElasticsearchKeys.WorkspaceIndex);
                if(!response)
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.CreateFailde]} {_localizer[SharedResourceKeys.Document]} Elasticsearch ",
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
        public async Task<ResultObject> Update(ApiBoard apiBoard)
        {
            try
            {
                #region Check Board exists
                var checkBoard = _unitOfWork.BoardRepository.FirstOrDefault(n => n.Id == apiBoard.Id);
                if (checkBoard == null)
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.Board]} {_localizer[SharedResourceKeys.NotFound]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.NotFound
                    };
                #endregion

                #region Update to Database
                _mapper.Map(apiBoard, checkBoard);
                _unitOfWork.BoardRepository.Update(checkBoard);
                var result = _unitOfWork.SaveChangesBool();
                if (!result)
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.SaveChanges]} {_localizer[SharedResourceKeys.Failde]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.InternalServerError
                    };
                #endregion

                #region Update document to Elasticsearch
                apiBoard.Type = ElasticsearchKeys.BoardType;
                var response = await _elasticsearchService.UpdateDocumentAsync(apiBoard, ElasticsearchKeys.WorkspaceIndex, checkBoard.Id);
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
        public async Task<ResultObject> Delete(Guid idBoard)
        {
            try
            {
                #region Check Board exists
                var checkBoard = _unitOfWork.BoardRepository.FirstOrDefault(n => n.Id == idBoard);
                if (checkBoard == null)
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.Board]} {_localizer[SharedResourceKeys.NotFound]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.NotFound
                    };
                #endregion

                #region Delete to Database
                _unitOfWork.BoardRepository.Remove(checkBoard);
                var response = _unitOfWork.SaveChangesBool();
                if (!response)
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.SaveChanges]} {_localizer[SharedResourceKeys.Error]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.InternalServerError
                    };
                var deleteChildTask = _deleteChild.DeleteChildBoard(idBoard);
                await deleteChildTask;
                _unitOfWork.SaveChanges();
                #endregion

                #region Delete document to Elasticsearch
                var result = await _elasticsearchService.DeleteDocumentAsync(idBoard, ElasticsearchKeys.WorkspaceIndex);
                if (!result)
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.DeleteFailde]} {_localizer[SharedResourceKeys.Document]} Elasticsearch ",
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
    }
}
