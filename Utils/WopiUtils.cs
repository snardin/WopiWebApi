using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WopiWebApi.Models;

namespace WopiWebApi.Utils
{
    public class WopiUtils
    {
        /// <summary>
        /// Forms the correct action url for the file and host
        /// </summary>
        public static string GetActionUrl(string hostPath, string actionSrc)
        {
            // Initialize the urlsrc
            var urlsrc = actionSrc;

            // Look through the action placeholders
            var phCnt = 0;
            foreach (var p in WopiUrlPlaceholders.Placeholders)
            {
                if (urlsrc.Contains(p))
                {
                    // Replace the placeholder value accordingly
                    var ph = WopiUrlPlaceholders.GetPlaceholderValue(p);
                    if (!String.IsNullOrEmpty(ph))
                    {
                        urlsrc = urlsrc.Replace(p, ph + "&");
                        phCnt++;
                    }
                    else
                        urlsrc = urlsrc.Replace(p, ph);
                }
            }

            urlsrc += ((phCnt > 0) ? "" : "?") + String.Format("wopisrc={0}/wopi/files/<HOST_FILE_ID>", hostPath);
            
            return urlsrc;
        }

        /// <summary>
        /// Contains valid WOPI response headers
        /// </summary>
        public class WopiResponseHeaders
        {
            //WOPI Header Consts
            public const string HOST_ENDPOINT = "X-WOPI-HostEndpoint";
            public const string INVALID_FILE_NAME_ERROR = "X-WOPI-InvalidFileNameError";
            public const string LOCK = "X-WOPI-Lock";
            public const string LOCK_FAILURE_REASON = "X-WOPI-LockFailureReason";
            public const string LOCKED_BY_OTHER_INTERFACE = "X-WOPI-LockedByOtherInterface";
            public const string MACHINE_NAME = "X-WOPI-MachineName";
            public const string PREF_TRACE = "X-WOPI-PerfTrace";
            public const string SERVER_ERROR = "X-WOPI-ServerError";
            public const string SERVER_VERSION = "X-WOPI-ServerVersion";
            public const string VALID_RELATIVE_TARGET = "X-WOPI-ValidRelativeTarget";
        }

        /// <summary>
        /// Contains valid WOPI request headers
        /// </summary>
        public class WopiRequestHeaders
        {
            //WOPI Header Consts
            public const string APP_ENDPOINT = "X-WOPI-AppEndpoint";
            public const string CLIENT_VERSION = "X-WOPI-ClientVersion";
            public const string CORRELATION_ID = "X-WOPI-CorrelationId";
            public const string LOCK = "X-WOPI-Lock";
            public const string MACHINE_NAME = "X-WOPI-MachineName";
            public const string MAX_EXPECTED_SIZE = "X-WOPI-MaxExpectedSize";
            public const string OLD_LOCK = "X-WOPI-OldLock";
            public const string OVERRIDE = "X-WOPI-Override";
            public const string OVERWRITE_RELATIVE_TARGET = "X-WOPI-OverwriteRelativeTarget";
            public const string PREF_TRACE_REQUESTED = "X-WOPI-PerfTraceRequested";
            public const string PROOF = "X-WOPI-Proof";
            public const string PROOF_OLD = "X-WOPI-ProofOld";
            public const string RELATIVE_TARGET = "X-WOPI-RelativeTarget";
            public const string REQUESTED_NAME = "X-WOPI-RequestedName";
            public const string SESSION_CONTEXT = "X-WOPI-SessionContext";
            public const string SIZE = "X-WOPI-Size";
            public const string SUGGESTED_TARGET = "X-WOPI-SuggestedTarget";
            public const string TIME_STAMP = "X-WOPI-TimeStamp";
        }

        /// <summary>
        /// Contains all valid URL placeholders for different WOPI actions
        /// </summary>
        public class WopiUrlPlaceholders
        {
            public const string BUSINESS_USER = "<IsLicensedUser=BUSINESS_USER&>";
            public const string DC_LLCC = "<rs=DC_LLCC&>";
            public const string DISABLE_ASYNC = "<na=DISABLE_ASYNC&>";
            public const string DISABLE_CHAT = "<dchat=DISABLE_CHAT&>";
            public const string DISABLE_BROADCAST = "<vp=DISABLE_BROADCAST&>";
            public const string EMBDDED = "<e=EMBEDDED&>";
            public const string FULLSCREEN = "<fs=FULLSCREEN&>";
            public const string PERFSTATS = "<showpagestats=PERFSTATS&>";
            public const string RECORDING = "<rec=RECORDING&>";
            public const string THEME_ID = "<thm=THEME_ID&>";
            public const string UI_LLCC = "<ui=UI_LLCC&>";
            public const string VALIDATOR_TEST_CATEGORY = "<testcategory=VALIDATOR_TEST_CATEGORY>";
            public const string HOST_SESSION_ID = "<hid=HOST_SESSION_ID&>";
            public const string SESSION_CONTEXT = "<sc=SESSION_CONTEXT&>";
            public const string WOPI_SOURCE = "<wopisrc=WOPI_SOURCE&>";
            public const string ACTIVITY_NAVIGATION_ID = "<actnavid=ACTIVITY_NAVIGATION_ID&>";

            public static List<string> Placeholders = new List<string>() {
                BUSINESS_USER, DC_LLCC, DISABLE_ASYNC, DISABLE_CHAT, DISABLE_BROADCAST,
                EMBDDED, FULLSCREEN, PERFSTATS, RECORDING, THEME_ID, UI_LLCC, VALIDATOR_TEST_CATEGORY,
                HOST_SESSION_ID, SESSION_CONTEXT, WOPI_SOURCE, ACTIVITY_NAVIGATION_ID
            };
            
            /// <summary>
            /// Sets a specific WOPI URL placeholder with the correct value
            /// Most of these are hard-coded in this WOPI implementation
            /// </summary>
            public static string GetPlaceholderValue(string placeholder)
            {
                var ph = placeholder.Substring(1, placeholder.IndexOf("="));
                string result = "";
                switch (placeholder)
                {
                    case BUSINESS_USER:
                        result = ph + "0";
                        break;
                    case DC_LLCC:
                    case UI_LLCC:
                        result = ph + "en-US";
                        break;
                    case DISABLE_ASYNC:
                    case DISABLE_BROADCAST:
                    case EMBDDED:
                    case FULLSCREEN:
                    case RECORDING:
                    case THEME_ID:
                        // These are all broadcast related actions
                        result = ph + "true";
                        break;
                    case DISABLE_CHAT:
                        result = ph + "1";
                        break;
                    case PERFSTATS:
                        result = ""; // No documentation
                        break;
                    case VALIDATOR_TEST_CATEGORY:
                        result = ph + "OfficeOnline"; //This value can be set to All, OfficeOnline or OfficeNativeClient to activate tests specific to Office Online and Office for iOS. If omitted, the default value is All.
                        break;
                    case HOST_SESSION_ID:
                        result = ph + "<HOST_SESSION_ID>";
                        break;
                    case SESSION_CONTEXT:
                        result = ""; //  If provided, this value will be passed back to the host in subsequent CheckFileInfo and CheckFolderInfo calls
                        break;
                    case WOPI_SOURCE:
                        result = "";
                        break;
                    case ACTIVITY_NAVIGATION_ID:
                        result = ""; // No documentation
                        break;
                    default:
                        result = "";
                        break;
                }

                return result;
            }
        }
    }
}
