using AutoMapper;
using BusinessLogic_Layer.Common;
using BusinessLogic_Layer.Entity;
using BusinessLogic_Layer.Enums;
using DataAccess_Layer.Interfaces;
using DataAccess_Layer.Models;
using Microsoft.Extensions.Localization;

namespace BusinessLogic_Layer.Service
{
    public class WorkspaceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly ElasticsearchService _elasticsearchService;
        private readonly DeleteChild _deleteChild;

        public WorkspaceService(IUnitOfWork unitOfWork, IMapper mapper, IStringLocalizer<SharedResource> localizer,
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
                var data = _unitOfWork.WorkspaceRepository.GetAll();
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
        public async Task<ResultObject> GetAllProptiesFromWorkspace()
        {
            try
            {
                var workspaces = _unitOfWork.WorkspaceRepository.Context().ToList();

                var users = (from workspaceUser in _unitOfWork.WorkspaceUserRepository.Context()
                             join user in _unitOfWork.UserRepository.Context() on workspaceUser.UserId equals user.Id
                             select new
                             {
                                 workspaceUser.WorkspaceId,
                                 User = new
                                 {
                                     Id = user.Id,
                                     Name = $"{user.FirstName} {user.LastName}",
                                     Email = user.Email
                                 }
                             }).ToList();

                var boards = (from board in _unitOfWork.BoardRepository.Context()
                              select new
                              {
                                  board.WorkspaceId,
                                  Board = new
                                  {
                                      Id = board.Id,
                                      Name = board.Name
                                  }
                              }).ToList();

                var result = (from workspace in workspaces
                              select new
                              {
                                  Id = workspace.Id,
                                  Name = workspace.Name,
                                  Users = users.Where(u => u.WorkspaceId == workspace.Id).Select(u => u.User).ToList(),
                                  Boards = boards.Where(b => b.WorkspaceId == workspace.Id).Select(b => b.Board).ToList()
                              }).ToList();

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
        public async Task<ResultObject> Create(ApiWorkspace apiWorkspace)
        {
            try
            {
                apiWorkspace.Id = Guid.NewGuid();
                var mapApiWorkspace = _mapper.Map<ApiWorkspace, Workspace>(apiWorkspace);

                if (apiWorkspace.UserIds.Count != 0)
                {
                    mapApiWorkspace.WorkspaceUsers = new List<WorkspaceUser>();
                    foreach (var userId in apiWorkspace.UserIds)
                    {
                        var user = _unitOfWork.UserRepository.FirstOrDefault(n => n.Id == userId);
                        if (user == null)
                        {
                            return new ResultObject
                            {
                                Message = $"{_localizer[SharedResourceKeys.User]} {user.Email} {_localizer[SharedResourceKeys.NotFound]}",
                                Success = true,
                                StatusCode = EnumStatusCodesResult.Success
                            };
                        }
                        mapApiWorkspace.WorkspaceUsers.Add(new WorkspaceUser
                        {
                            UserId = user.Id,
                            WorkspaceId = mapApiWorkspace.Id
                        });
                    }
                }

                _unitOfWork.WorkspaceRepository.Add(mapApiWorkspace);
                var result = _unitOfWork.SaveChangesBool();
                if (!result)
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.SaveChanges]} {_localizer[SharedResourceKeys.Failde]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.Success
                    };

                #region Add Document to Elasticsearch
                apiWorkspace.Type = ElasticsearchKeys.WorkspaceType;
                var response = await _elasticsearchService.CreateDocumentAsync(apiWorkspace, ElasticsearchKeys.WorkspaceIndex);
                if (!response)
                {
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.CreateFailde]} {_localizer[SharedResourceKeys.Document]} Elasticsearch ",
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
        public async Task<ResultObject> Update(ApiWorkspace apiWorkspace)
        {
            try
            {
                #region Check workspace exists
                var checkWorkspace = _unitOfWork.WorkspaceRepository.FirstOrDefault(n => n.Id == apiWorkspace.Id);
                if (checkWorkspace == null)
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.Workspace]} {_localizer[SharedResourceKeys.NotFound]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.NotFound
                    };
                #endregion

                _mapper.Map(apiWorkspace, checkWorkspace);

                #region Update WorkspaceUser relations
                var existingWorkspaceUsers = _unitOfWork.WorkspaceUserRepository
                                                            .Find(wu => wu.WorkspaceId == checkWorkspace.Id)
                                                            .ToList();

                var newUserIds = apiWorkspace.UserIds;

                // Find users to remove
                var usersToRemove = existingWorkspaceUsers
                    .Where(e => !newUserIds.Contains(e.UserId))
                    .ToList();

                // Find users to add
                var usersToAdd = newUserIds
                    .Where(id => !existingWorkspaceUsers.Any(e => e.UserId == id))
                    .Select(id => new WorkspaceUser { WorkspaceId = checkWorkspace.Id, UserId = id })
                    .ToList();

                _unitOfWork.WorkspaceUserRepository.RemoveRange(usersToRemove);
                _unitOfWork.WorkspaceUserRepository.AddRange(usersToAdd);
                #endregion

                _unitOfWork.WorkspaceRepository.Update(checkWorkspace);
                var result = _unitOfWork.SaveChangesBool();
                if (!result)
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.SaveChanges]} {_localizer[SharedResourceKeys.Failde]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.InternalServerError
                    };

