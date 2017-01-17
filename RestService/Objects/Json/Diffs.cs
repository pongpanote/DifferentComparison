using System.Runtime.Serialization;

namespace Assignment.RestService.Objects.Json
{
    [DataContract]
    public class Diffs
    {
        [DataMember]
        public int offset { get; set; }

        [DataMember]
        public int length { get; set; }
    }
}