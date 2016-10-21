using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVisitor.Core
{
    class VerifyCloud
    {
        private const string VERIFY_URL = "http://v.wapwei.com/index.php?g=Api&m=Face&a=valid&code={0}";

        private VerifyCloud()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static bool Verify(string code)
        {
            var url = string.Format(VERIFY_URL, code);
            var httpBarcode = HttpMethod.VerifyCode(url);
            if (httpBarcode.status == 0)
                return true;
            else
                return false;
        }
    }
}
