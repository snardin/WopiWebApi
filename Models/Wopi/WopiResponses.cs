using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WopiWebApi.Models.Wopi
{
    public class CheckFileInfoResponse
    {
        public string BaseFileName { get; set; }
        public string OwnerId { get; set; }
        public long Size { get; set; }
        public string UserId { get; set; }
        public string Version { get; set; }        
        public bool SupportsGetLock { get; set; }
        public bool SupportsLocks { get; set; }
        public bool SupportsUpdate { get; set; }        
        public bool IsAnonymousUser { get; set; }
        public string UserFriendlyName { get; set; }
        public bool UserCanWrite { get; set; }
        public string CloseUrl { get; set; }
    }
}
