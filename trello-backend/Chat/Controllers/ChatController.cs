using BusinessLogic_Layer.Entity;
using BusinessLogic_Layer.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly ChatService _chatService;

        public ChatController(ChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAll()
        {
            var response = await _chatService.GetAll();
            if (response.Success == false)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var response = await _chatService.GetById(id);
            if (response.Success == false)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Create([FromBody] ApiChat apiChat)
        {
            var respone = await _chatService.CreateChatAsync(apiChat);
            if (respone.Success == false)
                BadRequest(respone);

            return Ok(respone);
        }

        [HttpPatch("[action]")]
        public async Task<IActionResult> Update([FromBody] ApiMessage apiMessage)
        {
            var response = await _chatService.Update(apiMessage);
            if (response.Success == false)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var response = await _chatService.Delete(id);
            if (response.Success == false)
                return BadRequest(response);

            return Ok(response);
        }
        [HttpGet("GetChatByMembers")]
        public async Task<IActionResult> GetChatByMembers(string idUser1, string idUser2)
        {
            var result = await _chatService.GetChatByMembers(idUser1, idUser2);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
