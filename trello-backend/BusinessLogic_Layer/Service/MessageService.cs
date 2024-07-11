using AutoMapper;
using DataAccess_Layer.Interfaces;
using Microsoft.Extensions.Localization;

namespace BusinessLogic_Layer.Service
{
    public class MessageService
    {
        private readonly IUnitOfWorkChat _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<SharedResource> _localizer;
        public MessageService(IUnitOfWorkChat unitOfWork, IMapper mapper, IStringLocalizer<SharedResource> localizer)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _localizer = localizer;
        }

    }
}
