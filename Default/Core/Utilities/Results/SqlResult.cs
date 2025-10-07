using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCW.Core.Utilities.Results
{
    public class SqlResult
    {
        public int Id { get; set; }
        public bool IsOk { get; set; }
        public string Message { get; set; }
        public string ExMessage { get; set; }
        public Exception InnerException { get; set; }
    }
}
