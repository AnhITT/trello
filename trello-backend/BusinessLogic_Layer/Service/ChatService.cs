using AutoMapper;
using BusinessLogic_Layer.Entity;
using BusinessLogic_Layer.Enums;
using DataAccess_Layer.Interfaces;
using DataAccess_Layer.Models;
using Microsoft.Extensions.Localization;
using MongoDB.Bson;
using Microsoft.AspNetCore.Http;
using MongoDB.Driver;
using DataAccess_Layer.Helpers;
using BusinessLogic_LayerDataAccess_Layer.Common;

namespace BusinessLogic_Layer.Service
{
    public class ChatService
    {
        private readonly IUnitOfWorkChat _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly CallApi _callApi;
        public ChatService(IUnitOfWorkChat unitOfWork, IMapper mapper, IStringLocalizer<SharedResource> localizer,
            IHttpContextAccessor httpContextAccessor, CallApi callApi)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _localizer = localizer;
            _callApi = callApi;
        }

        public async Task<ResultObject> GetAll()
        {
            try
            {
                var chats = _unitOfWork.ChatRepository.GetAllAsync();
                return new ResultObject
                {
                    Data = chats,
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
        public async Task<ResultObject> GetById(string id)
        {
            try
            {
                var objectId = new ObjectId(id);
                var chat = await _unitOfWork.ChatRepository.GetByIdAsync(objectId);
                if (chat == null)
                {
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.Chat]} {_localizer[SharedResourceKeys.NotFound]}",
                        Success = false,
                        StatusCode = EnumStatusCodesResult.NotFound
                    };
                }
                return new ResultObject
                {
                    Data = chat,
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
        public async Task<ResultObject> GetChatByMembers(List<string> userIds)
        {
            try
            {
                var allChats = await _unitOfWork.ChatRepository.GetAllAsync();

                var chat = allChats.FirstOrDefault(c => c.Members.Count == userIds.Count &&
                                                   !c.Members.Except(userIds).Any());

                if (chat == null)
                {
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.Chat]} {_localizer[SharedResourceKeys.NotFound]}",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.Success
                    };
                }

                return new ResultObject
                {
                    Data = chat,
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
        public async Task<ResultObject> GetChatByMe()
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext?.GetCurrentUserId();

                var filter = Builders<Chat>.Filter.AnyEq(chat => chat.Members, userId.ToString());
                var chats = await _unitOfWork.ChatRepository.Find(filter);

                var sortedChats = chats.OrderByDescending(chat => chat.Messages.Max(msg => msg.CreatedDate)).ToList();

                var userIds = sortedChats
                    .SelectMany(chat => chat.Members)
                    .Distinct()
                    .Where(id => id != userId.ToString())
                    .ToList();

                var userResult = await _callApi.GetUsersByIds(userIds);

                var usersDict = userResult.ToDictionary(user => user.Id, user => user);

                var chatWithUserDetails = sortedChats.Select(chat => new ChatWithUserDetails
                {
                    Id = chat.Id,
                    NameGroup = chat.NameGroup,
                    AvatarGroup = chat.AvatarGroup,
                    IsGroup = chat.IsGroup,
                    Members = chat.Members.Select(memberId =>
                    {
                        if (usersDict.TryGetValue(Guid.Parse(memberId), out var user))
                        {
                            return new MemberWithUserDetails
                            {
                                Id = user.Id.ToString(),
                                FirstName = user.FirstName,
                                LastName = user.LastName,
                                Email = user.Email,
                                PhoneNumber = user.PhoneNumber,
                                VerifyEmail = user.VerifyEmail
                            };
                        }
                        return new MemberWithUserDetails { Id = memberId };
                    }).ToList(),
                    Messages = chat.Messages,
                    CreatedBy = chat.CreatedBy,
                    CreatedDate = chat.CreatedDate,
                    ModifiedBy = chat.ModifiedBy,
                    ModifiedDate = chat.ModifiedDate,
                    DeletedBy = chat.DeletedBy,
                    DeletedDate = chat.DeletedDate,
                    IsDeleted = chat.IsDeleted
                }).ToList();

                return new ResultObject
                {
                    Data = chatWithUserDetails,
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
        public async Task<ResultObject> Create(ApiChat apiChat)
        {
            try
            {
                var chat = _mapper.Map<ApiChat, Chat>(apiChat);
                await _unitOfWork.ChatRepository.AddAsync(chat);
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
        public async Task<ResultObject> Update(ApiMessage apiMessage)
        {
            try
            {
                var chat = await _unitOfWork.ChatRepository.GetByIdAsync(ObjectId.Parse(apiMessage.ChatId));
                if (chat == null)
                {
                    return new ResultObject
                    {
                        Message = "Chat not found",
                        Success = true,
                        StatusCode = EnumStatusCodesResult.NotFound
                    };
                }
                var newMessage = new Message
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Sender = apiMessage.Sender,
                    Type = apiMessage.Type,
                    Text = apiMessage.Text,
                    File = apiMessage.File,
                    CreatedDate = DateTime.UtcNow
                };

                // Thêm Message mới vào danh sách Messages của Chat
                chat.Messages.Add(newMessage);
                await _unitOfWork.ChatRepository.UpdateAsync(ObjectId.Parse(chat.Id), chat);
                return new ResultObject
                {
                    Data = chat,
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
        public async Task<ResultObject> Delete(string id)
        {
            try
            {
                var objectId = new ObjectId(id);
                var existingGroupChat = await _unitOfWork.ChatRepository.GetByIdAsync(objectId);
                if (existingGroupChat == null)
                {
                    return new ResultObject
                    {
                        Message = $"{_localizer[SharedResourceKeys.Chat]} {_localizer[SharedResourceKeys.NotFound]}",
                        Success = false,
                        StatusCode = EnumStatusCodesResult.NotFound
                    };
                }

                _unitOfWork.ChatRepository.DeleteAsync(objectId, existingGroupChat);

                return new ResultObject
                {
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
