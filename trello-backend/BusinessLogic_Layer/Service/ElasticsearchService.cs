using BusinessLogic_Layer.Entity;
using BusinessLogic_Layer.Enums;
using Microsoft.Extensions.Localization;
using Nest;

namespace BusinessLogic_Layer.Service
{
    public class ElasticsearchService
    {
        private readonly IElasticClient _elasticsearchClient;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public ElasticsearchService(IElasticClient elasticsearchClient, IStringLocalizer<SharedResource> localizer)
        {
            _elasticsearchClient = elasticsearchClient;
            _localizer = localizer;
        }

        #region CRUD Document Add Elasticsearch
        public async Task<bool> CreateDocumentAsync(object document, string indexes)
        {
            try
            {
                var response = await _elasticsearchClient.IndexAsync(document, i => i.Index(indexes));
                return response.IsValid;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> UpdateDocumentAsync(object document, string index, Guid id)
        {
            try
            {
                var response = await _elasticsearchClient.UpdateAsync<object>(id, u => u
                    .Index(index)
                    .Doc(document)
                );
                return response.IsValid;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> DeleteDocumentAsync(Guid id, string index)
        {
            try
            {
                var response = await _elasticsearchClient.DeleteAsync<object>(id, d => d.Index(index));
                return response.IsValid;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<bool> DeleteDocumentRangeAsync(List<Guid> ids, string index)
        {
            try
            {
                var bulkResponse = await _elasticsearchClient.BulkAsync(b => b
                    .Index(index)
                    .DeleteMany<object>(ids.Select(id => new { Id = id }))
                );

                if (bulkResponse.Errors)
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async Task<object> GetDocumentByIdAsync(Guid id, string index)
        {
            try
            {
                var response = await _elasticsearchClient.GetAsync<object>(id, g => g.Index(index));
                if (response.Found)
                {
                    return response.Source;
                }
                else
                {
                    return null;
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
        }
        #endregion

        #region Search with Elasticsearch
        public async Task<ResultObject> SearchInWorkspace(string textSearch, Guid workspaceID)
        {
            try
            {
                var boardIds = await GetBoardIdsFromElasticsearch(workspaceID);
                var workflowIds = await GetWorkflowIdsFromElasticsearch(boardIds);
                var userIds = await GetUserIdsFromWorkspace(workspaceID);

                var searchResponse = await _elasticsearchClient.SearchAsync<object>(s => s
                    .Index(new[] { "workspaces", "users" }) 
                    .Query(q => q
                        .Bool(b => b
                            .Must(mu => mu
                                .Bool(b1 => b1
                                    .Should(
                                        sh => sh
                                            .MultiMatch(mm => mm
                                                .Fields(new[] { "title", "name", "firstName", "lastName", "email", "phoneNumber" })
                                                .Query(textSearch)
                                            )
                                    )
                                )
                            )
                            .Filter(f => f
                                .Bool(b2 => b2
                                    .Should(
                                        sh => sh
                                            .Term(t => t
                                                .Field("workspaceId.keyword")
                                                .Value(workspaceID)
                                            ),
                                        sh => sh
                                            .Terms(t => t
                                                .Field("boardId.keyword")
                                                .Terms(boardIds)
                                            ),
                                        sh => sh
                                            .Terms(t => t
                                                .Field("workflowId.keyword")
                                                .Terms(workflowIds)
                                            ),
                                        sh => sh
                                            .Terms(t => t
                                                .Field("id.keyword")
                                                .Terms(userIds)
                                            )
                                    )
                                )
                            )
                        )
                    )
                );

                var hits = searchResponse.Hits;

                return new ResultObject
                {
                    Data = hits.Select(hit => hit.Source).ToList(),
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
        private async Task<IEnumerable<string>> GetBoardIdsFromElasticsearch(Guid workspaceID)
        {
            var searchResponse = await _elasticsearchClient.SearchAsync<object>(s => s
                .Index("workspaces")
                .Query(q => q
                    .Bool(b => b
                        .Must(mu => mu
                            .Term(t => t
                                .Field("workspaceId.keyword")
                                .Value(workspaceID)
                            )
                        )
                        .Filter(f => f
                            .Term(t => t
                                .Field("type.keyword")
                                .Value("Board")
                            )
                        )
                    )
                )
                .Source(src => src
                    .Includes(i => i
                        .Field("id")
                    )
                )
            );

            return searchResponse.Hits.Select(hit => hit.Id).ToList();
        }
        private async Task<IEnumerable<string>> GetWorkflowIdsFromElasticsearch(IEnumerable<string> boardIds)
        {
            var searchResponse = await _elasticsearchClient.SearchAsync<object>(s => s
                .Index("workspaces")
                .Query(q => q
                    .Bool(b => b
                        .Must(mu => mu
                            .Terms(t => t
                                .Field("boardId.keyword")
                                .Terms(boardIds)
                            )
                        )
                        .Filter(f => f
                            .Term(t => t
                                .Field("type.keyword")
                                .Value("Workflow")
                            )
                        )
                    )
                )
                .Source(src => src
                    .Includes(i => i
                        .Field("id")
                    )
                )
            );

            return searchResponse.Hits.Select(hit => hit.Id).ToList();
        }
        private async Task<IEnumerable<string>> GetUserIdsFromWorkspace(Guid workspaceID)
        {
            var searchResponse = await _elasticsearchClient.SearchAsync<object>(s => s
                .Index("workspaces")
                .Query(q => q
                    .Bool(b => b
                        .Must(mu => mu
                            .Term(t => t
                                .Field("id.keyword")
                                .Value(workspaceID)
                            )
                        )
                        .Filter(f => f
                            .Term(t => t
                                .Field("type.keyword")
                                .Value("Workspace")
                            )
                        )
                    )
                )
                .Source(src => src
                    .Includes(i => i
                        .Field("userIds")
                    )
                )
            );

            var userIds = new List<string>();
            foreach (var hit in searchResponse.Hits)
            {
                var source = hit.Source as IDictionary<string, object>;
                if (source != null && source.ContainsKey("userIds"))
                {
                    var ids = source["userIds"] as IEnumerable<object>;
                    if (ids != null)
                    {
                        userIds.AddRange(ids.Select(id => id.ToString()));
                    }
                }
            }

            return userIds;
        }
        #endregion

    }
}
