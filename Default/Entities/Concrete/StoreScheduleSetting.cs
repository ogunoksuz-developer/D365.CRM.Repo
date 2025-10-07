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
    [DataContract(Name = "obs_storeschedulesetting")]
    [Serializable]
    public class StoreScheduleSetting : ICrmEntity
    {
        public string LogicalName { get { return "obs_storeschedulesetting"; } }

        public string WebApiLogicalName { get { return "obs_storeschedulesettings"; } }

        [EntityAttribute(isPrimaryKey = true, PropertyName = "obs_storeschedulesettingid")]
        [DataMember(Name = "obs_storeschedulesettingid")]
        public Guid Id { get; set; }

        [EntityAttribute(isPrimaryKey = false, PropertyName = "obs_startdate", isDateTime = true, dateTimeFormat = "yyyy-MM-dd")]
        [DataMember(Name = "obs_startdate")]
        public DateTime StartDate { get; set; }

        [EntityAttribute(isPrimaryKey = false, PropertyName = "obs_enddate", isDateTime = true, dateTimeFormat = "yyyy-MM-dd")]
        [DataMember(Name = "obs_enddate")]
        public DateTime EndDate { get; set; }

        [EntityAttribute(isPrimaryKey = false, PropertyName = "obs_storecode")]
        [DataMember(Name = "obs_storecode")]
        public string StoreCode { get; set; }

        [EntityAttribute(isPrimaryKey = false, isEntityReference = true, EntityName = "obs_store", PropertyName = "obs_storeid")]
        [DataMember(Name = "_obs_storeid_value")]
        public Guid? Store { get; set; }

        [EntityAttribute(isPrimaryKey = false, PropertyName = "statecode")]
        [DataMember(Name = "statecode")]
        public int StateCode { get; set; }

        [EntityAttribute(isPrimaryKey = false, PropertyName = "statuscode")]
        [DataMember(Name = "statuscode")]
        public int StatusCode { get; set; }

        [EntityAttribute(isPrimaryKey = false, PropertyName = "obs_closedstore")]
        [DataMember(Name = "obs_closedstore")]
        public bool ClosedStore { get; set; }

        [EntityAttribute(isPrimaryKey = false, PropertyName = "obs_storeopeningtime")]
        [DataMember(Name = "obs_storeopeningtime")]
        public string StoreOpeningTime { get; set; }

        [EntityAttribute(isPrimaryKey = false, PropertyName = "obs_storeclosingtime")]
        [DataMember(Name = "obs_storeclosingtime")]
        public string StoreClosingTime { get; set; }

        [EntityAttribute(isPrimaryKey = false, PropertyName = "obs_type")]
        [DataMember(Name = "obs_type")]
        public int Type { get; set; }

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

        [EntityAttribute(isPrimaryKey = false, PropertyName = "obs_fridayppeningtime")]
        [DataMember(Name = "obs_fridayppeningtime")]
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

        [EntityAttribute(isPrimaryKey = false, PropertyName = "obs_isstorenotfound")]
        [DataMember(Name = "obs_isstorenotfound")]
        public bool IsStoreNotFound { get; set; }

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
