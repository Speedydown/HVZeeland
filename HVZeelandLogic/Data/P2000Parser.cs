using BaseLogic.HtmlUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVZeelandLogic
{
    internal static class P2000Parser
    {
        public static IList<P2000Item> GetP2000ItemsFromSource(string Source)
        {
            return ParseContent(Source);
        }

        private static List<P2000Item> ParseContent(string Input)
        {
            List<P2000Item> P2000Items = new List<P2000Item>();

            while (true)
            {
                try
                {
                    Input = Input.Substring(HTMLParserUtil.GetPositionOfStringInHTMLSource("<td width=50 class=text valign = top>", Input, false));
                    string Time = HTMLParserUtil.GetContentAndSubstringInput("<td width=50 class=text valign = top>", "</td>", Input, out Input);
                    string Content = HTMLParserUtil.GetContentAndSubstringInput("<td width=470 class=text valign = top>", "</td>", Input, out Input);

                    P2000Items.Add(new P2000Item(Time, Content));
                }
                catch
                {
                    break;
                }
            }

            if (P2000Items.Count == 0)
            {
                throw new Exception("p2000 not available");
            }

            return P2000Items;
        }
    }
}
