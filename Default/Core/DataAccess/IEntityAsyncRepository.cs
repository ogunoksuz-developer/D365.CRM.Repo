using LCW.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LCW.Core.DataAccess
{
    public interface IEntityAsyncRepository<T> where T : class, IEntity, new()
    {
        Task<T> Get(Expression<Func<T, bool>> filter);

        Task<IList<T>> GetList(Expression<Func<T, bool>> filter = null);

        Task<T> Add(T entity);

        Task Update(T entity);

        Task Delete(T entity);
    }
}