                #region Update Document Elasticsearch
                apiWorkspace.Type = ElasticsearchKeys.WorkspaceType;
                var response = await _elasticsearchService.UpdateDocumentAsync(apiWorkspace, ElasticsearchKeys.WorkspaceIndex, checkWorkspace.Id);
                if (!response)
                {
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.UpdateFailde]} {_localizer[SharedResourceKeys.Document]} Elasticsearch",
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
        public async Task<ResultObject> Delete(Guid idWorkspace)
        {
            try
            {
                #region Check workspace exists
                var checkWorkspace = _unitOfWork.WorkspaceRepository.FirstOrDefault(n => n.Id == idWorkspace);
                if (checkWorkspace == null)
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.Workspace]} {_localizer[SharedResourceKeys.NotFound]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.NotFound
                    };
                #endregion

                _unitOfWork.WorkspaceRepository.Remove(checkWorkspace);
                var result = _unitOfWork.SaveChangesBool();
                if (!result)
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.SaveChanges]} {_localizer[SharedResourceKeys.Failde]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.InternalServerError
                    };
                var deleteChildTask = _deleteChild.DeleteChildWorkspace(idWorkspace);
                await deleteChildTask;
                _unitOfWork.SaveChanges();

                #region Delete Document Elasticsearch
                var response = await _elasticsearchService.DeleteDocumentAsync(idWorkspace, ElasticsearchKeys.WorkspaceIndex);
                if (!response)
                {
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.DeleteFailde]} {_localizer[SharedResourceKeys.Document]} Elasticsearch",
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
        }
        public async Task<ResultObject> AddUserToWorkspace(ApiUserWorkspace apiUserWorkspace)
        {
            try
            {
                #region Check exists
                var checkUser = _unitOfWork.UserRepository.FirstOrDefault(n => n.Id == apiUserWorkspace.UserId);
                if (checkUser == null)
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.User]} {_localizer[SharedResourceKeys.NotFound]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.NotFound
                    };

                var checkWorkspace = _unitOfWork.WorkspaceRepository.FirstOrDefault(n => n.Id == apiUserWorkspace.WorkspaceId);
                if (checkWorkspace == null)
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.Workspace]} {_localizer[SharedResourceKeys.NotFound]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.NotFound
                    };
                #endregion

                var checkWorkspaceUser = _unitOfWork.WorkspaceUserRepository.FirstOrDefaultIncludeDelete(n => n.WorkspaceId == apiUserWorkspace.WorkspaceId 
                                                                                                         && n.UserId == apiUserWorkspace.UserId);
                if(checkWorkspaceUser != null)
                {
                    _unitOfWork.WorkspaceUserRepository.Restore(checkWorkspaceUser);
                }
                else
                {
                    //User already exists
                    var checkExitst = _unitOfWork.WorkspaceUserRepository
                        .FirstOrDefault(n => n.UserId == checkUser.Id && n.WorkspaceId == checkWorkspace.Id);
                    if (checkExitst != null)
                        return new ResultObject
                        {
                            Message = $"{_localizer[SharedResourceKeys.User]} {_localizer[SharedResourceKeys.AlreadyExists]} " +
                            $"{_localizer[SharedResourceKeys.Workspace]}",
                            Success = true,
                            StatusCode = EnumStatusCodesResult.NotFound
                        };
                    checkWorkspace.WorkspaceUsers = new List<WorkspaceUser>();
                    checkWorkspace.WorkspaceUsers.Add(new WorkspaceUser
                    {
                        UserId = checkUser.Id,
                        WorkspaceId = checkWorkspace.Id
                    });
                    _unitOfWork.WorkspaceRepository.Update(checkWorkspace);
                }
                
                var result = _unitOfWork.SaveChangesBool();
                if (!result)
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.SaveChanges]} {_localizer[SharedResourceKeys.Failde]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.Success
                    };

                #region Update Document Elasticsearch
                var currentDocument = await _elasticsearchService.GetDocumentByIdAsync(checkWorkspace.Id, ElasticsearchKeys.WorkspaceIndex);
                if (currentDocument == null)
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.NotFound]} {_localizer[SharedResourceKeys.Document]} Elasticsearch",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.Success
                    };

                // Update the userIds in the document
                var source = currentDocument as IDictionary<string, object>;
                if (source == null || !source.ContainsKey("userIds"))
                {
                    source = new Dictionary<string, object>();
                }

                var userIds = source.ContainsKey("userIds") ? source["userIds"] as List<object> : new List<object>();
                if (userIds == null)
                {
                    userIds = new List<object>();
                }

                userIds.Add(apiUserWorkspace.UserId.ToString());
                source["userIds"] = userIds;

                var updateResponse = await _elasticsearchService.UpdateDocumentAsync(source, "workspaces", checkWorkspace.Id);
                if (!updateResponse)
                {
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.CreateFailde]} {_localizer[SharedResourceKeys.Document]} Elasticsearch",
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
    }
}
