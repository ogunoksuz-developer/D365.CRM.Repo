using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCW.Core.Extensions
{
    public static class JObjectExtensions
    {
        public static JObject SetPropertyContent(this JObject source, string name, object content)
        {
            var prop = source.Property(name);

            if (prop == null)
            {
                prop = new JProperty(name, content);

                source.Add(prop);
            }
            else
            {
                prop.Value = JContainer.FromObject(content);
            }

            return source;
        }
    }
}
