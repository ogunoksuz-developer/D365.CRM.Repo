using LCW.Core.Attributes;
using LCW.Core.Exceptions;
using LCW.Core.Extensions;
using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LCW.DataAccess.Concrete.AdoNet
{
    public class ANBaseProvider
    {
        private readonly string _connectionString;

        public ANBaseProvider()
        {
            _connectionString = "";
        }

        public async Task<List<K>> ExecuteReaderToListGeneric<K>(SqlCommand cmd)
        {
            List<K> ret = new List<K>();
            using (SqlConnection cnn = new SqlConnection(_connectionString))
            {
                SqlDataReader dr = null;
                try
                {
                    cmd.Connection = cnn;
                    cnn.Open();
                    cmd.CommandTimeout = 0;

                    dr = await cmd.ExecuteReaderAsync(System.Data.CommandBehavior.CloseConnection);

                    while (dr.Read())
                    {
                        ret.Add(genericFiller<K>(dr));
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                finally
                {
                    if (dr != null)
                    {
                        if (!dr.IsClosed)
                            dr.Close();
                        dr = null;
                        if (cmd.Connection.State == System.Data.ConnectionState.Open)
                            cmd.Connection.Close();
                    }
                }
            }
            return ret;
        }

        protected async Task<object> ExecuteScalar(SqlCommand cmd)
        {
            cmd.Connection = new SqlConnection(_connectionString);
            try
            {
                if (cmd.Connection.State == System.Data.ConnectionState.Closed)
                    cmd.Connection.Open(); cmd.CommandTimeout = 0;
                return await cmd.ExecuteScalarAsync();
            }
            catch (Exception ex)
            {
                throw new SqlServerException(ex.Message);
            }
            finally
            {
                if (cmd != null)
                {
                    if (cmd.Connection != null)
                        if (cmd.Connection.State == System.Data.ConnectionState.Open) { cmd.Connection.Close(); cmd.Connection.Dispose(); cmd.Connection = null; }
                    cmd.Dispose();
                }
                cmd = null;
            }
        }

        public T genericFiller<T>(SqlDataReader dr)
        {
            Type a = typeof(T);

            //if (a.BaseType.Name != typeof(TEntity).Name)
            //    throw new Exception("Base Entity classından inherit değil.");

            object o = Activator.CreateInstance(a);
            foreach (var prop in a.GetProperties())
            {
                if (prop.SetMethod != null)
                {
                    EntityAttributes ea = (EntityAttributes)prop.GetCustomAttribute(typeof(EntityAttributes));
                    if (ea != null)
                        prop.SetValue(o, dr.fillerSetter(ea.SQLPropertyName, prop.PropertyType));
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
}
