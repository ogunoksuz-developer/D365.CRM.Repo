using LCW.Core.Attributes;
using LCW.Core.Exceptions;
using LCW.Core.Extensions;
using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LCW.DataAccess.Concrete.Npsql
{
    public class NpsqlBaseProvider
    {
        private readonly string _connectionString;

        public NpsqlBaseProvider()
        {
            _connectionString = ""; 
        }

        public static T genericFiller<T>(NpgsqlDataReader dr)
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
            if (cmd == null)
            {
                throw new ArgumentNullException(nameof(cmd));
            }

            cmd.Connection = new NpgsqlConnection(_connectionString);
            try
            {
                if (cmd.Connection.State == ConnectionState.Closed)
                {
                    await cmd.Connection.OpenAsync();
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
                if (cmd.Connection?.State == ConnectionState.Open)
                {
                    await cmd.Connection.CloseAsync();
                    await cmd.Connection.DisposeAsync();
                    cmd.Connection = null;
                }

                await cmd.DisposeAsync();
            }
        }

        public async Task<List<K>> ExecuteReaderToListGeneric2<K>(NpgsqlCommand cmd)
        {
            List<K> ret = new List<K>();

            if (cmd == null)
            {
                throw new ArgumentNullException(nameof(cmd));
            }

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
                        ret.Add(genericFiller<K>(dr));
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
                        {
                            await dr.CloseAsync();
                        }
                        await dr.DisposeAsync();
                    }

                    if (cmd.Connection?.State == ConnectionState.Open)
                    {
                        await cmd.Connection.CloseAsync();
                        await cmd.Connection.DisposeAsync();
                    }
                }
            }

            return ret;
        }

    }
}
