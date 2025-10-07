using LCW.Core.Attributes;
using LCW.Core.Exceptions;
using LCW.Core.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Threading.Tasks;

namespace LCW.Core.DataAccess.AdoNet
{
    public class ANBaseProvider
    {
        private readonly string _connectionString;

        public ANBaseProvider(string connection)
        {
            _connectionString = connection;
        }

        public async Task<IList<K>> ExecuteReaderToListGeneric<K>(SqlCommand cmd)
        {
            List<K> ret = new List<K>();
            await using (SqlConnection cnn = new SqlConnection(_connectionString))
            {
                SqlDataReader dr = null;
                try
                {
                    cmd.Connection = cnn;
                    await cnn.OpenAsync();
                    cmd.CommandTimeout = 0;

                    dr = await cmd.ExecuteReaderAsync(System.Data.CommandBehavior.CloseConnection);

                    while (await dr.ReadAsync())
                    {
                        ret.Add(genericFiller<K>(dr));
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("An error occurred while executing the SQL command.", ex);
                }

            }
            return ret;
        }

        public async Task ExecuteNonQuery(SqlCommand cmd)
        {
            await using (var cnn = new SqlConnection(_connectionString))
            {
                try
                {
                    cmd.Connection = cnn;
                    await cnn.OpenAsync();
                    cmd.CommandTimeout = 0;

                    await cmd.ExecuteNonQueryAsync();
                }
                catch (SqlException ex)
                {
                    throw new InvalidOperationException("An error occurred while executing the SQL command.", ex);
                }
                finally
                {
                    if (cmd.Connection.State == ConnectionState.Open)
                    {
                        await cmd.Connection.CloseAsync();
                    }
                }
            }
        }

        public async Task BulkCopyInsert<T>(string tableName, List<SqlBulkCopyColumnMapping> mappings, IList<T> entity)
        {
            await using (var cnn = new SqlConnection(_connectionString))
            {
                try
                {
                    await cnn.OpenAsync();
                    using var copy = new SqlBulkCopy(cnn)
                    {
                        BulkCopyTimeout = 120,
                        DestinationTableName = tableName
                    };

                    foreach (var item in mappings)
                    {
                        copy.ColumnMappings.Add(item);
                    }

                    await copy.WriteToServerAsync(entity.ToDataTable());
                }
                catch (SqlException ex)
                {
                    throw new InvalidOperationException("An error occurred while performing bulk copy insert.", ex);
                }
            }

        }

        protected async Task<object> ExecuteScalar(SqlCommand cmd)
        {
            await using (var cnn = new SqlConnection(_connectionString))
            {
                try
                {
                    cmd.Connection = cnn;
                    if (cmd.Connection.State == ConnectionState.Closed)
                    {
                        await cmd.Connection.OpenAsync();
                    }
                    cmd.CommandTimeout = 0;
                    return await cmd.ExecuteScalarAsync();
                }
                catch (SqlException ex)
                {
                    throw new InvalidOperationException("An error occurred while executing the SQL command.", ex);
                }
                finally
                {
                    if (cmd.Connection.State == ConnectionState.Open)
                    {
                        await cmd.Connection.CloseAsync();
                    }
                }
            }
        }

        public static T genericFiller<T>(SqlDataReader dr)
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

        public static DataTable ConvertToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);

            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;

        }


    }

    public static class DataTableHelper
    {
        public static DataTable ToDataTable<T>(this IList<T> data)
        {
            PropertyDescriptorCollection properties =
                TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }
    }
}
