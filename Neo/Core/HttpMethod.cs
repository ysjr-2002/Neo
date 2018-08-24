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
using System.Web;
using System.Web.Script.Serialization;

namespace NeoVisitor.Core
{
    class HttpMethod
    {
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
            LogHelper.Info(url);
            webRequest.Method = "Get";
            webRequest.Timeout = 5000;
            var json = "";
            try
            {
                using (var webResponse = webRequest.GetResponse())
                {
                    using (var stream = webResponse.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(stream);
                        json = reader.ReadToEnd();
                        var jsonStr = json;
                        jsonStr = jsonStr.Replace("{", "[");
                        jsonStr = jsonStr.Replace("}", "]");
                        LogHelper.LogJson(jsonStr);
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.Info("访问服务异常:" + ex.Message);
                json = string.Empty;
            }
            finally
            {
                sw.Stop();
                LogHelper.Info("call api:" + sw.ElapsedMilliseconds);
            }
            return json;
        }

        public static HttpBarcode VerifyCode(string url, string code)
        {
            var json = Post(url, code);
            if (json.IsEmpty())
                return new HttpBarcode();

            var httpbarcode = HttpBarcodeDeserialize<HttpBarcode>(json);
            return httpbarcode;
        }

        public static string Post(string url, string code)
        {
            Stopwatch sw = Stopwatch.StartNew();
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "application/x-www-form-urlencoded";
            request.Method = "POST";
            code = HttpUtility.UrlEncode(code);
            var data = "code=" + code;
            var buffer = Encoding.UTF8.GetBytes(data);
            request.ContentLength = buffer.Length;
            try
            {
                var requestStream = request.GetRequestStream();
                requestStream.Write(buffer, 0, buffer.Length);
                requestStream.Close();

                var response = request.GetResponse();
                var responseStream = response.GetResponseStream();
                StreamReader sr = new StreamReader(responseStream, Encoding.UTF8);
                var content = sr.ReadToEnd();
                sr.Close();
                responseStream.Close();
                return content;
            }
            catch (Exception ex)
            {
                LogHelper.Info("访问服务异常->" + ex.Message);
                return string.Empty;
            }
            finally
            {
                sw.Stop();
                LogHelper.Info("call->" + sw.ElapsedMilliseconds);
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
