using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using WopiWebApi.Models;
using WopiWebApi.Models.Wopi;
using WopiWebApi.Utils;

namespace WopiWebApi.Controllers.Wopi
{
    [Route("wopi/files")]
    [ApiController]
    public class FilesController : ControllerBase
    {        
        private readonly FileManager _fileManager;
        private readonly UserManager _userManager;
        private readonly IOptions<WopiConfig> _appSettings;
        private readonly ILogger _logger;

        public FilesController(FileManager fm, UserManager um, IOptions<WopiConfig> app, ILogger<FilesController> logger)
        {            
            _fileManager = fm;
            _userManager = um;
            _appSettings = app;
            _logger = logger;
        }

        //CheckFileInfo
        [HttpGet("{file_id}")]
        public IActionResult Get(string file_id, [FromQuery]string access_token = null)
        {
            _logger.LogDebug("CheckFileInfo");

            WopiRequest wreq = new WopiRequest(file_id, access_token, Request);
            if (!wreq.CheckAccessToken())
                return Unauthorized();

            if (_fileManager.GetFileInfo(file_id, out FileStoreModel fileInfo))
            {
                CheckFileInfoResponse wresp = new CheckFileInfoResponse()
                {
                    BaseFileName = fileInfo.Name,
                    Version = fileInfo.Version.ToString(),
                    OwnerId = fileInfo.OwnerId,
                    UserId = fileInfo.UserId,
                    Size = fileInfo.FileSize,
                    SupportsGetLock = true,
                    SupportsLocks = true,
                    SupportsUpdate = true,
                    IsAnonymousUser = true,
                    UserFriendlyName = _userManager.GetNickName(access_token),
                    UserCanWrite = true,
                    CloseUrl = _appSettings.Value.CloseURL
                };

                return Ok(wresp);
            }
            else
                return NotFound();
        }

        //Lock/Unlock/GetLock/RefreshLock/UnlockAndRelock/PutUserInfo
        [HttpPost("{file_id}")]
        public IActionResult Post(string file_id, [FromQuery]string access_token = null)
        {
            _logger.LogDebug("Post FilesController");

            string user_info = String.Empty;
            string wopiCurrentLock = String.Empty;
            WopiRequest wreq = new WopiRequest(file_id, access_token, Request);
            if (!wreq.CheckAccessToken())
                return Unauthorized();

            _logger.LogDebug(wreq.XWOPIOverride);

            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                _logger.LogDebug("Check user_info");

                _logger.LogDebug(wreq.XWOPIOverride);
                user_info = reader.ReadToEnd();
            }

            _logger.LogDebug($"User info: {user_info}");

            switch (wreq.XWOPIOverride)
            {
                //Lock/UnlockAndRelock
                case WopiAction.LOCK:
                    if (String.IsNullOrEmpty(wreq.XWOPILock))
                    {
                        return BadRequest();
                    }
                    else
                    {
                        if (String.IsNullOrEmpty(wreq.XWOPIOldLock))
                        {
                            if (_fileManager.LockFile(file_id, wreq.XWOPILock, out wopiCurrentLock))
                            {
                                Response.Headers.Add(WopiHeaders.Lock, wopiCurrentLock);
                                if (String.Compare(wreq.XWOPILock, wopiCurrentLock, true) != 0)
                                {                                    
                                    return Conflict();
                                }
                            }
                            else
                                return NotFound();
                        }
                        else
                        {
                            if (_fileManager.UnlockAndRelockFile(file_id, wreq.XWOPILock, wreq.XWOPIOldLock, out wopiCurrentLock))
                            {
                                Response.Headers.Add(WopiHeaders.Lock, wopiCurrentLock);
                                if (String.Compare(wreq.XWOPILock, wopiCurrentLock, true) != 0)
                                {                                    
                                    return Conflict();
                                }
                            }
                            else
                                return NotFound();
                        }
                    }
                    break;

                //GetLock
                case WopiAction.GET_LOCK:
                    if (_fileManager.GetLockFile(file_id, out wopiCurrentLock))
                        Response.Headers.Add(WopiHeaders.Lock, wopiCurrentLock);
                    else
                        return NotFound();
                    break;

                //RefreshLock
                case WopiAction.REFRESH_LOCK:
                    if (String.IsNullOrEmpty(wreq.XWOPILock))
                    {
                        return BadRequest();
                    }
                    else
                    {
                        if (_fileManager.RefreshLockFile(file_id, wreq.XWOPILock, out wopiCurrentLock))
                        {
                            if (String.Compare(wreq.XWOPILock, wopiCurrentLock, true) != 0)
                            {
                                Response.Headers.Add(WopiHeaders.Lock, wopiCurrentLock);
                                return Conflict();
                            }
                        }
                        else
                            return NotFound();
                    }
                    break;

                //Unlock
                case WopiAction.UNLOCK:
                    if (String.IsNullOrEmpty(wreq.XWOPILock))
                    {
                        return BadRequest();
                    }
                    else
                    {
                        if (_fileManager.UnlockFile(file_id, wreq.XWOPILock, out wopiCurrentLock))
                        {
                            if (String.Compare(wreq.XWOPILock, wopiCurrentLock, true) != 0)
                            {
                                Response.Headers.Add(WopiHeaders.Lock, wopiCurrentLock);
                                return Conflict();
                            }
                        }
                        else
                            return NotFound();
                    }
                    break;

                //PutUserInfo
                case WopiAction.PUT_USER_INFO:
                    if (!_fileManager.PutUserInfoFile(file_id, user_info))
                        return NotFound();
                    break;

                //PutRelativeFile
                case WopiAction.PUT_RELATIVE:
                    return StatusCode(StatusCodes.Status501NotImplemented);

                default:
                    return StatusCode(StatusCodes.Status501NotImplemented);                    
            }

            return Ok();
        }        
    }
}