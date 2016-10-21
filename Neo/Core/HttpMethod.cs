using Common;
using Common.Log;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace NeoVisitor.Core
{
    class HttpMethod
    {
        public static HttpBarcode GetBarCode(string url)
        {
            var json = Get(url);
            if (json.IsEmpty())
                return new HttpBarcode();

            var code = HttpBarcodeDeserialize<HttpBarcode>(json);
            return code;
        }

        public static HttpBarcode VerifyCode(string url)
        {
            var json = Get(url);
            if (json.IsEmpty())
                return new HttpBarcode();

            var code = HttpBarcodeDeserialize<HttpBarcode>(json);
            return code;
        }

        private static string Get(string url)
        {
            Stopwatch sw = Stopwatch.StartNew();
            var webRequest = (HttpWebRequest)WebRequest.Create(url);
            LogHelper.Info("服务地址:" + url);
            webRequest.Method = "Get";
            webRequest.Timeout = 5000;
            try
            {
                using (var webResponse = webRequest.GetResponse())
                {
                    using (var stream = webResponse.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream);
                        var json = reader.ReadToEnd();
                        sw.Stop();
                        LogHelper.Info("调用耗时:" + sw.ElapsedMilliseconds);
                        var jsonStr = "调用返回:" + json;
                        jsonStr = jsonStr.Replace("{", "[");
                        jsonStr = jsonStr.Replace("}", "]");
                        LogHelper.LogJson(jsonStr);
                        return json;
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Info("访问服务异常:" + ex.Message);
                return string.Empty;
            }
        }

        private static T HttpBarcodeDeserialize<T>(string input) where T : new()
        {
            try
            {
                input = input.Replace("[]", "null");
                JavaScriptSerializer serial = new JavaScriptSerializer();
                var code = serial.Deserialize<T>(input);
                return code;
            }
            catch
            {
                return new T();
            }
        }
    }
}
