using Newtonsoft.Json;

namespace CosmosDb.Logging.Entities;
internal class Item
{
                    
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }
              
    [JsonProperty(PropertyName = "partitionKey")]
    public string PartitionKey { get; set; }
    public decimal Money { get; set; }

    public bool Bboolean { get; set; }

    public string[] Set { get; set; }

    public double Numbers { get; set; }

    public int Morenumbers { get; set; }

    public List<Product> Onetomany { get; set; }
}
