using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ProServ.Shared.Models.Util
{
    public class ResponseWrapper<T>
    {
        [JsonPropertyName("$id")]
        public string Id { get; set; }

        [JsonPropertyName("$values")]
        public List<T> values { get; set; }
    }
}
