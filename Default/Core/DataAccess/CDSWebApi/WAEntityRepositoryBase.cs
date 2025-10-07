using Autofac.Core;
using LCW.Core.Attributes;
using LCW.Core.DataAccess.CDSWebApi.Messages;
using LCW.Core.Entities;
using LCW.Core.Extensions;
using LCW.Core.Utilities.IoC;
using LCW.Core.Utilities.Results;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PowerApps.Samples;
using PowerApps.Samples.Messages;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using MergeRequest = LCW.Core.DataAccess.CDSWebApi.Messages.MergeRequest;

namespace LCW.Core.DataAccess.CDSWebApi
{
    public class WAEntityRepositoryBase<TEntity> : ICrmEntityRepository<TEntity>
     where TEntity : class, ICrmEntity, new()
    {
        private readonly Config config;
        private const string JsonValueKey = "value";


        public WAEntityRepositoryBase()
        {
            Config configParam = App.InitializeApp();
            config = configParam;
        }

        /// <summary>
        /// Creates a record
        /// </summary>
        /// <param name="service">The Service</param>
        /// <param name="entitySetName">The EntitySetName for the table</param>
        /// <param name="record">Contains the data to create the record.</param>
        /// <returns>A reference to the created record.</returns>
        public async Task<Guid> Add(TEntity entity)
        {
            string entitySetName = entity.WebApiLogicalName;
            JObject record = genericCrmFiller(entity);

            CreateRequest request = new(entitySetName: entitySetName, record: record);

            var service = ServiceTool.ServiceProvider.GetService<IHttpClientService>();

            var response = await service.SendAsync<CreateResponse>(request);

            return response.EntityReference.Id.Value;
        }

        public void AddTransactionRequest(List<TEntity> entities)
        {
            throw new NotImplementedException();
        }

        public async Task Associate(TEntity entity, List<Reference> relatedEntities, string relationname)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (relatedEntities == null) throw new ArgumentNullException(nameof(relatedEntities));
            if (relationname == null) throw new ArgumentNullException(nameof(relationname));

            var BaseAddress = new Uri($"{config.Url}/api/data/v{config.Version}/");

            foreach (var relatedEntity in relatedEntities)
            {
                AssociateRequest request = new(baseAddress: BaseAddress, entityWithCollection: new EntityReference(entity.getSchemaName(), entity.Id)
                , collectionName: relationname, entityToAdd: new EntityReference(relatedEntity.LogicalName, relatedEntity.Id));

                var service = ServiceTool.ServiceProvider.GetService<IHttpClientService>();

                await service.SendAsync(request);
            }
        }

        public async Task Disassociate(TEntity entity, List<Reference> relatedEntities, string relationname)
        {

            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (relatedEntities == null) throw new ArgumentNullException(nameof(relatedEntities));
            if (relationname == null) throw new ArgumentNullException(nameof(relationname));

            #region Optimize Connection

            // Change max connections from .NET to a remote service default: 2
            System.Net.ServicePointManager.DefaultConnectionLimit = 65000;
            // Bump up the min threads reserved for this app to ramp connections faster - minWorkerThreads defaults to 4, minIOCP defaults to 4 
            ThreadPool.SetMinThreads(100, 100);
            // Turn off the Expect 100 to continue message - 'true' will cause the caller to wait until it round-trip confirms a connection to the server 
            System.Net.ServicePointManager.Expect100Continue = false;
            // Can decrease overall transmission overhead but can cause delay in data packet arrival
            System.Net.ServicePointManager.UseNagleAlgorithm = false;

            #endregion Optimize Connection

            int recommendedDegreeOfParallelism = 8;

            var parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = recommendedDegreeOfParallelism };

            var service = ServiceTool.ServiceProvider.GetService<IHttpClientService>();

