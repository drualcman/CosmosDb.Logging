
using CosmosDb.Logging.Entities;

const string endpoint = "https://localhost:8081";
const string key = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";


CosmosClientBuilder builder = new(endpoint, key);

builder.AddCustomHandlers(new CosmosLoggingHandler());

using CosmosClient client = builder.Build();
         
Database database = await client.CreateDatabaseIfNotExistsAsync("cosmicworks");
Console.WriteLine($"New Database:\tId: {database.Id}");

Container container = await database.CreateContainerIfNotExistsAsync("products", "/categoryId", 400);
Console.WriteLine($"New Container:\tId: {container.Id}");

Product saddle = new()
{
    Id = "027D0B9A-F9D9-4C96-8213-C8546C4AAE71",
    CategoryId = "26C74104-40BC-4541-8EF5-9892F7F03D72",
    Name = "LL Road Seat/Saddle",
    Price = 27.12d,
    Tags = new List<string>
    {
        "brown",
        "weathered"
    }
};


try
{
    await container.CreateItemAsync<Product>(saddle);

    ItemResponse<Product> response = await container.CreateItemAsync<Product>(saddle);

    HttpStatusCode status = response.StatusCode;
    double requestUnits = response.RequestCharge;

    Product item = response.Resource;
}
catch(CosmosException ex) when(ex.StatusCode == HttpStatusCode.Conflict)
{
    // Add logic to handle conflicting ids
    Console.WriteLine($"CosmosException when Conflict: {ex.Message}");

}
catch(CosmosException ex)
{
    // Add general exception handling logic
    Console.WriteLine($"CosmosException: {ex.Message}");

}

string id = "027D0B9A-F9D9-4C96-8213-C8546C4AAE71";
string categoryId = "26C74104-40BC-4541-8EF5-9892F7F03D72";
PartitionKey partitionKey = new(categoryId);

string eTag = string.Empty;    //optimstic concurrency control
try
{

    ItemResponse<Product> response = await container.ReadItemAsync<Product>(id, partitionKey);
    Product saddle1 = response.Resource;
    eTag = response.ETag;        //last update
    
    string formattedName = $"Update Product [${saddle1.Name}] etag {eTag}";
    Console.WriteLine(formattedName);
}
catch(CosmosException ex)
{
    // Add general exception handling logic
    Console.WriteLine($"CosmosException: {ex.Message}");
}

saddle.Price = 35.00d;
try
{     
    ItemRequestOptions options = new ItemRequestOptions { IfMatchEtag = eTag };         //update only if same eTag
    await container.UpsertItemAsync<Product>(saddle, partitionKey, requestOptions: options);    
    Console.WriteLine($"Can save with eTag {eTag}");
}
catch(CosmosException ex)
{
    // Add general exception handling logic
    Console.WriteLine($"CosmosException: {ex.Message}");
}
saddle.Tags = new List<string> { "brown", "new", "crisp" };
saddle.TimeToLive = 1000;
try
{
    await container.UpsertItemAsync<Product>(saddle);
}
catch(CosmosException ex)
{
    // Add general exception handling logic
    Console.WriteLine($"CosmosException: {ex.Message}");
}
try
{
    await container.DeleteItemAsync<Product>(id, partitionKey);
}
catch(CosmosException ex)
{
    // Add general exception handling logic
    Console.WriteLine($"CosmosException: {ex.Message}");
}

BatchOperations batchOperations = new(client);
await batchOperations.Execute();

using BulkOperations bulkOperations = new(endpoint, key);
await  bulkOperations.Execute();