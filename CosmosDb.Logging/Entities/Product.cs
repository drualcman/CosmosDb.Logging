using Newtonsoft.Json;

namespace CosmosDb.Logging.Entities;
internal class Product
{
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }

    public string Name { get; set; }

    [JsonProperty(PropertyName = "categoryId")]
    public string CategoryId { get; set; }

    public double Price { get; set; }

    public List<string> Tags { get; set; }

    [JsonProperty(PropertyName = "ttl", NullValueHandling = NullValueHandling.Ignore)]
    public int? TimeToLive { get; set; }
}
