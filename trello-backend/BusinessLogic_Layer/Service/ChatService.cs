using AutoMapper;
using DataAccess_Layer.Common;
using BusinessLogic_Layer.Entity;
using BusinessLogic_Layer.Enums;
using DataAccess_Layer.Interfaces;
using DataAccess_Layer.Models;
using Microsoft.Extensions.Localization;
using MongoDB.Bson;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using Nest;

namespace BusinessLogic_Layer.Service
{
    public class ChatService
    {
        private readonly IUnitOfWorkChat _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IStringLocalizer<SharedResource> _localizer;
        public ChatService(IUnitOfWorkChat unitOfWork, IMapper mapper, IStringLocalizer<SharedResource> localizer)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _contextAccessor = new HttpContextAccessor();
            _localizer = localizer;
        }

        public async Task<ResultObject> CreateChatAsync(ApiChat apiChat)
        {
            try
            {
                var mapApiChat = _mapper.Map<ApiChat, Chat>(apiChat);
                await _unitOfWork.ChatRepository.AddAsync(mapApiChat);
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

        public async Task<ResultObject> GetAll()
        {
            try
            {
                var data = _unitOfWork.ChatRepository.GetAllAsync();
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
        }

        public async Task<ResultObject> GetById(string id)
        {
            try
            {
                var objectId = new ObjectId(id);
                var data = await _unitOfWork.ChatRepository.GetByIdAsync(objectId);
                if (data == null)
                {
                    return new ResultObject
                    {
                        Message = $"{_localizer["GroupChat"]} {_localizer["NotFound"]}",
                        Success = false,
                        StatusCode = EnumStatusCodesResult.NotFound
                    };
                }
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
                        Message = $"{_localizer["GroupChat"]} {_localizer["NotFound"]}",
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
                        Message = $"{_localizer["GroupChat"]} {_localizer["NotFound"]}",
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
    }
}
