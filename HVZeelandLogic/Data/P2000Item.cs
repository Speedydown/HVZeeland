using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HVZeelandLogic
{
    public sealed class P2000Item
    {
        public string Time { get; private set; }
        public string Content { get; private set; }

        public P2000Item(string Time, string Content)
        {
            this.Time = WebUtility.HtmlDecode(Time).Replace("\t", "").Replace("\n", "");
            this.Content = WebUtility.HtmlDecode(Content).Replace("\t", "").Replace("\n", "");
        }
    }
}
