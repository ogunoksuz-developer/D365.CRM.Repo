using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LCW.Core.Entities
{
    [Serializable]
    public class ActivityParty
    {

        [JsonProperty("_partyid_value")]
        public Guid PartyId { get; set; }

        [JsonProperty("participationtypemask")]
        public int Mask { get; set; }

        [JsonProperty("_partyid_value@Microsoft.Dynamics.CRM.associatednavigationproperty")]
        public string AssociatedNavigationProperty { get; set; }

        [JsonProperty("_partyid_value@Microsoft.Dynamics.CRM.lookuplogicalname")]
        public string PartyIdLogicalName { get; set; }
    }

    public class TempActivityParty
    {
        [JsonProperty("partyid_contact@odata.bind")]
        public string Party { get; set; }

        [JsonProperty("participationtypemask")]
        public int Mask { get; set; }
    }
}
