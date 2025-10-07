using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using LCW.Core.Entities;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Web;
using LCW.Core.Common;
using System.Text.RegularExpressions;
using LCW.Core.Entities.Concrete;

namespace LCW.Core.Extensions
{
    public static class SqlExtensions
    {
        public static bool DataReaderHasColumn(this IDataReader reader, string columnName)
        {
            reader.GetSchemaTable().DefaultView.RowFilter = "ColumnName= '" + columnName + "'";
            return (reader.GetSchemaTable().DefaultView.Count > 0);
        }

        public static bool isNotNull(this IDataReader rd, string kolonAdi)
        {
            if (rd[kolonAdi] != null && rd[kolonAdi] != DBNull.Value && rd[kolonAdi].ToString().Trim() != "NULL")
                return true;
            else
                return false;
        }

        public static string drToString(this SqlDataReader dr, string kolonAdi)
        {
            if (dr.DataReaderHasColumn(kolonAdi) && dr.isNotNull(kolonAdi))
                return dr[kolonAdi].ToString();
            else
                return string.Empty;
        }

        public static int? drToInt(this SqlDataReader dr, string kolonAdi)
        {
            if (dr.DataReaderHasColumn(kolonAdi) && dr.isNotNull(kolonAdi))
                return (int?)dr[kolonAdi];
            else
                return null;
        }

        public static DateTime? drToDateTime(this SqlDataReader dr, string kolonAdi)
        {
            if (dr.DataReaderHasColumn(kolonAdi) && dr.isNotNull(kolonAdi))
                return (DateTime?)dr[kolonAdi];
            else
                return null;
        }
        public static bool drToBoolean2(this SqlDataReader dr, string kolonAdi)
        {
            bool? v = drToBoolean(dr, kolonAdi);
            if (v == null)
                return false;
            else
                return v.Value;
        }
        public static bool? drToBoolean(this SqlDataReader dr, string kolonAdi)
        {
            if (dr.DataReaderHasColumn(kolonAdi) && dr.isNotNull(kolonAdi))
                return (bool?)dr[kolonAdi];
            else
                return null;
        }
        public static Guid? drToGuid(this SqlDataReader dr, string kolonAdi)
        {
            if (dr.DataReaderHasColumn(kolonAdi) && dr.isNotNull(kolonAdi))
                return (Guid?)dr[kolonAdi];
            else
                return null;
        }
        public static Guid drToGuid2(this SqlDataReader dr, string kolonAdi)
        {
            if (dr.DataReaderHasColumn(kolonAdi) && dr.isNotNull(kolonAdi))
                return (Guid)dr[kolonAdi];
            else
                return Guid.Empty;
        }
        public static double? drToDouble(this SqlDataReader dr, string kolonAdi)
        {
            if (dr.DataReaderHasColumn(kolonAdi) && dr.isNotNull(kolonAdi))
            {
                return Math.Round(((double?)dr[kolonAdi]).Value, 2);
            }
            else
                return null;
        }
        public static decimal? drToDecimal(this SqlDataReader dr, string kolonAdi)
        {
            if (dr.DataReaderHasColumn(kolonAdi) && dr.isNotNull(kolonAdi))
            {
                return Math.Round(((decimal?)dr[kolonAdi]).Value, 2);
            }
            else
                return null;
        }

        public static object fillerSetter(this SqlDataReader dr, string kolonAdi, Type propType)
        {
            if (!dr.DataReaderHasColumn(kolonAdi) || !dr.isNotNull(kolonAdi))
                return null;

            var typeMethodMap = new Dictionary<Type, Func<string, object>>
            {
                { typeof(string), kolon => (object)dr.drToString(kolon) },
                { typeof(bool?), kolon => (object)dr.drToBoolean(kolon) },
                { typeof(int?), kolon => (object)dr.drToInt(kolon) },
                { typeof(DateTime?), kolon => (object)dr.drToDateTime(kolon) },
                { typeof(decimal?), kolon => (object)dr.drToDecimal(kolon) },
                { typeof(decimal), kolon => (object)dr.drToDecimal(kolon) },
                { typeof(double?), kolon => (object)dr.drToDouble(kolon) },
                { typeof(int), kolon => (object)dr.drToInt(kolon) },
                { typeof(bool), kolon => (object)dr.drToBoolean2(kolon) },
                { typeof(Guid), kolon => (object)dr.drToGuid2(kolon) },
                { typeof(Guid?), kolon => (object)dr.drToGuid(kolon) }
            };

            if (typeMethodMap.TryGetValue(propType, out var method))
            {
                return method(kolonAdi);
            }

            return null;
        }


    }
}
