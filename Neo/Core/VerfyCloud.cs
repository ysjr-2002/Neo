using Common.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NeoVisitor.Core
{
    class VerifyCloud
    {
        private static readonly string VERIFY_URL = "";
        private static string etx = "&code={0}";

        static VerifyCloud()
        {
            VERIFY_URL = ConfigProfile.Instance.opendoorAPIUrl;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static bool GetVerify(string code)
        {
            var url = string.Concat(VERIFY_URL, etx);
            code = HttpUtility.UrlEncode(code);
            url = string.Format(url, code);
            var httpBarcode = HttpMethod.VerifyCode(url);
            if (httpBarcode.status == 0)
                return true;
            else
                return false;
        }

        public static bool PostVerify(string code)
        {
            var httpBarcode = HttpMethod.VerifyCode(VERIFY_URL, code);
            if (httpBarcode.status == 0)
            {
                LogHelper.Info("请通行->" + code);
                return true;
            }
            else
            {
                LogHelper.Info("未授权->" + code);
                return false;
            }
        }
    }
}
