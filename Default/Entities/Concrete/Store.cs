using LCW.Core.Attributes;
using LCW.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace LCW.Entities.Concrete
{
    [DataContract(Name = "obs_store")]
    [Serializable]
    public class Store : ICrmEntity
    {
        public string LogicalName { get { return "obs_store"; } }

        public string WebApiLogicalName { get { return "obs_stores"; } }

        [EntityAttribute(isPrimaryKey = true, PropertyName = "obs_storeid")]
        [DataMember(Name = "obs_storeid")]
        public Guid Id { get; set; }

        [EntityAttribute(isPrimaryKey = false, PropertyName = "obs_code")]
        [DataMember(Name = "obs_code")]
        public string Code { get; set; }

        [EntityAttribute(isPrimaryKey = false, PropertyName = "obs_closedstore")]
        [DataMember(Name = "obs_closedstore")]
        public bool ClosedStore { get; set; }

        [EntityAttribute(isPrimaryKey = false, PropertyName = "obs_storeopeningtime")]
        [DataMember(Name = "obs_storeopeningtime")]
        public string StoreOpeningTime { get; set; }

        [EntityAttribute(isPrimaryKey = false, PropertyName = "obs_storeclosingtime")]
        [DataMember(Name = "obs_storeclosingtime")]
        public string StoreClosingTime { get; set; }

        [EntityAttribute(isPrimaryKey = false, PropertyName = "obs_mondayopeningtime")]
        [DataMember(Name = "obs_mondayopeningtime")]
        public string MondayOpeningTime { get; set; }

        [EntityAttribute(isPrimaryKey = false, PropertyName = "obs_mondayclosingtime")]
        [DataMember(Name = "obs_mondayclosingtime")]
        public string MondayClosingTime { get; set; }

        [EntityAttribute(isPrimaryKey = false, PropertyName = "obs_tuesdayopeningtime")]
        [DataMember(Name = "obs_tuesdayopeningtime")]
        public string TuesdayOpeningTime { get; set; }

        [EntityAttribute(isPrimaryKey = false, PropertyName = "obs_tuesdayclosingtime")]
        [DataMember(Name = "obs_tuesdayclosingtime")]
        public string TuesdayClosingTime { get; set; }

        [EntityAttribute(isPrimaryKey = false, PropertyName = "obs_wednesdayopeningtime")]
        [DataMember(Name = "obs_wednesdayopeningtime")]
        public string WednesdayOpeningTime { get; set; }

        [EntityAttribute(isPrimaryKey = false, PropertyName = "obs_wednesdayclosingtime")]
        [DataMember(Name = "obs_wednesdayclosingtime")]
        public string WednesdayClosingTime { get; set; }

        [EntityAttribute(isPrimaryKey = false, PropertyName = "obs_thursdayopeningtime")]
        [DataMember(Name = "obs_thursdayopeningtime")]
        public string ThursdayOpeningTime { get; set; }

        [EntityAttribute(isPrimaryKey = false, PropertyName = "obs_thursdayclosingtime")]
        [DataMember(Name = "obs_thursdayclosingtime")]
        public string ThursdayClosingTime { get; set; }

        [EntityAttribute(isPrimaryKey = false, PropertyName = "obs_fridayopeningtime")]
        [DataMember(Name = "obs_fridayopeningtime")]
        public string FridayOpeningTime { get; set; }

        [EntityAttribute(isPrimaryKey = false, PropertyName = "obs_fridayclosingtime")]
        [DataMember(Name = "obs_fridayclosingtime")]
        public string FridayClosingTime { get; set; }

        [EntityAttribute(isPrimaryKey = false, PropertyName = "obs_saturdayopeningtime")]
        [DataMember(Name = "obs_saturdayopeningtime")]
        public string SaturdayOpeningTime { get; set; }

        [EntityAttribute(isPrimaryKey = false, PropertyName = "obs_saturdayclosingtime")]
        [DataMember(Name = "obs_saturdayclosingtime")]
        public string SaturdayClosingTime { get; set; }

        [EntityAttribute(isPrimaryKey = false, PropertyName = "obs_sundayopeningtime")]
        [DataMember(Name = "obs_sundayopeningtime")]
        public string SundayOpeningTime { get; set; }

        [EntityAttribute(isPrimaryKey = false, PropertyName = "obs_sundayclosingtime")]
        [DataMember(Name = "obs_sundayclosingtime")]
        public string SundayClosingTime { get; set; }

        [EntityAttribute(isPrimaryKey = false, PropertyName = "obs_backuptime")]
        [DataMember(Name = "obs_backuptime")]
        public string BackupTime { get; set; }

        [EntityAttribute(isPrimaryKey = false, PropertyName = "obs_isclosedmonday")]
        [DataMember(Name = "obs_isclosedmonday")]
        public bool IsClosedMonday { get; set; } = false;

        [EntityAttribute(isPrimaryKey = false, PropertyName = "obs_isclosedtuesday")]
        [DataMember(Name = "obs_isclosedtuesday")]
        public bool IsClosedTuesday { get; set; } = false;

        [EntityAttribute(isPrimaryKey = false, PropertyName = "obs_isclosedwednesday")]
        [DataMember(Name = "obs_isclosedwednesday")]
        public bool IsClosedWednesday { get; set; } = false;

        [EntityAttribute(isPrimaryKey = false, PropertyName = "obs_isclosedthursday")]
        [DataMember(Name = "obs_isclosedthursday")]
        public bool IsClosedThursday { get; set; } = false;

        [EntityAttribute(isPrimaryKey = false, PropertyName = "obs_isclosedfriday")]
        [DataMember(Name = "obs_isclosedfriday")]
        public bool IsClosedFriday { get; set; } = false;

        [EntityAttribute(isPrimaryKey = false, PropertyName = "obs_isclosedsaturday")]
        [DataMember(Name = "obs_isclosedsaturday")]
        public bool IsClosedSaturday { get; set; } = false;

        [EntityAttribute(isPrimaryKey = false, PropertyName = "obs_isclosedsunday")]
        [DataMember(Name = "obs_isclosedsunday")]
        public bool IsClosedSunday { get; set; } = false;
    }
}
