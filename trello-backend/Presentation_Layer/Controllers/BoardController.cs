using BusinessLogic_Layer.Entity;
using Microsoft.AspNetCore.Mvc;
using BusinessLogic_Layer.Service;
using Microsoft.AspNetCore.Authorization;

namespace Presentation_Layer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BoardController : ControllerBase
    {
        private readonly BoardService _boardService;

        public BoardController(BoardService boardService)
        {
            _boardService = boardService;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAll()
        {
            var respone = await _boardService.GetAll();
            if (respone.Success == false)
                BadRequest(respone);

            return Ok(respone);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetById(Guid idBoard)
        {
            var respone = await _boardService.GetById(idBoard);
            if (respone.Success == false)
                BadRequest(respone);

            return Ok(respone);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllPropertiesFromBoard(Guid idBoard)
        {
            var respone = await _boardService.GetAllPropertiesFromBoard(idBoard);
            if (respone.Success == false)
                BadRequest(respone);

            return Ok(respone);
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> Create([FromBody] ApiBoard apiBoard)
        {
            var respone = await _boardService.Create(apiBoard);
            if (respone.Success == false)
                BadRequest(respone);

            return Ok(respone);
        }

        [HttpPatch("[action]")]
        public async Task<IActionResult> Update([FromBody] ApiBoard apiBoard)
        {
            var respone = await _boardService.Update(apiBoard);
            if (respone.Success == false)
                BadRequest(respone);

            return Ok(respone);
        }

        [HttpDelete("[action]")]
        public async Task<IActionResult> Delete(Guid idBoard)
        {
            var respone = await _boardService.Delete(idBoard);
            if (respone.Success == false)
                BadRequest(respone);

            return Ok(respone);
        }
    }
}
