using BusinessLogic_Layer.Entity;
using BusinessLogic_Layer.Service;
using Microsoft.AspNetCore.Mvc;

namespace Presentation_Layer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            var response = await _authService.Login(loginRequest);
            if (!response.Success)
                return BadRequest(response); 

            return Ok(response); 
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> Register([FromBody] ApiUser registerRequest)
        {
            var response = await _authService.Register(registerRequest);
            if (!response.Success)
                return BadRequest(response); 

            return Ok(response);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> VerifyEmail(string tokenEncode)
        {
            var response = await _authService.VerifyEmail(tokenEncode);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var response = await _authService.ForgotPassword(email);
            if (!response.Success)
                return BadRequest(response); 

            return Ok(response); 
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> ConfirmOTPChangePassword([FromBody] PasswordConfirm passwordReset)
        {
            var response = await _authService.CornfirmOTPChangePassword(passwordReset);
            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }
    }
}
