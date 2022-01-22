using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YoutubeChatApi.Models
{
    readonly internal struct YoutubeCookies
    {
        public YoutubeCookies(string SAPISID, string HSID, string SSID, string APISID, string SID)
        {
            this.SAPISID = SAPISID;
            this.HSID = HSID;
            this.SSID = SSID;
            this.APISID = APISID;
            this.SID = SID;
        }

        public string SAPISID { get; }
        public string HSID { get; }
        public string SSID { get; }
        public string APISID { get; }
        public string SID { get; }
    }
}
