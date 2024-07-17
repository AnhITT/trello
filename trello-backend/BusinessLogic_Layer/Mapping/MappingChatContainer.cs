using AutoMapper;
using BusinessLogic_Layer.Entity;
using DataAccess_Layer.Models;

namespace BusinessLogic_Layer.Mapping
{
    public class MappingUploadContainer : Profile
    {
        public MappingUploadContainer()
        {
            CreateMap<ApiChat, Chat>().ReverseMap();
            CreateMap<ApiMessage, Message>().ReverseMap();
        }
    }
}
