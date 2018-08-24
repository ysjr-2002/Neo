using Common.Log;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;

namespace NeoVisitor.Wcf
{
    public static class WcfInvoker
    {
        static readonly string wcf_server = "";

        static WcfInvoker()
        {
            wcf_server = ConfigurationManager.AppSettings["localserver"];
        }

        public static void TestService()
        {
            long a = 0;
            string error = "";
            LocalCheck(1, "2", "1", out a, out error);
        }

        public static bool LocalCheck(long visitorId, string visitorguid, string channelno, out long elapsedTime, out string msg)
        {
            Stopwatch sw = Stopwatch.StartNew();
            try
            {
                ChannelFactory<IVisitorService> factory = CreateWCFChannel<IVisitorService>(wcf_server, "wsHttpBinding");
                IVisitorService visitorService = factory.CreateChannel();
                var result = visitorService.IsWellVisitor(visitorId, visitorguid, channelno, out msg);
                return result;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return false;
            }
            finally
            {
                sw.Stop();
                elapsedTime = sw.ElapsedMilliseconds;
                LogHelper.Info("call->" + sw.ElapsedMilliseconds);
            }
        }

        //public static void UploadPassRecord(string channelno, string visitorname)
        //{
        //    try
        //    {
        //        ChannelFactory<IVisitorService> factory = CreateWCFChannel<IVisitorService>(wcf_server, "wsHttpBinding");
        //        IVisitorService visitorService = factory.CreateChannel();
        //        visitorService.UpPassRecord(1, 0, 0, visitorname, channelno, 0, "", DateTime.Now, "", "", "");
        //    }
        //    catch (Exception ex)
        //    {
        //        LogUtils.d("上传访客记录异常->" + ex.Message);
        //    }
        //}

        public static ChannelFactory<T> CreateWCFChannel<T>(string url, string binding = "nettcpbinding")
        {
            return new ChannelFactory<T>(CreateBinding(binding), url);
        }

        private static Binding CreateBinding(string binding)
        {
            Binding bindinginstance = null;
            if (binding.ToLower() == "basichttpbinding")
            {
                BasicHttpBinding ws = new BasicHttpBinding();
                ws.MaxBufferSize = 2147483647;
                ws.MaxBufferPoolSize = 2147483647;
                ws.MaxReceivedMessageSize = 2147483647;
                ws.ReaderQuotas.MaxStringContentLength = 2147483647;
                ws.CloseTimeout = new TimeSpan(0, 10, 0);
                ws.OpenTimeout = new TimeSpan(0, 10, 0);
                ws.ReceiveTimeout = new TimeSpan(0, 10, 0);
                ws.SendTimeout = new TimeSpan(0, 10, 0);
                bindinginstance = ws;
            }
            else if (binding.ToLower() == "netnamedpipebinding")
            {
                NetNamedPipeBinding ws = new NetNamedPipeBinding();
                ws.MaxReceivedMessageSize = 65535000;
                bindinginstance = ws;
            }
            else if (binding.ToLower() == "nettcpbinding")
            {
                NetTcpBinding ws = new NetTcpBinding();
                ws.MaxReceivedMessageSize = 65535000;
                ws.MaxBufferSize = 2147483647;
                ws.MaxBufferPoolSize = 2147483647;
                ws.ReaderQuotas.MaxStringContentLength = 2147483647;
                ws.Security.Mode = SecurityMode.None;
                ws.ReceiveTimeout = new TimeSpan(0, 10, 0);
                ws.SendTimeout = new TimeSpan(0, 10, 0);
                bindinginstance = ws;
            }
            else if (binding.ToLower() == "wsdualhttpbinding")
            {
                WSDualHttpBinding ws = new WSDualHttpBinding();
                ws.MaxReceivedMessageSize = 65535000;
                bindinginstance = ws;
            }
            else if (binding.ToLower() == "webhttpbinding")
            {
                //WebHttpBinding ws = new WebHttpBinding();
                //ws.MaxReceivedMessageSize = 65535000;
                //bindinginstance = ws;
            }
            else if (binding.ToLower() == "wsfederationhttpbinding")
            {
                WSFederationHttpBinding ws = new WSFederationHttpBinding();
                ws.MaxReceivedMessageSize = 65535000;
                bindinginstance = ws;
            }
            else if (binding.ToLower() == "wshttpbinding")
            {
                WSHttpBinding ws = new WSHttpBinding(SecurityMode.None);
                ws.MaxReceivedMessageSize = 65535000;
                ws.Security.Message.ClientCredentialType = System.ServiceModel.MessageCredentialType.Windows;
                ws.Security.Transport.ClientCredentialType = System.ServiceModel.HttpClientCredentialType.Windows;
                bindinginstance = ws;
            }
            return bindinginstance;
        }
    }
}
