using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCW.Core.Extensions
{
    public static class NpsqlExtensions
    {
        public static string drToString(this NpgsqlDataReader dr, string kolonAdi)
        {
            if (dr.DataReaderHasColumn(kolonAdi) && dr.isNotNull(kolonAdi))
                return dr[kolonAdi].ToString();
            else
                return string.Empty;
        }

        public static int? drToInt(this NpgsqlDataReader dr, string kolonAdi)
        {
            if (dr.DataReaderHasColumn(kolonAdi) && dr.isNotNull(kolonAdi))
                return (int?)dr[kolonAdi];
            else
                return null;
        }

        public static DateTime? drToDateTime(this NpgsqlDataReader dr, string kolonAdi)
        {
            if (dr.DataReaderHasColumn(kolonAdi) && dr.isNotNull(kolonAdi))
                return (DateTime?)dr[kolonAdi];
            else
                return null;
        }
        public static bool drToBoolean2(this NpgsqlDataReader dr, string kolonAdi)
        {
            bool? v = drToBoolean(dr, kolonAdi);
            if (v == null)
                return false;
            else
                return v.Value;
        }
        public static bool? drToBoolean(this NpgsqlDataReader dr, string kolonAdi)
        {
            if (dr.DataReaderHasColumn(kolonAdi) && dr.isNotNull(kolonAdi))
                return (bool?)dr[kolonAdi];
            else
                return null;
        }
        public static Guid? drToGuid(this NpgsqlDataReader dr, string kolonAdi)
        {
            if (dr.DataReaderHasColumn(kolonAdi) && dr.isNotNull(kolonAdi))
                return (Guid?)dr[kolonAdi];
            else
                return null;
        }
        public static Guid drToGuid2(this NpgsqlDataReader dr, string kolonAdi)
        {
            if (dr.DataReaderHasColumn(kolonAdi) && dr.isNotNull(kolonAdi))
                return (Guid)dr[kolonAdi];
            else
                return Guid.Empty;
        }
        public static double? drToDouble(this NpgsqlDataReader dr, string kolonAdi)
        {
            if (dr.DataReaderHasColumn(kolonAdi) && dr.isNotNull(kolonAdi))
            {
                return Math.Round(((double?)dr[kolonAdi]).Value, 2);
            }
            else
                return null;
        }
        public static decimal? drToDecimal(this NpgsqlDataReader dr, string kolonAdi)
        {
            if (dr.DataReaderHasColumn(kolonAdi) && dr.isNotNull(kolonAdi))
            {
                return Math.Round(((decimal?)dr[kolonAdi]).Value, 2);
            }
            else
                return null;
        }


        public static object fillerSetter(this NpgsqlDataReader dr, string kolonAdi, Type propType)
        {
            if (!dr.DataReaderHasColumn(kolonAdi) || !dr.isNotNull(kolonAdi))
                return null;

            var typeMethodMap = new Dictionary<Type, Func<string, object>>
            {
                { typeof(string), kol => dr.drToString(kolonAdi) },
                { typeof(bool?), kol => dr.drToBoolean(kolonAdi) },
                { typeof(int?), kol => dr.drToInt(kolonAdi) },
                { typeof(DateTime?), kol => dr.drToDateTime(kolonAdi) },
                { typeof(decimal?), kol => dr.drToDecimal(kolonAdi) },
                { typeof(double?), kol => dr.drToDouble(kolonAdi) },
                { typeof(int), kol => dr.drToInt(kolonAdi) },
                { typeof(decimal), kol => dr.drToDecimal(kolonAdi) },
                { typeof(bool), kol => dr.drToBoolean2(kolonAdi) },
                { typeof(Guid), kol => dr.drToGuid2(kolonAdi) },
                { typeof(Guid?), kol => dr.drToGuid(kolonAdi) }
            };

            if (typeMethodMap.TryGetValue(propType, out var method))
                return method(kolonAdi);
            
            return null;
        }


    }
}
