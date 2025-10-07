using LCW.Core.DataAccess.CDSWebApi;
using LCW.Core.Utilities.Results;
using LCW.DataAccess.Abstract;
using LCW.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace LCW.DataAccess.Concrete.CrmWebApi
{
    public class WAStoreDal : WAEntityRepositoryBase<Store>, IStoreDal
    {
        public WAStoreDal() : base()
        {
            // Initialization code, if any
        }

        /// <summary>
        /// Retrieves a list of stores by their codes using FetchXML.
        /// </summary>
        /// <param name="codes">The list of store codes to retrieve.</param>
        /// <returns>A list of stores that match the provided codes.</returns>
        public async Task<IList<Store>> GetStoreListByCode(List<string> codes)
        {
            #region Construct FetchXML
            var fetchXml = $@"<fetch version=""1.0"" output-format=""xml-platform"" mapping=""logical"" distinct=""false"">
                        <entity name=""obs_store"">
                          <attribute name=""obs_storeid"" />
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
                          <attribute name=""obs_fridayopeningtime"" />
                          <attribute name=""obs_fridayclosingtime"" />
                          <attribute name=""obs_code"" />
                          <attribute name=""obs_backuptime"" />
                          <attribute name=""obs_closedstore"" />
                          <attribute name=""obs_storeopeningtime"" />
                          <attribute name=""obs_storeclosingtime"" />
                          <filter type=""and"">
                            <condition attribute='obs_code' operator='in' value=''>
                             {string.Join(" ", codes.Select(x => @$"<value>{Uri.EscapeDataString(HttpUtility.HtmlEncode(x.Trim()))}</value>"))}
                            </condition>
                            <condition attribute=""statecode"" operator=""eq"" value=""0"" />
                          </filter>
                        </entity>
                      </fetch>";
            #endregion

            #region Fetch Stores
            var entities = (await GetList(new Store().WebApiLogicalName, fetchXml)).ToList();
            #endregion

            return entities;
        }


        /// <summary>
        /// Retrieves a list of stores by their codes using FetchXML.
        /// </summary>
        /// <param name="codes">The list of store codes to retrieve.</param>
        /// <returns>A list of stores that match the provided codes.</returns>
        public async Task<IList<Store>> GetStoreList()
        {
            #region Construct FetchXML
            var fetchXml = $@"<fetch version=""1.0"" output-format=""xml-platform"" mapping=""logical"" distinct=""false"">
                              <entity name=""obs_store"">
                              <attribute name=""obs_storeid"" />
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
                              <attribute name=""obs_fridayopeningtime"" />
                              <attribute name=""obs_fridayclosingtime"" />
                              <attribute name=""obs_code"" />
                              <attribute name=""obs_backuptime"" />
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
                              <filter type=""and"">
                                <condition attribute=""statecode"" operator=""eq"" value=""0"" />
                              </filter>
                            </entity>
                      </fetch>";
            #endregion

            #region Fetch Stores
            var entities = (await GetListMoreThan5000(new Store().WebApiLogicalName, fetchXml)).ToList();
            #endregion

            return entities;
        }

    }
}
