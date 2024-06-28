using BusinessLogic_Layer.Entity;
using Microsoft.AspNetCore.Mvc;
using BusinessLogic_Layer.Service;
using Microsoft.AspNetCore.Authorization;

namespace Presentation_Layer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class TaskCardController : ControllerBase
    {
        private readonly TaskCardService _taskCardService;

        public TaskCardController(TaskCardService taskCardService)
        {
            _taskCardService = taskCardService;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAll()
        {
            var respone = await _taskCardService.GetAll();
            if (respone.Success == false)
                BadRequest(respone);

            return Ok(respone);
        }
      
        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllCheckList()
        {
            var respone = await _taskCardService.GetAllCheckList();
            if (respone.Success == false)
                BadRequest(respone);

            return Ok(respone);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllCheckListItem()
        {
            var respone = await _taskCardService.GetAllCheckListItem();
            if (respone.Success == false)
                BadRequest(respone);

            return Ok(respone);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Create([FromBody] ApiTaskCard apiTaskCard)
        {
            var respone = await _taskCardService.Create(apiTaskCard);
            if (respone.Success == false)
                BadRequest(respone);

            return Ok(respone);
        }

        [HttpPatch("[action]")]
        public async Task<IActionResult> Update([FromBody] ApiTaskCard apiTaskCard)
        {
            var respone = await _taskCardService.Update(apiTaskCard);
            if (respone.Success == false)
                BadRequest(respone);

            return Ok(respone);
        }

        [HttpDelete("[action]")]
        public async Task<IActionResult> Delete(Guid idTaskCard)
        {
            var respone = await _taskCardService.Delete(idTaskCard);
            if (respone.Success == false)
                BadRequest(respone);

            return Ok(respone);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> AddUserToTask([FromBody] ApiUserTask apiUserTask)
        {
            var respone = await _taskCardService.AddUserToTask(apiUserTask);
            if (respone.Success == false)
                BadRequest(respone);

            return Ok(respone);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> AddCheckListToTask([FromBody] ApiCheckList apiCheckList)
        {
            var respone = await _taskCardService.AddCheckListToTask(apiCheckList);
            if (respone.Success == false)
                BadRequest(respone);

            return Ok(respone);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> AddItemToCheckList([FromBody] ApiCheckListItem apiCheckListItem)
        {
            var respone = await _taskCardService.AddItemToCheckList(apiCheckListItem);
            if (respone.Success == false)
                BadRequest(respone);

            return Ok(respone);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> CheckTaskCard(Guid idTask)
        {
            var respone = await _taskCardService.CheckTaskCard(idTask);

            return Ok(respone);
        }

        [HttpDelete("[action]")]
        public async Task<IActionResult> DeleteCheckList(Guid idCheckList)
        {
            var respone = await _taskCardService.DeleteCheckList(idCheckList);
            if (respone.Success == false)
                BadRequest(respone);

            return Ok(respone);
        }

        [HttpDelete("[action]")]
        public async Task<IActionResult> DeleteCheckListItem(Guid idCheckListItem)
        {
            var respone = await _taskCardService.DeleteCheckListItem(idCheckListItem);
            if (respone.Success == false)
                BadRequest(respone);

            return Ok(respone);
        }
        [HttpPost("[action]")]
        public async Task<IActionResult> UpdateTaskCardPosition([FromBody] UpdatePositionRequest request)
        {
            var respone = await _taskCardService.UpdateTaskCardPosition(request);
            if (respone.Success == false)
                BadRequest(respone);

            return Ok(respone);
        }
    }
}
