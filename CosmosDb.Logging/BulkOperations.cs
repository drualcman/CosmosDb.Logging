using Bogus;
using CosmosDb.Logging.Entities;

namespace CosmosDb.Logging;
internal class BulkOperations : IDisposable
{
    readonly CosmosClient Client;
    public BulkOperations(string endpoint, string key)
    {
        CosmosClientOptions options = new()
        {
            AllowBulkExecution = true
        };
        Client = new(endpoint, key, options);
    }

    public void Dispose() => Client.Dispose();

    public async Task Execute()
    {
        Container container = Client.GetContainer("cosmicworks", "products");
          
        List<Product> productsToInsert = new Faker<Product>()
          .StrictMode(true)
          .RuleFor(o => o.Id, f => Guid.NewGuid().ToString())
          .RuleFor(o => o.Name, f => f.Commerce.ProductName())
          .RuleFor(o => o.Price, f => Convert.ToDouble(f.Commerce.Price(max: 1000, min: 10, decimals: 2)))
          .RuleFor(o => o.CategoryId, f => f.Commerce.Department(1))
          .RuleFor(o => o.Tags, f => f.Make(1, () => f.Random.Word()))
          .RuleFor(o => o.TimeToLive, f => Convert.ToInt32(f.Random.Int(1, 10)))
          .Generate(25000);

        List<Task> concurrentTasks = new List<Task>();
        foreach(Product product in productsToInsert)
        {
            concurrentTasks.Add(
                container.CreateItemAsync(product, new PartitionKey(product.CategoryId))
            );
        }
        await Task.WhenAll(concurrentTasks);
        Console.WriteLine("Bulk tasks complete");
    }
}
