using AutoMapper;
using BusinessLogic_Layer.Entity;
using DataAccess_Layer.Models;

namespace BusinessLogic_Layer.Mapping
{
    public class MappingContainer : Profile
    {
        public MappingContainer()
        {
            CreateMap<ApiUser, User>().ReverseMap();
            CreateMap<ApiWorkspace, Workspace>().ReverseMap();
            CreateMap<ApiBoard, Board>().ReverseMap();
            CreateMap<ApiWorkflow, Workflow>().ReverseMap();
            CreateMap<ApiTaskCard, TaskCard>().ReverseMap();
            CreateMap<ApiUserTask, TaskCardUser>().ReverseMap();
            CreateMap<ApiUserWorkspace, WorkspaceUser>().ReverseMap();
            CreateMap<ApiCheckList, CheckList>().ReverseMap();
            CreateMap<ApiCheckListItem, CheckListItem>().ReverseMap();
            CreateMap<ApiComment, Comment>().ReverseMap();
            CreateMap<Comment, ApiComment>().ReverseMap();
        }
    }
}
