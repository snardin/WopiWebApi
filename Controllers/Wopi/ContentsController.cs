using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using WopiWebApi.Models;
using WopiWebApi.Models.Wopi;
using WopiWebApi.Utils;

namespace WopiWebApi.Controllers.Wopi
{
    [Route("wopi/files")]
    [ApiController]
    public class ContentsController : ControllerBase
    {
        private readonly FileManager _fileManager;
        private readonly ILogger _logger;

        public ContentsController(FileManager fm, ILogger<ContentsController> logger)
        {
            _fileManager = fm;
            _logger = logger;
        }

        //GetFile
        [HttpGet("{file_id}/contents")]
        public IActionResult Get(string file_id, [FromQuery]string access_token = null)
        {
            _logger.LogDebug("GetFile");

            WopiRequest wreq = new WopiRequest(file_id, access_token, Request);
            if (!wreq.CheckAccessToken())
                return Unauthorized();

            if (_fileManager.GetFile(file_id, out byte[] fileContent) && fileContent != null)
            {
                if (fileContent.Length > wreq.XWOPIMaxExpectedSize)
                    return StatusCode(StatusCodes.Status412PreconditionFailed);
            }
            else
                return NotFound();

            return File(fileContent, "application/octet-stream");
        }

        //PutFile
        [HttpPost("{file_id}/contents")]
        public IActionResult Put(string file_id, [FromQuery]string access_token = null)
        {
            _logger.LogDebug("PutFile");

            WopiRequest wreq = new WopiRequest(file_id, access_token, Request);
            if (!wreq.CheckAccessToken())
                return Unauthorized();

            byte[] file_content = null;
            using (MemoryStream ms = new MemoryStream())
            {
                Request.Body.CopyTo(ms);

                file_content = ms.ToArray(); 
                ms.Close();
            }
            
            if (file_content == null)
                return BadRequest();

            if (file_content != null && file_content.Length > wreq.XWOPIMaxExpectedSize)
                return StatusCode(StatusCodes.Status413RequestEntityTooLarge);

            if (_fileManager.GetLockFile(file_id, out string wopiCurrentLock))
            {
                if (String.IsNullOrEmpty(wopiCurrentLock))
                {
                    _fileManager.GetFileInfo(file_id, out FileStoreModel fileInfo);
                    if (fileInfo.FileSize > 0)
                        return Conflict();
                }

                if (String.Compare(wopiCurrentLock, wreq.XWOPILock, true) == 0)
                    _fileManager.PutFile(file_id, file_content);
                else
                {
                    Response.Headers.Add(WopiHeaders.Lock, wopiCurrentLock);
                    return Conflict();
                }
            }
            else
                return NotFound();

            return Ok();
        }
       
    }
}