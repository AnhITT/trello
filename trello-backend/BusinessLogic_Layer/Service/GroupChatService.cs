using AutoMapper;
using BusinessLogic_Layer.Common;
using BusinessLogic_Layer.Entity;
using BusinessLogic_Layer.Enums;
using DataAccess_Layer.DTOs;
using DataAccess_Layer.Interfaces;
using DataAccess_Layer.Models;
using Microsoft.Extensions.Localization;
using MongoDB.Bson;

namespace BusinessLogic_Layer.Service
{
    public class GroupChatService
    {
        private readonly IUnitOfWorkChat _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public GroupChatService(IUnitOfWorkChat unitOfWork, IMapper mapper, IStringLocalizer<SharedResource> localizer)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _localizer = localizer;
        }

        public async Task CreateGroupChatAsync(GroupChat groupChat)
        {
            try
            {
                await _unitOfWork.GroupChatRepository.AddAsync(groupChat);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Handle exception
            }
        }

        public async Task<ResultObject> GetAll()
        {
            try
            {
                var data = _unitOfWork.GroupChatRepository.GetAllAsync();
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
                var data = await _unitOfWork.GroupChatRepository.GetByIdAsync(objectId);
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

        public async Task<ResultObject> Update(GroupChat groupChat)
        {
            try
            {
                var objectId = new ObjectId(groupChat.Id);
                var existingGroupChat = await _unitOfWork.GroupChatRepository.GetByIdAsync(objectId);
                if (existingGroupChat == null)
                {
                    return new ResultObject
                    {
                        Message = $"{_localizer["GroupChat"]} {_localizer["NotFound"]}",
                        Success = false,
                        StatusCode = EnumStatusCodesResult.NotFound
                    };
                }

                _mapper.Map(groupChat, existingGroupChat);
                _unitOfWork.GroupChatRepository.UpdateAsync(objectId, existingGroupChat);
                await _unitOfWork.SaveChangesAsync();

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

        public async Task<ResultObject> Delete(string id)
        {
            try
            {
                var objectId = new ObjectId(id);
                var existingGroupChat = await _unitOfWork.GroupChatRepository.GetByIdAsync(objectId);
                if (existingGroupChat == null)
                {
                    return new ResultObject
                    {
                        Message = $"{_localizer["GroupChat"]} {_localizer["NotFound"]}",
                        Success = false,
                        StatusCode = EnumStatusCodesResult.NotFound
                    };
                }

                _unitOfWork.GroupChatRepository.DeleteAsync(objectId);
                await _unitOfWork.SaveChangesAsync();

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
