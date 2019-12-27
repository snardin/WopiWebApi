using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WopiWebApi.Utils
{
    public static class WopiHeaders
    {
        public const string SessionContext = "X-WOPI-SessionContext";
        public const string MaxExpectedSize = "X-WOPI-MaxExpectedSize";
        public const string Override = "X-WOPI-Override";
        public const string Lock = "X-WOPI-Lock";
        public const string OldLock = "X-WOPI-OldLock";
        public const string LockFailureReason = "X-WOPI-LockFailureReason";
        public const string ItemVersion = "X-WOPI-ItemVersion";

    }

    public static class WopiAction
    {
        public const string PUT = "PUT";
        public const string LOCK = "LOCK";
        public const string GET_LOCK = "GET_LOCK";
        public const string UNLOCK = "UNLOCK";
        public const string REFRESH_LOCK = "REFRESH_LOCK";
        public const string PUT_USER_INFO = "PUT_USER_INFO";
        public const string PUT_RELATIVE = "PUT_RELATIVE";
    }
}
