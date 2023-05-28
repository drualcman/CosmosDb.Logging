using Newtonsoft.Json;

namespace CosmosDb.Logging.Entities;
internal class Item
{
                    
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }
              
    [JsonProperty(PropertyName = "categoryId")]
    public string CategoryId { get; set; }
    public string CategoryName { get; set; }
    public string SKU { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public List<Tag> Tags { get; set; }
}
