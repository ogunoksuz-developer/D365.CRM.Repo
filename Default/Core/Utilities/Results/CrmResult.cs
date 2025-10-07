using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCW.Core.Utilities.Results
{
    public class CrmResult
    {
        public Guid Id { get; set; }

        public Dictionary<string,ResultDetailObject> ResultValues { get; set; }
    }

    public class ResultDetailObject
    {
        public string FormattedValues { get; set; }

        public object Values { get; set; }
    }
}
