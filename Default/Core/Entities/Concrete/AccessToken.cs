using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LCW.Core.LCW.Entities.Concrete
{
    
  //  [Serializable]
    public class AccessToken
    {
        private static readonly TimeSpan Threshold = new(0, 5, 0);

        [JsonProperty("access_token")]
      //  [DataMember(Name = "access_token")]
        public string Token { get; set; }

        [JsonProperty("refresh_token")]
      //  [DataMember(Name = "refresh_token")]
        public string RefreshToken { get; set; }

        [JsonProperty("expires_in")]
     //   [DataMember(Name = "expires_in")]
        public int ExpiresIn { get; set; }

        public DateTime Expires { get; set; }

        [JsonIgnore]
        public bool IsExpired
        {
            get
            {
                return (Expires - DateTime.UtcNow).TotalSeconds <= Threshold.TotalSeconds;
            }
        }

    }
}
