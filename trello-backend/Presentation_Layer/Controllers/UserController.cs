using BusinessLogic_Layer.Model;
using BusinessLogic_Layer.Entity;
using Microsoft.AspNetCore.Mvc;
using BusinessLogic_Layer.Service;
using Microsoft.AspNetCore.Authorization;

namespace Presentation_Layer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetAll()
        {
            var respone = await _userService.GetAll();
            if (respone.Success == false)
                BadRequest(respone);

            return Ok(respone);
        }

        [HttpGet("[action]")]
        public async Task<ApiPageList<ApiUser>> GetPage([FromQuery] ApiPageModel apiPageModel)
        {
            return await _userService.GetPage(apiPageModel);
        }

        [HttpPatch("[action]")]
        public async Task<IActionResult> Update([FromBody] ApiUser apiUser)
        {
            var respone = await _userService.Update(apiUser);
            if (respone.Success == false)
                BadRequest(respone);

            return Ok(respone);
        }

        [HttpDelete("[action]")]
        public async Task<IActionResult> Delete(Guid idUser)
        {
            var respone = await _userService.Delete(idUser);
            if (respone.Success == false)
                BadRequest(respone);

            return Ok(respone);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> GetUsersByIds([FromBody] List<string> userIds)
        {
            var respone = await _userService.GetUsersByIds(userIds);
            if (respone.Success == false)
                BadRequest(respone);

            return Ok(respone);
        }
    }
}
