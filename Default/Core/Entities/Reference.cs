using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LCW.Core.Entities
{
    [DataContract(Name = "Reference")]
    [Serializable]
    public class Reference : IEntity
    {
        [DataMember(Name = "Id")]
        public Guid Id { get; set; }

        [DataMember(Name = "Name")]
        public string Name { get; set; }

        [DataMember(Name = "LogicalName")]
        public string LogicalName { get; set; }

        public Reference() { }

        public Reference(string logicalName, Guid id, string name)
        {
            Id = id;
            LogicalName = logicalName;
            Name = name;
        }

        public Reference(string logicalName, Guid id)
        {
            Id = id;
            LogicalName = logicalName;
        }

    }
}
