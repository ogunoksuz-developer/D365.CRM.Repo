using LCW.Core.Attributes;
using LCW.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LCW.Entities.Concrete
{
    [DisplayColumn("StoreSchedule")]
    [DataContract(Name = "DailyStoreSchedule")]
    [Serializable]
    public class DailyStoreSchedule: IEntity
    {
        [EntityAttribute(isPrimaryKey = true, PropertyName = "Id")]
        [DataMember(Name = "Id")]
        public Guid Id { get; set; }

        [EntityAttribute(isPrimaryKey = false, PropertyName = "StoreCode")]
        [DataMember(Name = "StoreCode")]
        public string StoreCode { get; set; } = string.Empty;

        [EntityAttribute(isPrimaryKey = false, PropertyName = "StoreId")]
        [DataMember(Name = "StoreId")]
        public Guid StoreId { get; set; }

        [EntityAttribute(isPrimaryKey = false, isDateTime = true, dateTimeFormat = "yyyy-MM-dd", PropertyName = "ScheduleDate")]
        [DataMember(Name = "ScheduleDate")]
        public DateTime ScheduleDate { get; set; }        

        [EntityAttribute(isPrimaryKey = false, PropertyName = "OpeningTime")]
        [DataMember(Name = "OpeningTime")]
        public TimeSpan OpeningTime { get; set; }

        [EntityAttribute(isPrimaryKey = false, PropertyName = "ClosingTime")]
        [DataMember(Name = "ClosingTime")]
        public TimeSpan ClosingTime { get; set; }

        [EntityAttribute(isPrimaryKey = false, PropertyName = "IsClosed")]
        [DataMember(Name = "IsClosed")]
        public bool IsClosed { get; set; }
    }
}
