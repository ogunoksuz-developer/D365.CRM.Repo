using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerApps.Samples.Types
{
    public class PagingInfo
    {
        public int Count { get; set; }
        public int PageNumber { get; set; }
        public string PagingCookie { get; set; }
        public bool ReturnTotalRecordCount { get; set; }
    }
}
