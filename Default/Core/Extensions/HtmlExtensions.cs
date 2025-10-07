using LCW.Core.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace LCW.Core.Extensions
{
    public static class HtmlExtensions
    {
        public static string HtmlRemove(this string str)
        {
            return RecursiveHtmlDecode(HtmlRemoval.StripTagsCharArray(str));
        }

        public static string RecursiveHtmlDecode(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return str;

            var tmp = HttpUtility.HtmlDecode(str);
            while (tmp != str)
            {
                str = tmp;
                tmp = HttpUtility.HtmlDecode(str);
            }


            return str; //completely decoded string
        }
    }
}
