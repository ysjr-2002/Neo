using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVisitor.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class HttpBarcode
    {
        public HttpBarcode()
        {
            status = -1;
        }

        public int status { get; set; }

        public string message { get; set; }

        public Code data { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Code
    {
        public Code()
        {
        }
        public string code { get; set; }
    }
}
