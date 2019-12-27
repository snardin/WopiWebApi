using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WopiWebApi.Models;
using WopiWebApi.Utils;

namespace WopiWebApi.Controllers
{
    /// <summary>
    /// This controller retrieves the base informations
    /// </summary>    
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly WopiDiscovery _wopiDiscovery;
        private readonly FileManager _fileManager;
        private readonly ILogger _logger;

        public HomeController(WopiDiscovery wd, FileManager fm, ILogger<HomeController> logger)
        {
            _wopiDiscovery = wd;
            _fileManager = fm;
            _logger = logger;
        }

        private string GetHostPath()
        {
            string strPath = Request.Scheme + "://" + Request.Host.Value + Request.PathBase;
            return strPath;
        }

        /// <summary>
        /// Gets a list of files info
        /// </summary>
        [HttpGet("GetFiles")]
        [Produces(typeof(List<FileInfoModel>))]
        public IActionResult GetFiles()
        {
            _logger.LogDebug("GetFiles");

            return Ok(_fileManager.GetFilesInfo());
        }

        /// <summary>
        /// Gets a list of actions info
        /// </summary>
        [HttpGet("GetActions")]
        [Produces(typeof(List<WopiAppModel>))]
        public IActionResult GetActions()
        {
            _logger.LogDebug("GetActions");

            return Ok(_wopiDiscovery.GetXmlActions(GetHostPath()));
        }

        /// <summary>
        /// Gets a list of actions info from a specific extension
        /// </summary>
        [HttpGet("GetActions/{actionExt}")]
        [Produces(typeof(List<WopiAppModel>))]
        public IActionResult GetActions(string actionExt)
        {
            _logger.LogDebug($"GetActions {actionExt}");

            return Ok(_wopiDiscovery.GetXmlActions(GetHostPath(), actionExt));
        }

        /// <summary>
        /// Gets an actions info from a specific extension and action name
        /// </summary>
        [HttpGet("GetAction/{actionExt}/{actionName}")]
        [Produces(typeof(List<WopiAppModel>))]
        public IActionResult GetAction(string actionExt, string actionName)
        {
            _logger.LogDebug($"GetAction {actionExt} {actionName}");

            return Ok(_wopiDiscovery.GetXmlAction(GetHostPath(), actionExt, actionName));
        }
    }
}
