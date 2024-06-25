using AutoMapper;
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
                var data = _unitOfWork.WorkflowRepository.GetAll();
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
        public async Task<ResultObject> Create(ApiWorkflow apiWorkflow)
        {
            try
            {
                #region Check board exists
                var checkBoard = _unitOfWork.BoardRepository.FirstOrDefault(n => n.Id == apiWorkflow.BoardId);
                if (checkBoard == null)
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.Board]} {_localizer[SharedResourceKeys.NotFound]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.Success
                    };
                #endregion

                apiWorkflow.Id = Guid.NewGuid();
                var mapApiWorkflow = _mapper.Map<ApiWorkflow, Workflow>(apiWorkflow);
                _unitOfWork.WorkflowRepository.Add(mapApiWorkflow);
                var result = _unitOfWork.SaveChangesBool();
                if (!result)
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.SaveChanges]} {_localizer[SharedResourceKeys.Failde]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.Success
                    };

                #region Add Document to Elasticsearch
                apiWorkflow.Type = ElasticsearchKeys.WorkflowType;
                var response = await _elasticsearchService.CreateDocumentAsync(apiWorkflow, ElasticsearchKeys.WorkspaceIndex);
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
        public async Task<ResultObject> Update(ApiWorkflow apiWorkflow)
        {
            try
            {
                #region Check Workflow exists
                var checkWorkflow = _unitOfWork.WorkflowRepository.FirstOrDefault(n => n.Id == apiWorkflow.Id);
                if (checkWorkflow == null)
                    return new ResultObject
                    {
                        Message = $"Workflow {_localizer[SharedResourceKeys.NotFound]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.Success
                    };
                #endregion

                _mapper.Map(apiWorkflow, checkWorkflow);

                _unitOfWork.WorkflowRepository.Update(checkWorkflow);
                var result = _unitOfWork.SaveChangesBool();
                if (!result)
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.SaveChanges]} {_localizer[SharedResourceKeys.Failde]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.Success
                    };

                #region Update Document to Elasticsearch
                apiWorkflow.Type = ElasticsearchKeys.WorkflowType;
                var response = await _elasticsearchService.UpdateDocumentAsync(apiWorkflow, ElasticsearchKeys.WorkspaceIndex, checkWorkflow.Id);
                if (!response)
                {
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.Update]} {_localizer[SharedResourceKeys.Document]} Elasticsearch ",
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
        public async Task<ResultObject> Delete(Guid idWorkflow)
        {
            try
            {
                #region Check Workflow exists
                var checkWorkflow = _unitOfWork.WorkflowRepository.FirstOrDefault(n => n.Id == idWorkflow);
                if (checkWorkflow == null)
                    return new ResultObject
                    {
                        Message = $"Workflow {_localizer[SharedResourceKeys.NotFound]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.Success
                    };
                #endregion

                _unitOfWork.WorkflowRepository.Remove(checkWorkflow);
                
                var result = _unitOfWork.SaveChangesBool();
                if (!result)
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.SaveChanges]} {_localizer[SharedResourceKeys.Failde]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.Success
                    };

                var deleteChildTask = _deleteChild.DeleteChildWorkflow(idWorkflow);
                await deleteChildTask;
                _unitOfWork.SaveChanges();

                #region Delete Document to Elasticsearch
                var response = await _elasticsearchService.DeleteDocumentAsync(idWorkflow, ElasticsearchKeys.WorkspaceIndex);
                if (!response)
                {
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.DeleteFailde]} {_localizer[SharedResourceKeys.Document]} Elasticsearch",
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
        }
    }
}
