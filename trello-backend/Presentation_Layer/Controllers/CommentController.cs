using BusinessLogic_Layer.Entity;
using BusinessLogic_Layer.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation_Layer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CommentController : ControllerBase
    {
        private readonly CommentService _commentService;

        public CommentController(CommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> CreateComment([FromBody] ApiComment apiComment)
        {
            var respone = await _commentService.CreateComment(apiComment);
            if (respone.Success == false)
                BadRequest(respone);

            return Ok(respone);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> CreateCommentReply([FromBody] ApiComment apiComment)
        {
            var respone = await _commentService.CreateCommentReply(apiComment);
            if (respone.Success == false)
                BadRequest(respone);

            return Ok(respone);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllCommentFromTask(Guid TaskId)
        {
            var respone = await _commentService.GetAllCommentFromTask(TaskId);
            if (respone.Success == false)
                BadRequest(respone);

            return Ok(respone);
        }

        [HttpDelete("[action]")]
        public async Task<IActionResult> Delete(Guid idComment)
        {
            var respone = await _commentService.Delete(idComment);
            if (respone.Success == false)
                BadRequest(respone);

            return Ok(respone);
        }
    }
}
