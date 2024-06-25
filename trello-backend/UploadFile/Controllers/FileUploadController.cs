using BusinessLogic_Layer.Entity;
using Microsoft.AspNetCore.Mvc;
using BusinessLogic_Layer.Service;
using Microsoft.AspNetCore.Authorization;

namespace UploadFile.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FileUploadController : ControllerBase
    {
        private readonly UploadService _uploadService;

        public FileUploadController(UploadService uploadService)
        {
            _uploadService = uploadService;
        }
        [HttpGet("[action]")]
        public async Task<IActionResult> GetAll()
        {
            var respone = await _uploadService.GetAll();
            if (respone.Success == false)
                BadRequest(respone);

            return Ok(respone);
        }

        [HttpPost("[action]")]
        public async Task<IActionResult> UploadFileToTask([FromForm] ApiFileUpload apiFileUpload)
        {
            var respone = await _uploadService.UploadFileToTask(apiFileUpload);
            if (respone.Success == false)
                BadRequest(respone);

            return Ok(respone);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> DownloadFile(Guid idFile)
        {
            var fileDownload = await _uploadService.DownloadFile(idFile);
            if (fileDownload == null)
                return NotFound();

            return File(fileDownload.FileBytes, "application/octet-stream", fileDownload.FileName);
        }

        [HttpDelete("[action]")]
        public async Task<IActionResult> DeleteFile(Guid idFile)
        {
            var response = await _uploadService.DeleteFile(idFile);
            if (response.Success == false)
                return BadRequest(response);

            return Ok(response);
        }
    }
}
