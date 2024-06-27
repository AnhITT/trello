using BusinessLogic_Layer.Entity;
using Microsoft.AspNetCore.Mvc;
using BusinessLogic_Layer.Service;
using Microsoft.AspNetCore.Authorization;
using DataAccess_Layer.Models;

namespace Presentation_Layer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class WorkflowController : ControllerBase
    {
        private readonly WorkflowService _workflowService;

        public WorkflowController(WorkflowService workflowService)
        {
            _workflowService = workflowService;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAll()
        {
            var respone = await _workflowService.GetAll();
            if (respone.Success == false)
                BadRequest(respone);

            return Ok(respone);
        }
        [HttpGet("[action]")]
        public async Task<IActionResult> GetByBoardID(Guid boardId)
        {
            var respone = await _workflowService.GetByBoardID(boardId);
            if (respone.Success == false)
                BadRequest(respone);

            return Ok(respone);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> UpdateWorkflowPosition([FromBody] UpdateWorkflowPositionRequest request)
        {
            var respone = await _workflowService.UpdateWorkflowPosition(request);
            if (respone.Success == false)
                BadRequest(respone);

            return Ok(respone);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Create([FromBody] ApiWorkflow apiWorkflow)
        {
            var respone = await _workflowService.Create(apiWorkflow);
            if (respone.Success == false)
                BadRequest(respone);

            return Ok(respone);
        }

        [HttpPatch("[action]")]
        public async Task<IActionResult> Update([FromBody] ApiWorkflow apiWorkflow)
        {
            var respone = await _workflowService.Update(apiWorkflow);
            if (respone.Success == false)
                BadRequest(respone);

            return Ok(respone);
        }

        [HttpDelete("[action]")]
        public async Task<IActionResult> Delete(Guid idWorkflow)
        {
            var respone = await _workflowService.Delete(idWorkflow);
            if (respone.Success == false)
                BadRequest(respone);

            return Ok(respone);
        }
    }
}
