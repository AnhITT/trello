using BusinessLogic_Layer.Entity;
using Microsoft.AspNetCore.Mvc;
using BusinessLogic_Layer.Service;
using Microsoft.AspNetCore.Authorization;

namespace Presentation_Layer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WorkspaceController : ControllerBase
    {
        private readonly WorkspaceService _workspaceService;
        private readonly ElasticsearchService _elasticsearchService;


        public WorkspaceController(WorkspaceService workspaceService, ElasticsearchService elasticsearchService)
        {
            _workspaceService = workspaceService;
            _elasticsearchService = elasticsearchService;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAll()
        {
            var respone = await _workspaceService.GetAll();
            if (respone.Success == false)
                BadRequest(respone);

            return Ok(respone);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Create([FromBody] ApiWorkspace apiWorkspace)
        {
            var respone = await _workspaceService.Create(apiWorkspace);
            if (respone.Success == false)
                BadRequest(respone);

            return Ok(respone);
        }
        [HttpPatch("[action]")]
        public async Task<IActionResult> Update([FromBody] ApiWorkspace apiWorkspace)
        {
            var respone = await _workspaceService.Update(apiWorkspace);
            if (respone.Success == false)
                BadRequest(respone);

            return Ok(respone);
        }
        [HttpDelete("[action]")]
        public async Task<IActionResult> Delete(Guid idWorkspace)
        {
            var respone = await _workspaceService.Delete(idWorkspace);
            if (respone.Success == false)
                BadRequest(respone);

            return Ok(respone);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> AddUserToWorkspace([FromBody] ApiUserWorkspace apiUserWorkspace)
        {
            var respone = await _workspaceService.AddUserToWorkspace(apiUserWorkspace);
            if (respone.Success == false)
                BadRequest(respone);

            return Ok(respone);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAllProptiesFromWorkspace()
        {
            var respone = await _workspaceService.GetAllProptiesFromWorkspace();
            if (respone.Success == false)
                BadRequest(respone);

            return Ok(respone);
        }
        [HttpGet("[action]")]
        public async Task<IActionResult> SearchInWorkspace(string textSearch, Guid workspaceID)
        {
            var respone = await _elasticsearchService.SearchInWorkspace(textSearch, workspaceID);
            if (respone.Success == false)
                BadRequest(respone);

            return Ok(respone);
        }
    }
}
