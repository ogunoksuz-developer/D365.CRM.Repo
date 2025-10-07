using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LCW.Core.Entities
{

    [DataContract(Name = "Option")]
    [Serializable]
    public class Option : IEntity
    {
        [DataMember(Name = "Value")]
        public int Value { get; set; }

        [DataMember(Name = "Label")]
        public string Label { get; set; }

        [DataMember(Name = "Id")]
        public Guid Id { get; set; }

        public Option()
        {
            Value = -1;
        }
        public Option(int value)
        {
            Value = value;
        }

        public Option(int value, string label)
        {
            Value = value;
            Label = label;
        }

        public Option(Guid id, string label)
        {
            Id = id;
            Label = label;
        }
    }
}
