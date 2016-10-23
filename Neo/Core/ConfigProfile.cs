using Common.Log;
using Common.NotifyBase;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoVisitor.Core
{
    /// <summary>
    /// 系统配置项
    /// </summary>
    public class ConfigProfile : PropertyNotifyObject
    {
        /// <summary>
        /// 通道ID
        /// </summary>
        public string TermID
        {
            get { return this.GetValue(s => s.TermID); }
            set { this.SetValue(s => s.TermID, value); }
        }
        /// <summary>
        /// 入串口
        /// </summary>
        public string InPortName
        {
            get { return this.GetValue(s => s.InPortName); }
            set { this.SetValue(s => s.InPortName, value); }
        }
        /// <summary>
        /// API Url
        /// </summary>
        public string MeetingAPIUrl
        {
            get { return this.GetValue(s => s.MeetingAPIUrl); }
            set { this.SetValue(s => s.MeetingAPIUrl, value); }
        }

        public string RebootWeekofDay
        {
            get { return this.GetValue(s => s.RebootWeekofDay); }
            set { this.SetValue(s => s.RebootWeekofDay, value); }
        }

        public string RebootTime
        {
            get { return this.GetValue(s => s.RebootTime); }
            set { this.SetValue(s => s.RebootTime, value); }
        }


        static ConfigProfile()
        {
            LogHelper.Info("系统启动--------------------------------------->");
        }

        private ConfigProfile()
        {
        }

        private static object lockObj = new object();
        private static ConfigProfile _instance = null;
        public static ConfigProfile Instance
        {
            get
            {
                //双判断锁定
                if (_instance == null)
                {
                    lock (lockObj)
                    {
                        if (_instance == null)
                        {
                            _instance = new ConfigProfile();
                        }
                    }
                }
                return _instance;
            }
        }

        public void ReadConfig()
        {
            try
            {
                TermID = GetKey("termId");
                InPortName = GetKey("inPortName");
                MeetingAPIUrl = GetKey("opendoorAPIUrl");
                RebootWeekofDay = GetKey("rebootweekofDay");
                RebootTime = GetKey("reboottime");
            }
            catch
            {
                LogHelper.Info("加载参数异常");
            }
        }

        private string GetKey(string key)
        {
            if (ConfigurationManager.AppSettings.AllKeys.Contains(key))
            {
                var val = ConfigurationManager.AppSettings[key];
                LogHelper.Info(string.Format("参数[{0}]={1}", key, val));
                return val;
            }
            else
            {
                return string.Empty;
            }
        }

        public bool SaveConfig()
        {
            try
            {
                Configuration cfg = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                cfg.AppSettings.Settings["termId"].Value = TermID.ToString();
                cfg.AppSettings.Settings["inPortName"].Value = InPortName;
                cfg.AppSettings.Settings["opendoorAPIUrl"].Value = MeetingAPIUrl;
                cfg.AppSettings.Settings["rebootweekofDay"].Value = RebootWeekofDay;
                cfg.AppSettings.Settings["reboottime"].Value = RebootTime;
                cfg.Save();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
