using Castle.Core.Resource;
using LCW.Core.Attributes;
using LCW.Core.Entities;
using LCW.Core.Exceptions;
using LCW.Core.Extensions;
using LCW.Core.Utilities.Results;
using Microsoft.Xrm.Sdk.Extension;
using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static LCW.Core.Enums.Enumarations;

namespace LCW.Core.DataAccess.Npgsql
{
    public class NpsqlRepositoryBase<TEntity> : IEntityAsyncRepository<TEntity>
       where TEntity : class, IEntity, new()
    {
        private readonly string _connectionString;
        protected delegate T FillerToList<out T>(NpgsqlDataReader dr);

        public NpsqlRepositoryBase(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IList<TEntity>> GetList(Expression<Func<TEntity, bool>> filter = null)
        {
            #region Sorgu
            string cmdstr = "select * from Account";
            #endregion

            var cmd = new NpgsqlCommand();
            cmd.CommandText = cmdstr;
            cmd.CommandTimeout = 0;


            return await ExecuteReaderToListGeneric<TEntity>(cmd);
        }

        public Task<TEntity> Get(Expression<Func<TEntity, bool>> filter)
        {
            throw new NotImplementedException();
        }

        public async Task<TEntity> Add(TEntity entity)
        {
            NpgsqlCommand cmd = GetCmd(entity, OperationType.Insert);

            await ExecuteNonQuery(false, cmd, true);

            return entity;
        }

        public async Task AddBulk(List<TEntity> entities)
        {
            var cmdText = entities.Aggregate(
                new StringBuilder(),
                (sb, evamlog) =>
                    sb.AppendLine(@$"
                     {GetCmdText(evamlog)}")
                );

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                try
                {
                    var command = new NpgsqlCommand(cmdText.ToString(), connection);
                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(cmdText + Environment.NewLine + ex.ToString());
                }
            }
        }

        public Task Delete(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public Task Update(TEntity entity)
        {
            throw new NotImplementedException();
        }

        protected async Task<List<K>> ExecuteReaderToList<K>(NpgsqlCommand cmd, FillerToList<K> filler)
        {
            List<K> ret = new List<K>();
            using (NpgsqlConnection cnn = new NpgsqlConnection(_connectionString))
            {
                NpgsqlDataReader dr = null;
                try
                {
                    cmd.Connection = cnn;
                    await cnn.OpenAsync();
                    cmd.CommandTimeout = 0;

                    dr = await cmd.ExecuteReaderAsync(System.Data.CommandBehavior.CloseConnection);

                    while (await dr.ReadAsync())
                    {
                        ret.Add(filler(dr));
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(ex.Message);
                }
                finally
                {
                    if (dr != null)
                    {
                        if (!dr.IsClosed)
                            await dr.CloseAsync();
                        if (cmd.Connection.State == System.Data.ConnectionState.Open)
                            await cmd.Connection.CloseAsync();
                    }
                }
            }
            return ret;
        }

        public async Task<List<K>> ExecuteReaderToListGeneric<K>(NpgsqlCommand cmd)
        {
            List<K> ret = new List<K>();
            using (NpgsqlConnection cnn = new NpgsqlConnection(_connectionString))
            {
                NpgsqlDataReader dr = null;
                try
                {
                    cmd.Connection = cnn;
                    await cnn.OpenAsync();
                    cmd.CommandTimeout = 0;

                    dr = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);

                    while (await dr.ReadAsync())
                    {
                        ret.Add(GenericFiller<K>(dr));
                    }
                }
                catch (Exception ex)
                {
                    throw new SqlServerException(ex.Message);
                }
                finally
                {
                    if (dr != null)
                    {
                        if (!dr.IsClosed)
                            await dr.CloseAsync();
                        if (cmd.Connection.State == ConnectionState.Open)
                            await cmd.Connection.CloseAsync();
                    }
                }
            }
            return ret;
        }

        public List<K> ExecuteDataAdapterToListGeneric<K>(NpgsqlCommand cmd)
        {
            List<K> ret = new List<K>();
            using (NpgsqlConnection cnn = new NpgsqlConnection(_connectionString))
            {
                using (NpgsqlDataAdapter dr = new NpgsqlDataAdapter(cmd))
                {
                    dr.SelectCommand.CommandType = CommandType.Text;

                    DataSet myDataSet = new DataSet();
                    dr.Fill(myDataSet);

                }
            }
            return ret;
        }

        public T GenericFiller<T>(NpgsqlDataReader dr)
        {
            Type a = typeof(T);

            object o = Activator.CreateInstance(a);
            foreach (var prop in a.GetProperties())
            {
                if (prop.SetMethod != null)
                {
                    EntityAttribute ea = (EntityAttribute)prop.GetCustomAttribute(typeof(EntityAttribute));
                    if (ea != null)
                        prop.SetValue(o, dr.fillerSetter(ea.PropertyName, prop.PropertyType));
                }

            }
            return (T)o;

        }

        protected async Task<object> ExecuteScalar(NpgsqlCommand cmd)
        {
            await using (var connection = new NpgsqlConnection(_connectionString))
            {
                cmd.Connection = connection;
                try
                {
                    if (connection.State == ConnectionState.Closed)
                    {
                        await connection.OpenAsync();
                    }
                    cmd.CommandTimeout = 0;
                    return await cmd.ExecuteScalarAsync();
                }
                catch (Exception ex)
                {
                    throw new SqlServerException(ex.Message);
                }
                finally
                {
                    if (cmd.Connection != null && cmd.Connection.State == ConnectionState.Open)
                    {
                        await cmd.Connection.CloseAsync();
                    }
                    await cmd.DisposeAsync();
                }
            }
        }

        public virtual NpgsqlCommand GetCmd(IEntity item, OperationType type)
        {

            NpgsqlCommand cmd = new NpgsqlCommand();
            Type a = item.GetType();

            Dictionary<string, object> columnNames = new Dictionary<string, object>();
            KeyValuePair<string, object> primaryKey = new KeyValuePair<string, object>();

            string schemaName = getSchemaName(item);

            foreach (PropertyInfo prop in a.GetProperties())
            {

                EntityAttribute ec = prop.GetCustomAttribute<EntityAttribute>(false);
                if (ec == null)
                    continue;

                switch (ec)
                {
                    case { isPrimaryKey: true }:
                        primaryKey = new KeyValuePair<string, object>(ec.PropertyName, prop.GetValue(item, null));
                        break;

                    case { isForeignKey: true } when (prop.PropertyType == typeof(int) && Convert.ToInt32(prop.GetValue(item, null)) != 0) ||
                                      prop.PropertyType == typeof(int?) ||
                                      prop.PropertyType == typeof(Guid) ||
                                      prop.PropertyType == typeof(Guid?):
                    case { isForeignKey: false }:
                        columnNames.Add(ec.PropertyName, prop.GetValue(item, null));
                        break;
                }
            }
            string commandText = string.Empty;
            switch (type)
            {
                case OperationType.Insert:
                    commandText = "insert into " + schemaName;

                    string[] insertColumns = columnNames.Keys.ToArray();
                    commandText += " (" + string.Join(",", insertColumns) + ") values (" + string.Join(",", insertColumns.Select(s => "@" + s)) + ")";
                    cmd.CommandText = commandText;

                    foreach (var param in columnNames)
                    {
                        cmd.Parameters.AddWithValue("@" + param.Key, param.Value ?? DBNull.Value);
                    }
                    break;

                case OperationType.Delete:
                    commandText = "DELETE FROM @SchemaName WHERE @PrimaryKeyKey = @PrimaryKey";
                    cmd.CommandText = commandText;
                    cmd.Parameters.AddWithValue("@SchemaName", schemaName);
                    cmd.Parameters.AddWithValue("@PrimaryKeyKey", primaryKey.Key);
                    cmd.Parameters.AddWithValue("@PrimaryKey", primaryKey.Value ?? (object)DBNull.Value);
                    break;

                case OperationType.Update:
                    string[] updateColumns = columnNames.Keys.ToArray();
                    commandText = "update " + schemaName + " set " + string.Join(",", updateColumns.Select(s => s + "=@" + s));
                    commandText += " where " + primaryKey.Key + "=@" + primaryKey.Key;
                    cmd.CommandText = commandText;

                    foreach (var param in columnNames)
                    {
                        cmd.Parameters.AddWithValue("@" + param.Key, param.Value ?? DBNull.Value);
                    }
                    cmd.Parameters.AddWithValue("@" + primaryKey.Key, primaryKey.Value);
                    break;
            }

            return cmd;

        }

        public virtual string GetCmdText(IEntity item)
        {
            Type a = item.GetType();

            Dictionary<string, object> columnNames = new Dictionary<string, object>();

            string schemaName = getSchemaName(item);

            foreach (PropertyInfo prop in a.GetProperties())
            {
                if (prop.GetCustomAttribute<EntityAttribute>(false) is EntityAttribute ec && !ec.isPrimaryKey)
                {
                    object o = prop.GetValue(item, null);

                    if (!ec.isForeignKey ||
                        (ec.isForeignKey &&
                        ((prop.PropertyType == typeof(int) && Convert.ToInt32(o) != 0) ||
                         prop.PropertyType == typeof(int?) ||
                         prop.PropertyType == typeof(Guid) ||
                         prop.PropertyType == typeof(Guid?))))
                    {
                        columnNames.Add(ec.PropertyName, o);
                    }
                }
            }


            StringBuilder commandText = new StringBuilder();

            commandText.Append(" insert into ").Append(schemaName);

            string[] dizi = (from x in columnNames select x.Key).ToArray();

            commandText.Append(" (").Append(string.Join(",", dizi)).Append(')');

            commandText.Append(" values (");

            var values = columnNames
                .Select(param => param.Value == null ? "null" : $"'{param.Value}'")
                .ToArray();

            var result = string.Join(",", values);

            commandText.Append(string.Join(string.Empty, result));

            commandText.Append(");");

            return commandText.ToString();

        }

        public string getSchemaName(IEntity item)
        {
            try
            {
                DisplayColumnAttribute c = item.GetType().GetCustomAttribute<DisplayColumnAttribute>();
                return c.DisplayColumn;
            }
            catch (Exception)
            {
                return string.Empty;
            }

        }

        public async Task<SqlResult> ExecuteNonQuery(bool isTransaction, NpgsqlCommand cmd, bool isInsert)
        {
            var rs = new SqlResult();

            using (NpgsqlConnection cnn = new NpgsqlConnection(_connectionString))
            {
                try
                {
                    cmd.Connection = cnn;
                    cmd.CommandTimeout = 0;
                    if (cmd.Connection.State != ConnectionState.Open)
                        await cmd.Connection.OpenAsync();
                    if (isTransaction)
                    {
                        NpgsqlTransaction trans = await cnn.BeginTransactionAsync();
                        cmd.Transaction = trans;

                    }

                    if (isInsert)
                        await cmd.ExecuteScalarAsync();
                    else
                        await cmd.ExecuteNonQueryAsync();

                    if (isTransaction)
                        await cmd.Transaction.CommitAsync();

                    rs.IsOk = true;
                }
                catch (Exception ex)
                {
                    rs.IsOk = false;
                    rs.ExMessage = ex.ToString();
                    rs.InnerException = ex;

                    if (isTransaction)
                    {
                        try
                        {
                            await cmd.Transaction.RollbackAsync();

                        }
                        catch (Exception ex2)
                        {

                            rs.IsOk = false;
                            rs.ExMessage = ex2.ToString();
                        }
                    }

                }
                finally
                {
                    if (cmd?.Connection?.State == ConnectionState.Open)
                    {
                        await cmd.Connection.CloseAsync();
                        cmd.Connection = null;
                    }

                    if (cmd != null)
                        await cmd.DisposeAsync();
                }
            }

            return rs;
        }

    }
}
