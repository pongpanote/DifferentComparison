using System.Collections.Generic;
using System.Runtime.Serialization;
using Assignment.RestService.Objects.Converter;
using Newtonsoft.Json;

namespace Assignment.RestService.Objects.Json
{
    [DataContract]
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class DiffResult
    {
        [DataMember]
        public string diffResultType { get; set; }

        [DataMember]
        [JsonConverter(typeof(DiffsConverter))]
        public List<Diffs> diffs { get; set; }
    }
}