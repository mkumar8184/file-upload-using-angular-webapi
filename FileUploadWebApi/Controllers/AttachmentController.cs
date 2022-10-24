using AttachmentUtility;
using AttachmentUtility.Models;
using Microsoft.AspNetCore.Mvc;

namespace FileUploadWebApi.Controllers
{

    [Route("attachment")]
    public class AttachmentController : ControllerBase
    {


        private readonly ILogger<AttachmentController> _logger;
        private readonly IAttachmentService _attachmentService;
        public AttachmentController(ILogger<AttachmentController> logger,
            IAttachmentService attachmentService)
        {
            _logger = logger;
            _attachmentService = attachmentService;
        }

        [HttpPost]
        [Route("upload-files")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadFiles(FileCommand command)
        {
            if (command == null)
            {
                return BadRequest("No data");
            }
            var result = await _attachmentService.UploadFile(command);
            return Ok(result);

        }
        [HttpGet]

        [Route("downloads-file/{id}")]
        public IActionResult Download(int id)
        {
            string fileName;
            var stream = _attachmentService.DownloadFile(id, out fileName);
            return File(stream, _attachmentService.GetContentType(fileName), fileName);

        }
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            _attachmentService.DeleleFileById(id);
            return Ok();

        }

    }
}