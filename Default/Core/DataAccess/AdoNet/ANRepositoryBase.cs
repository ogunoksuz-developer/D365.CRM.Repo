using LCW.Core.Attributes;
using LCW.Core.DataAccess;
using LCW.Core.Entities;
using LCW.Core.Exceptions;
using LCW.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace LCW.Core.DataAccess.AdoNet
{
    public class AnRepositoryBase<TEntity> : ANBaseProvider,IEntityAsyncRepository<TEntity>
       where TEntity : class, IEntity, new()

    {
        public AnRepositoryBase(string connectionString): base(connectionString)
        {
        }

        public Task<TEntity> Get(Expression<Func<TEntity, bool>> filter)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<TEntity>> GetList(Expression<Func<TEntity, bool>> filter = null)
        {
            #region Sorgu
            string cmdstr = "select * from Account";
            #endregion

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = cmdstr;
            cmd.CommandTimeout = 0;


            return await ExecuteReaderToListGeneric<TEntity>(cmd);
        }

        public Task<TEntity> Add(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public Task Update(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public Task Delete(TEntity entity)
        {
            throw new NotImplementedException();
        }

    }
}
