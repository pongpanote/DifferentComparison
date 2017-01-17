using System;
using System.Collections.Generic;
using System.Linq;
using Assignment.RestService.Objects.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Assignment.RestService.Objects.Converter
{
    public class DiffsConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                new JObject().WriteTo(writer);
            }

            var jToken = JToken.FromObject(value);

            if (jToken.Type != JTokenType.Array)
            {
                jToken.WriteTo(writer);
            }
            else
            {
                var jArray = (JArray)jToken;

                jArray.WriteTo(writer);
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (objectType == typeof(Diffs))
            {
                var jObject = serializer.Deserialize<JObject>(reader);

                if (jObject == null)
                {
                    return null;
                }

                return new Diffs { offset = jObject["offset"].Value<int>(), length = jObject["length"].Value<int>() };
            }

            var jArray = serializer.Deserialize<JArray>(reader);

            if (jArray == null || jArray.Count == 0)
            {
                return null;
            }

            return jArray.Select(item => new Diffs { offset = item["offset"].Value<int>(), length = item["length"].Value<int>() }).ToList();
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(List<Diffs>) || objectType == typeof(Diffs);
        }
    }
}
