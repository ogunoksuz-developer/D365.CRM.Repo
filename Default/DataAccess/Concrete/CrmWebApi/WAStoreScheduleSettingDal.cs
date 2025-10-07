using LCW.Core.DataAccess;
using LCW.Core.DataAccess.CDSWebApi;
using LCW.Core.Utilities.Results;
using LCW.DataAccess.Abstract;
using LCW.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LCW.Core.Enums.Enumarations;

namespace LCW.DataAccess.Concrete.CrmWebApi
{
    public class WAStoreScheduleSettingDal : WAEntityRepositoryBase<StoreScheduleSetting>, IStoreScheduleSettingDal
    {
        public WAStoreScheduleSettingDal() : base()
        {
            // Initialization code, if any
        }

        /// <summary>
        /// Retrieves a list of store schedule settings based on the provided criteria.
        /// </summary>
        /// <param name="startDate">The start date to filter the schedule settings.</param>
        /// <param name="endDate">The end date to filter the schedule settings.</param>
        /// <param name="statusCode">The status code to filter the schedule settings. Default is 1.</param>
        /// <returns>A list of store schedule settings that match the provided criteria.</returns>
        public async Task<IList<StoreScheduleSetting>> StoreScheduleSettingList(DateTime? startDate, DateTime? endDate, int? statusCode = 1)
        {
            #region Build Condition String
            var conditionStr = string.Empty;

            if (startDate.HasValue)
                conditionStr += $@"<condition attribute='obs_startdate' operator='on-or-before' value='{startDate.Value.ToString("yyyy-MM-dd")}' />";

            if (endDate.HasValue)
                conditionStr += $@"<condition attribute='obs_enddate' operator='on-or-before' value='{endDate.Value.ToString("yyyy-MM-dd")}' />";

            if (statusCode.HasValue)
                conditionStr += $@"<condition attribute='statuscode' operator='eq' value='{statusCode.Value}' />";
            #endregion

            #region Construct FetchXML
            var fetchXml = $@"<fetch version=""1.0"" output-format=""xml-platform"" mapping=""logical"" distinct=""false"">
                        <entity name=""obs_storeschedulesetting"">
                          <attribute name=""obs_storeschedulesettingid"" />
                          <attribute name=""obs_wednesdayopeningtime"" />
                          <attribute name=""obs_wednesdayclosingtime"" />
                          <attribute name=""obs_tuesdayopeningtime"" />
                          <attribute name=""obs_tuesdayclosingtime"" />
                          <attribute name=""obs_thursdayopeningtime"" />
                          <attribute name=""obs_thursdayclosingtime"" />
                          <attribute name=""obs_sundayopeningtime"" />
                          <attribute name=""obs_sundayclosingtime"" />
                          <attribute name=""obs_saturdayopeningtime"" />
                          <attribute name=""obs_saturdayclosingtime"" />
                          <attribute name=""obs_mondayopeningtime"" />
                          <attribute name=""obs_mondayclosingtime"" />
                          <attribute name=""obs_fridayppeningtime"" />
                          <attribute name=""obs_fridayclosingtime"" />
                          <attribute name=""obs_startdate"" />
                          <attribute name=""obs_enddate"" />
                          <attribute name=""obs_storecode"" />
                          <attribute name=""obs_storeid"" />
                          <attribute name=""statecode"" />
                          <attribute name=""statuscode"" />
                          <attribute name=""obs_isstorenotfound"" />
                          <attribute name=""obs_closedstore"" />
                          <attribute name=""obs_storeopeningtime"" />
                          <attribute name=""obs_storeclosingtime"" />
                          <attribute name=""obs_isclosedmonday"" />
                          <attribute name=""obs_isclosedtuesday"" />
                          <attribute name=""obs_isclosedwednesday"" />
                          <attribute name=""obs_isclosedthursday"" />
                          <attribute name=""obs_isclosedfriday"" />   
                          <attribute name=""obs_isclosedsaturday"" />
                          <attribute name=""obs_isclosedsunday"" />
                          <attribute name=""obs_type"" />
                          <filter type=""and"">
                            <condition attribute=""statecode"" operator=""eq"" value=""0"" />
                            {conditionStr}
                          </filter>
                        </entity>
                      </fetch>";
            #endregion

            #region Fetch Store Schedule Settings
            var entities = (await GetList(new StoreScheduleSetting().WebApiLogicalName, fetchXml)).ToList();
            #endregion

            return entities;
        }

      
    }
}
