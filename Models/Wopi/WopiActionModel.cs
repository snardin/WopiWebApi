using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WopiWebApi.Models
{
    /// <summary>
    /// Wopi app info
    /// </summary>
    public class WopiAppModel
    {
        /// <summary>
        /// App name
        /// </summary>
        public string app { get; set; }

        /// <summary>
        /// App favIconUrl
        /// </summary>
        public string favIconUrl { get; set; }

        /// <summary>
        ///App check license
        /// </summary>
        public bool checkLicense { get; set; }

        /// <summary>
        /// Appp actions list
        /// </summary>
        public List<WopiActionModel> actions { get; set; }
    }

    /// <summary>
    /// Wopi Action info
    /// </summary>
    public class WopiActionModel
    {
        /// <summary>
        /// Action name
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Action extension
        /// </summary>
        public string ext { get; set; }

        /// <summary>
        /// Action identifier
        /// </summary>
        public string progid { get; set; }

        /// <summary>
        /// Action requirements
        /// </summary>
        public string requires { get; set; }

        /// <summary>
        /// Action default
        /// </summary>
        public bool? isDefault { get; set; }

        /// <summary>
        /// Action url
        /// </summary>
        public string urlsrc { get; set; }
    }
}
