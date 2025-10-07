using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCW.Core.Utilities.Results
{
    public class PaggingResult
    {

        public int TotalCount { get; set; }

        public List<CrmResult> Results { get; set; }
    }
}
