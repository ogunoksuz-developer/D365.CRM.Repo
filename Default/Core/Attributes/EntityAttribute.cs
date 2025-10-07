using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCW.Core.Attributes
{
    public interface IEntityAttributes
    {

    }

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class EntityAttribute : Attribute
    {
        public EntityAttribute()
        {
            this.allowNull = false;
            this.isPrimaryKey = false;
            this.isForeignKey = false;
            this.isOptionSet = false;
            this.NoCreate = false;
            this.isDateTime = false;
        }

        public string JsonPropertyName { get; set; }

        public string PropertyName { get; set; }

        public bool isPrimaryKey { get; set; }

        public bool isForeignKey { get; set; }

        public bool allowNull { get; set; }

        public int MaxLength { get; set; }

        public bool isRequired { get; set; }

        public bool isAutoNumber { get; set; }

        public bool isOptionSet { get; set; }

        public bool isEntityReference { get; set; }

        public bool isMoney { get; set; }

        public string EntityName { get; set; }

        public bool isAliasValue { get; set; }

        public string AliasName { get; set; }

        public bool isMasked { get; set; }

        public bool isText { get; set; }

        public bool isReference { get; set; }

        public bool isInt { get; set; }

        public bool NoCreate { get; set; }

        public bool isDateTime { get; set; }

        public bool isActivityPartyCollection { get; set; }

        public string dateTimeFormat { get; set; } = "yyyy-MM-dd";
    }
}
