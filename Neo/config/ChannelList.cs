using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NeoVisitor.config
{
    internal class Channels
    {
        static string filepath = System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "channel.xml");
        static XElement xelement = null;
        private Channels()
        {
        }

        static Channels()
        {
            ChannelList = new List<Channel>();
        }

        public static List<Channel> ChannelList
        {
            get;
            private set;
        }

        public static void Load()
        {
            if (!System.IO.File.Exists(filepath))
            {
                return;
            }
            xelement = XElement.Load(filepath);
            foreach (var item in xelement.Elements("channel"))
            {
                Channel cm = new Channel
                {
                    GateIp = item.Element("gateIp").Value,
                    ReaderIp = item.Element("readerIp").Value,
                };
                ChannelList.Add(cm);
            }
        }
    }
}
