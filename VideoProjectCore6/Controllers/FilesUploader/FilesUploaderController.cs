using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VideoProjectCore6.DTOs.CommonDto;
using VideoProjectCore6.DTOs.FileDto;
using VideoProjectCore6.Repositories.IFilesUploader;
using VideoProjectCore6.Repositories.IFileRepository;


namespace VideoProjectCore6.Controllers.FilesUploader
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class FilesUploaderController : Controller
    {
        private readonly IFilesUploaderRepository _IFilesUploaderRepositiory;
        private readonly IFileRepository _IFileRepository;

        public FilesUploaderController(IFilesUploaderRepository iFilesUploaderRepository, IFileRepository fileRepository)
        {
            _IFilesUploaderRepositiory = iFilesUploaderRepository;
            _IFileRepository = fileRepository;

        }
        //-------------------------------Upload-----------------------------------
        /*[HttpPost("{target}")]
               public async Task<ActionResult> Upload([FromForm] IFormFile file, [FromRoute] string target)
               {
                   string lang = Request.Headers["lang"].ToString().ToLower();
                   Response.Headers.Add("lang", lang);
       UploadedFileMessage m = await _IFilesUploaderRepositiory.UploadFile(file, target);
                   if (m.SuccessUpload == true)
                       return  Ok(m);
                   else 
                       return BadRequest(m);

                   }*/
        /* [HttpGet("{folder}")]
         public IActionResult DownloadFile([FromRoute] string folder, [FromQuery] string file)
         {
             string lang = Request.Headers["lang"].ToString().ToLower();
             Response.Headers.Add("lang", lang);

             var result = _IFilesUploaderRepositiory.ReadFile(folder, file);

             if (result != null)
                 return Ok(result);
             else
                 return BadRequest();
         }*/
        
        //-----------------------------------
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("{type}")]
        public async Task<IActionResult> downloadAsync(string type, [FromQuery] string filePath)
        {
            if (!_IFilesUploaderRepositiory.FileExist(filePath))
            {
                return NotFound("Missing File!");
            }
            var path = _IFilesUploaderRepositiory.GetFilePath(filePath);
            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            if (type == "view")
                return File(memory, _IFilesUploaderRepositiory.GetMimeType(Path.GetExtension(filePath)));
            if (type == "download")
                return File(memory, _IFilesUploaderRepositiory.GetMimeType(Path.GetExtension(filePath)), filePath);
            return NotFound();
        }

        [HttpGet("GetFile/{file}")]
        public IActionResult Get(string file)
        {
            return PhysicalFile(_IFilesUploaderRepositiory.GetFilePath(file), _IFilesUploaderRepositiory.GetMimeType(file));
        }

        [HttpPost("createFolder")]
        public IActionResult createFolder(string folder)
        {
            var m = _IFilesUploaderRepositiory.CreateFolder(folder);
            return Ok(m);
        }

        [HttpPost("moveFile")]
        public IActionResult moveFile(string s, string t)
        {
            var m = _IFilesUploaderRepositiory.MoveFile(s, t);
            return Ok(m);
        }

        [HttpGet("GetFileNames/{folder}")]
        public IActionResult GetFileNames(string folder)
        {
            return Ok(_IFilesUploaderRepositiory.GetFolderFilesNames(folder));
        }

        [HttpGet("GetLogsFilesNames")]
        public IActionResult GetLogsFilesNames(string search)
        {
            return Ok(_IFilesUploaderRepositiory.GetLogsFilesNames(search));
        }



        [HttpPost("DeleteTemporaryFiles")]
        public IActionResult DeleteTemporaryFiles(DateTime dateTime)
        {
            return Ok(_IFilesUploaderRepositiory.DeleteTemporaryFiles(dateTime));
        }


        [HttpGet("DownloadDocument")]
        public IActionResult DownloadDocument(string fileName)
        {
            var path = Path.Combine("Logs", fileName);
            byte[] fileBytes = System.IO.File.ReadAllBytes(path);
            return File(fileBytes, "application/force-download", fileName);
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("Upload")]
        public async Task<ActionResult> UploadFile(IFormFile file)
        {
            UploadedFileMessage m = await _IFilesUploaderRepositiory.UploadFileToTemp(file);
            return m.SuccessUpload ? Ok(m) : BadRequest(m);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("Download")]
        public async Task<IActionResult> download([FromQuery] string filePath)
        {
            if (!_IFilesUploaderRepositiory.FileExist(filePath))
            {
                return NotFound("Missing File!");
            }
            var path = _IFilesUploaderRepositiory.GetFilePath(filePath);
            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, _IFilesUploaderRepositiory.GetMimeType(Path.GetExtension(filePath)), filePath);
        }
        
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("View")]
        public async Task<IActionResult> view([FromQuery] string filePath)
        {
            if (!_IFilesUploaderRepositiory.FileExist(filePath))
            {
                return NotFound("Missing File!");
            }
            var path = _IFilesUploaderRepositiory.GetFilePath(filePath);
            var memory = new MemoryStream();
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, _IFilesUploaderRepositiory.GetMimeType(Path.GetExtension(filePath)));
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("ViewFile")]
        public async Task<FileContentResult> ViewFile(int fileId)
        {
            var res = await _IFileRepository.Download(fileId);

            return File(res.Key, res.Value);
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost("DeleteFile")]
        public async Task<IActionResult> DeleteFile(int fileId)
        {
            var res = await _IFileRepository.Delete(fileId);

            return Ok(res);
        }
    }
}
