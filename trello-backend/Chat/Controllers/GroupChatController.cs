using BusinessLogic_Layer.Service;
using DataAccess_Layer.Models;
using Microsoft.AspNetCore.Mvc;

namespace Chat.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupChatController : ControllerBase
    {
        private readonly GroupChatService _groupChatService;

        public GroupChatController(GroupChatService groupChatService)
        {
            _groupChatService = groupChatService;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAll()
        {
            var response = await _groupChatService.GetAll();
            if (response.Success == false)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpGet("[action]/{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var response = await _groupChatService.GetById(id);
            if (response.Success == false)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Create([FromBody] GroupChat groupChat)
        {
            await _groupChatService.CreateGroupChatAsync(groupChat);
            return Ok(new { Success = true });
        }

        [HttpPut("[action]")]
        public async Task<IActionResult> Update([FromBody] GroupChat groupChat)
        {
            var response = await _groupChatService.Update(groupChat);
            if (response.Success == false)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var response = await _groupChatService.Delete(id);
            if (response.Success == false)
                return BadRequest(response);

            return Ok(response);
        }
    }
}
