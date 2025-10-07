using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCW.Core.Entities
{
    public interface ICrmEntity
    {
        Guid Id { get; set; }

        string LogicalName { get; }

        string WebApiLogicalName { get; }
    }
}
