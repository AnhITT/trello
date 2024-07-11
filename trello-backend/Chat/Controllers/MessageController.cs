using BusinessLogic_Layer.Service;
using DataAccess_Layer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Chat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly MessageService _chatService;

        public MessageController(MessageService chatService)
        {
            _chatService = chatService;
        }
    }
}
