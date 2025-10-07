using LCW.Core.Entities;
using Microsoft.Xrm.Sdk;
using PowerApps.Samples.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCW.Core.DataAccess
{

    public interface ICrmEntityRepository<T> where T : class, ICrmEntity, new()
    {
        Task<T> Get(string logicalName, Guid id, string[] columnSet);
        Task<IList<T>> GetList(string entityName,string fetchXml);

        Task<IList<T>> RetriveMultiple(string queryUri);
        Task<IList<T>> GetListMoreThan5000(string entityName, string fetchXml);
        T GetDTos(string logicalName, Guid id);

        Task<Guid> Add(T entity);
        Task Update(T entity);
        Task Delete(T entity);
        Task Merge(T target, T subOrdinate);
        void AddTransactionRequest(List<T> entities);
        byte[] ExportToExcel(string fetchXml, string layoutXml, Guid viewId);

        Task Associate(T entity, List<Reference> relatedEntities, string relationname);

        Task Disassociate(T entity, List<Reference> relatedEntities, string relationname);
    }
}