            await Parallel.ForEachAsync(relatedEntities, parallelOptions, async (relatedEntity, token) =>
            {
                DisassociateRequest request = new(entityWithCollection: new EntityReference(entity.getSchemaName(), entity.Id)
                  , collectionName: relationname, entityToRemove: new EntityReference(relatedEntity.LogicalName, relatedEntity.Id));

                await service.SendAsync(request);
            });

        }

        public async Task Delete(TEntity entity)
        {
            EntityReference entityReference = new EntityReference(entity.getSchemaName(), entity.Id);
            string partitionId = null;
            bool strongConsistency = false;
            string eTag = null;

            var service = ServiceTool.ServiceProvider.GetService<IHttpClientService>();

            DeleteRequest request = new(
               entityReference: entityReference,
               partitionId: partitionId,
               strongConsistency: strongConsistency,
               eTag: eTag);

            await service.SendAsync(request: request);
        }

        public async Task Merge(TEntity target, TEntity subOrdinate)
        {
            JObject recordTarget = genericCrmFiller(target);
            JObject recordSubOrdinate = genericCrmFiller(subOrdinate);

            var service = ServiceTool.ServiceProvider.GetService<IHttpClientService>();

            MergeRequest request = new(
                target: recordTarget,
                subOrdinate: recordSubOrdinate);

            await service.SendAsync(request: request);
        }

        public byte[] ExportToExcel(string fetchXml, string layoutXml, Guid viewId)
        {
            throw new NotImplementedException();
        }

        public async Task<TEntity> Get(string logicalName, Guid id, string[] columnSet)
        {
            var service = ServiceTool.ServiceProvider.GetService<IHttpClientService>();

            string query = string.Empty;

            if (columnSet != null && columnSet.Length != 0)
            {
                query = $"?$select={logicalName}id,{string.Join(",", columnSet)}";
            }
            else
            {
                query = $"?$select={logicalName}id";
            }

            JObject retrieved = await service.Retrieve(
                entityReference: new EntityReference(logicalName + "s", id),
                query: query);

            return genericFiller<TEntity>(retrieved);
        }

        public TEntity GetDTos(string logicalName, Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<TEntity>> GetList(string entityName, string fetchXml)
        {
            var ret = new List<TEntity>();
            var service = ServiceTool.ServiceProvider.GetService<IHttpClientService>();

            XDocument fetchXmlQueryDoc = XDocument.Parse(fetchXml);

            var fetchXmlRequest = new FetchXmlRequest(
                entitySetName: entityName,
                fetchXml: fetchXmlQueryDoc,
                includeAnnotations: true);

            var results = await service.FetchXml(fetchXmlRequest);

            var jRetrieveResponse = JObject.Parse(results.Content.ReadAsStringAsync().Result);

            if (jRetrieveResponse != null)
            {
                var result = JsonConvert.DeserializeObject<IList<TEntity>>(jRetrieveResponse[JsonValueKey].ToString());
                return result;
            }

            return ret;
        }

        public async Task<IList<TEntity>> RetriveMultiple(string queryUri)
        {
            var ret = new List<TEntity>();
            var service = ServiceTool.ServiceProvider.GetService<IHttpClientService>();

            RetrieveMultipleResponse activeAccountsSavedQueryIdResponse =
                await service.RetrieveMultiple(
                    queryUri: queryUri);

            var records = activeAccountsSavedQueryIdResponse.Records;

            if (records != null)
            {
                var result = JsonConvert.DeserializeObject<IList<TEntity>>(records[JsonValueKey].ToString());
                return result;
            }

            return ret;
        }

        public async Task<IList<TEntity>> GetListMoreThan5000(string entityName, string fetchXml)
        {
            var ret = new List<TEntity>();
            var service = ServiceTool.ServiceProvider.GetService<IHttpClientService>();

            #region FetchXML paging with paging cookie

            XDocument fetchXmlQueryDoc = XDocument.Parse(fetchXml);

            // Add attribute to set the page size
            fetchXmlQueryDoc.Root.Add(new XAttribute("count", "5000"));

            // Add attribute to set the page
            fetchXmlQueryDoc.Root.Add(new XAttribute("page", "1"));

            int page = 1;

            // Using the same FetchXml
            // Add attribute to set the paging cookie
            fetchXmlQueryDoc.Root.Add(new XAttribute("paging-cookie", ""));

            // Reset the page
            fetchXmlQueryDoc.Root.Attribute("page").Value = page.ToString();

            // Reset the count
            fetchXmlQueryDoc.Root.Attribute("count").Value = "500";

            var fetchXmlRequest = new FetchXmlRequest(
               entitySetName: entityName,
               fetchXml: fetchXmlQueryDoc,
               includeAnnotations: true);

            // Send first request
            FetchXmlResponse cookiePagedContacts = await service.FetchXml(fetchXmlRequest);

            var jRetrieveResponse = JObject.Parse(cookiePagedContacts.Content.ReadAsStringAsync().Result);

            if (jRetrieveResponse != null)
            {
                var result = JsonConvert.DeserializeObject<IList<TEntity>>(jRetrieveResponse[JsonValueKey].ToString());
                ret.AddRange(result);
            }

            // Loop through subsequent requests while more records match criteria
            while (cookiePagedContacts.MoreRecords)
            {

                page++;

                fetchXmlQueryDoc.Root.Attribute("page").Value = page.ToString();

                // Extract the FetchxmlPagingCookie XML document value from the response.
                var cookieDoc = XDocument.Parse(cookiePagedContacts.FetchxmlPagingCookie);

                // Extract the encoded pagingcookie attribute value from the FetchxmlPagingCookie XML document
                string pagingCookie = cookieDoc.Root.Attribute("pagingcookie").Value;

                // Double URL decode the pagingCookie string value
                string decodedPagingCookie = HttpUtility.UrlDecode(HttpUtility.UrlDecode(pagingCookie));

                // Set the paging cookie value in the FetchXML paging-cookie attribute
                fetchXmlQueryDoc.Root.Attribute("paging-cookie").Value = decodedPagingCookie;

                // Send the request
                cookiePagedContacts = await service.FetchXml(new FetchXmlRequest(
                    entitySetName: entityName,
                    fetchXml: fetchXmlQueryDoc,
                    includeAnnotations: true));

                // Output results of subsequent requests

                var jRetrieveResponse1 = JObject.Parse(cookiePagedContacts.Content.ReadAsStringAsync().Result);

                if (jRetrieveResponse1 != null)
                {
                    var result = JsonConvert.DeserializeObject<IList<TEntity>>(jRetrieveResponse1[JsonValueKey].ToString());
                    ret.AddRange(result);
                    if (ret.Count > config.TotalCount) return ret;
                }

            }


            #endregion FetchXML paging with paging cookie

            return ret;
        }

        public async Task Update(TEntity entity)
        {
            EntityReference entityReference = new EntityReference(entity.WebApiLogicalName, entity.Id);
            JObject record = genericCrmFiller(entity);
            bool preventDuplicateRecord = false;
            string partitionId = null;
            string eTag = null;

            var service = ServiceTool.ServiceProvider.GetService<IHttpClientService>();

            UpdateRequest request = new(
                entityReference: entityReference,
                record: record,
                preventDuplicateRecord: preventDuplicateRecord,
                partitionId: partitionId,
                eTag: eTag);

            await service.SendAsync(request: request);
        }

        private static JObject genericCrmFiller(TEntity entity)
        {
            var crmEntity = new JObject();
            Type a = typeof(TEntity);

            foreach (var prop in a.GetProperties())
            {
                if (prop.SetMethod == null || prop.GetCustomAttribute(typeof(EntityAttribute)) is not EntityAttribute ea)
                {
                    continue;
                }

                switch (ea)
                {
                    case { isAliasValue: true }:
                        // if it is an alias value, we need to set the value to the alias name
                        break;

                    case { isPrimaryKey: true }:
                        // if it is a primary key, we need to set the value to the primary key
                        break;

                    case { isAutoNumber: true }:
                        // if it is an auto number, we need to set the value to the auto number
                        break;

                    case { isOptionSet: true }:
                        {
                            var number = (int?)prop.GetValue(entity);
                            crmEntity[ea.PropertyName] = number.HasValue ? number.Value : (int?)null;
                            break;
                        }

                    case { isEntityReference: true }:
                        {
                            var guid = (Guid?)prop.GetValue(entity);
                            crmEntity[ea.PropertyName + "@odata.bind"] = guid != null
                                ? $"/{ea.EntityName}s({entity.fillerCrmSetter(ea.PropertyName, prop)})"
                                : null;
                            break;
                        }

                    case { isMoney: true }:
                        // if it is a money, we need to set the value to the money
                        break;

                    case { isDateTime: true }:
                        {
                            var dateTime = (DateTime?)prop.GetValue(entity);
                            crmEntity[ea.PropertyName] = dateTime?.ToString(ea.dateTimeFormat) ?? null;
                            break;
                        }

                    case { NoCreate: true }:
                        // if it is a no create, we need to set the value to the no create
                        break;

                    case { isActivityPartyCollection: true }:
                        {
                            var listReference = (List<ActivityParty>)prop.GetValue(entity);
                            crmEntity[ea.PropertyName] = listReference != null
                                ? JArray.Parse(JsonConvert.SerializeObject(listReference.Select(item => new TempActivityParty
                                {
                                    Party = $"/{item.PartyIdLogicalName}s({item.PartyId})",
                                    Mask = item.Mask
                                }).ToList()))
                                : null;
                            break;
                        }

                    default:
                        crmEntity.SetPropertyContent(ea.PropertyName, entity.fillerCrmSetter(ea.PropertyName, prop));
                        break;
                }
            }
            return crmEntity;
        }


        private static T genericFiller<T>(JObject entity)
        {
            Type a = typeof(T);

            object o = Activator.CreateInstance(a);
            foreach (var prop in a.GetProperties())
            {
                try
                {
                    if (prop.SetMethod == null || prop.GetCustomAttribute(typeof(EntityAttribute)) is not EntityAttribute ea)
                    {
                        continue;
                    }

                    switch (ea)
                    {
                        case { isEntityReference: true }:
                            {
                                var option = entity[ea.JsonPropertyName];
                                if (option != null && !string.IsNullOrEmpty(option.ToString()) && new Guid(option.ToString()) != Guid.Empty)
                                {
                                    prop.SetValue(o, new Guid(option.ToString()));
                                }
                                break;
                            }

                        case { isAliasValue: true }:
                            // if it is an alias value, we need to set the value to the alias name
                            break;

                        case { isOptionSet: true }:
                            {
                                var option = entity.Value<int?>(ea.PropertyName);
                                if (option != null)
                                    prop.SetValue(o, option.Value);
                                break;
                            }

                        case { isMasked: true }:
                            // if it is a masked, we need to set the value to the masked
                            break;

                        case { isMoney: true }:
                            // if it is a money, we need to set the value to the money
                            break;

                        default:
                            prop.SetValue(o, entity.fillerSetter(ea.PropertyName, prop.PropertyType));
                            break;
                    }


                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(ex.Message);
                }

            }
            return (T)o;
        }


    }
}
