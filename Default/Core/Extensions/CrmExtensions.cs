using LCW.Core.Entities;
using Newtonsoft.Json.Linq;
using PowerApps.Samples;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LCW.Core.Extensions
{
    public static class CrmExtensions
    {
        public static object fillerSetter(this JObject entity, string kolonAdi, Type propType)
        {
            object o = null;
            if (propType == typeof(string))
                o = entity[kolonAdi]?.ToString();
            else if (propType == typeof(Nullable<DateTime>))
                o = entity[kolonAdi] != null ?Convert.ToDateTime(entity[kolonAdi].ToString()) : null;
            else if (propType == typeof(Guid))
                o = entity[kolonAdi] !=null ? new Guid(entity[kolonAdi].ToString()):Guid.Empty;
            else if (propType == typeof(Nullable<Guid>))
                o = entity[kolonAdi] != null ? new Guid(entity[kolonAdi].ToString()) : null;
            return o;
        }

        public static object fillerCrmSetter(this ICrmEntity entity, string kolonAdi, PropertyInfo propertyInfo)
        {

            object o = null;

            Type propType = propertyInfo.PropertyType;

            if (propType == typeof(Guid))
                o = propertyInfo.GetValue(entity);
            else if (propType == typeof(string))
                o = propertyInfo.GetValue(entity);
            else if (propType == typeof(Nullable<bool>))
                o = propertyInfo.GetValue(entity);
            else if (propType == typeof(Nullable<int>))
                o = propertyInfo.GetValue(entity);
            else if (propType == typeof(Nullable<DateTime>))
                o = propertyInfo.GetValue(entity);
            else if (propType == typeof(Nullable<decimal>))
                o = propertyInfo.GetValue(entity);
            else if (propType == typeof(Nullable<double>))
                o = propertyInfo.GetValue(entity);
            else if (propType == typeof(int))
                o = propertyInfo.GetValue(entity);
            else if (propType == typeof(bool))
                o = propertyInfo.GetValue(entity);
            else if (propType == typeof(Nullable<Guid>))
                o = propertyInfo.GetValue(entity);
            else if (propType == typeof(Reference))
            {
                Reference reference = propertyInfo.GetValue(entity) as Reference;
                if (reference != null && reference.Id != Guid.Empty)
                    o = new EntityReference(reference.LogicalName, reference.Id);
            }
            else if (propType == typeof(Option))
            {
               //if property is Option type, we need to get the value of the optionset
            }

            return o;
        }

        public static string getSchemaName(this ICrmEntity item)
        {
            DisplayColumnAttribute c = item.GetType().GetCustomAttribute<DisplayColumnAttribute>();
            return c.DisplayColumn;
        }
    }
}
