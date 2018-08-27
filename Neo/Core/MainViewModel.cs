using Common;
using Common.Log;
using Common.NotifyBase;
using NeoVisitor.config;
using NeoVisitor.Wcf;
using Obria.Core;
using SR104;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace NeoVisitor.Core
{
    class MainViewModel : PropertyNotifyObject, IDisposable
    {
        private const string OKImage = "yes.png";
        private const string NOImage = "no.png";
        private const string WelCome = "请刷二维码通行";

        private WGSerialReader _wgReader = null;
        private BardCodeHook BarCode = new BardCodeHook();

        private FuncTimeout _timeout = null;

        public string StateImage
        {
            get { return this.GetValue(s => s.StateImage); }
            set { this.SetValue(s => s.StateImage, value); }
        }

        public string VerfiyMessage
        {
            get { return this.GetValue(s => s.VerfiyMessage); }
            set { this.SetValue(s => s.VerfiyMessage, value); }
        }

        public string QRCode
        {
            get { return this.GetValue(s => s.QRCode); }
            set { this.SetValue(s => s.QRCode, value); }
        }

        Obria.Core.UdpComServer udpserver = null;
        public void Init()
        {
            if (ConfigProfile.Instance.VirtualPort.ToLower() == "none")
            {
                BarCode.BarCodeEvent += new BardCodeHook.BardCodeDeletegate(BarCode_BarCodeEvent);
                BarCode.Start();
            }
            else
            {
                _wgReader = new WGSerialReader();
                _wgReader.SetQRCodeCallback(ReadCode);
                _wgReader.Open(ConfigProfile.Instance.VirtualPort);
            }

            SRController.OpenPort(ConfigProfile.Instance.SwitchPort);
            _timeout = new FuncTimeout();

            VerfiyMessage = WelCome;

            udpserver = new UdpComServer(9876);
            udpserver.OnMessageInComming += Udpserver_OnMessageInComming;
            udpserver.ReceiveAsync();

            CheatCall();
        }

        private void Udpserver_OnMessageInComming(object sender, Obria.Core.DataEventArgs e)
        {
            var code = e.BarCode;
            var channel = Channels.ChannelList.FirstOrDefault(s => s.ReaderIp == e.ReaderIp);
            if (channel != null)
            {
                RemoteCode(code, true, channel.GateIp);
            }
        }

        private async void CheatCall()
        {
            var task = Task.Factory.StartNew(() =>
            {
                var qrcode = "run";
                VerifyCloud.PostVerify(qrcode);
            });
            await task;
        }

        private void BarCode_BarCodeEvent(BardCodeHook.BarCodes barCode)
        {
            LogHelper.Info(barCode.BarCode);
        }

        /// <summary>
        /// Usb或串口数据
        /// </summary>
        /// <param name="qrcode"></param>
        public void ReadCode(string qrcode)
        {
            RemoteCode(qrcode, false, "");
        }

        /// <summary>
        /// 网络二维码
        /// </summary>
        /// <param name="qrcode"></param>
        /// <param name="bRemote"></param>
        /// <param name="gateIp"></param>
        public void RemoteCode(string qrcode, bool bRemote, string gateIp)
        {
            var check = ReadBarCode(qrcode);
            if (bRemote)
            {
                //远程二维码，进行远程开闸
                if (check)
                {
                    OpenRemoteGate(gateIp);
                }
            }
            else
            {
                if (check)
                {
                    OpenLocalGate();
                }
            }
            _timeout.StartOnce(2000, () =>
            {
                StateImage = "";
                VerfiyMessage = WelCome;
            });
        }

        private bool ReadBarCode(string qrcode)
        {
            if (qrcode.StartsWith("LOCAL"))
            {
                //本地验证
                var elapsedTime = 0L;
                var error = "";
                var array = qrcode.Split(',');
                var visitorId = Convert.ToInt64(array[1]);
                var visitorguid = array[2];
                var open = WcfInvoker.LocalCheck(visitorId, visitorguid, ConfigProfile.Instance.TermID, out elapsedTime, out error);
                if (open)
                {
                    StateImage = "yes.png";
                    VerfiyMessage = "请通行";
                    LogHelper.Info("请通行->" + visitorguid);
                    return true;
                }
                else
                {
                    StateImage = "no.png";
                    VerfiyMessage = "未授权";
                    LogHelper.Info("未授权->" + visitorguid + error);
                    return false;
                }
            }
            else
            {
                var success = true;
                success = VerifyCloud.PostVerify(qrcode);
                if (success)
                {
                    StateImage = "yes.png";
                    VerfiyMessage = "请通行";
                    return true;
                }
                else
                {
                    StateImage = "no.png";
                    VerfiyMessage = "未授权";
                    return false;
                }
            }
        }

        private void OpenRemoteGate(string gateIp)
        {
            NIRenGate remote = new NIRenGate(gateIp);
            remote.Open(5);
        }

        private void OpenLocalGate()
        {
            SRController.Close(1);
            Thread.Sleep(ConfigProfile.Instance.OpenDelay);
            SRController.Free(1);
        }

        public void Dispose()
        {
            BarCode.Stop();
            udpserver?.Stop();
            SRController.ClosePort();
            if (_wgReader != null)
            {
                //关闭虚拟串口
                _wgReader.Dispose();
            }
        }
    }
}
