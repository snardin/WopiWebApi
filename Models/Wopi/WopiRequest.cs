using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WopiWebApi.Utils;

namespace WopiWebApi.Models.Wopi
{
    public class WopiRequest
    {
        public WopiRequest(string fileId, string accessToken, HttpRequest request)
        {
            file_id = fileId;
            access_token = accessToken;
            XWOPISessionContext = String.Empty;
            XWOPIOverride = String.Empty;
            XWOPILock = String.Empty;
            XWOPIOldLock = String.Empty;
            XWOPIMaxExpectedSize = Int32.MaxValue;

            if (request.Headers.TryGetValue(WopiHeaders.SessionContext, out StringValues wopiSessions))
                XWOPISessionContext = wopiSessions.First();

            if (request.Headers.TryGetValue(WopiHeaders.Override, out StringValues wopiActions))
                XWOPIOverride = wopiActions.First();

            if (request.Headers.TryGetValue(WopiHeaders.Lock, out StringValues wopiLocks))
                XWOPILock = wopiLocks.First();

            if (request.Headers.TryGetValue(WopiHeaders.OldLock, out StringValues wopiOldLocks))
                XWOPIOldLock = wopiOldLocks.First();

            if (request.Headers.TryGetValue(WopiHeaders.MaxExpectedSize, out StringValues wopiSizes))
            {
                string maxSizeStr = wopiSizes.First();
                if (Int32.TryParse(maxSizeStr, out int maxSizeNum))
                    XWOPIMaxExpectedSize = maxSizeNum;
            }
        }

        public string file_id { get; set; }

        public string access_token { get; set; }

        public string XWOPISessionContext { get; set; }

        public string XWOPIOverride { get; set; }

        public string XWOPILock { get; set; }

        public string XWOPIOldLock { get; set; }

        public int XWOPIMaxExpectedSize { get; set; }

        public bool CheckAccessToken()
        {
            bool bRet = true;
            if (!String.IsNullOrEmpty(access_token))
            {
                if (!access_token.StartsWith("SID_WOPI_"))
                    bRet = false;
            }

            return bRet;
        }
    }
}
