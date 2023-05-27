
const string endpoint = "https://localhost:8081";
const string key = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";

CosmosClientBuilder builder = new (endpoint, key);

builder.AddCustomHandlers(new CosmosLoggingHandler());

CosmosClient client = builder.Build();

Database database = await client.CreateDatabaseIfNotExistsAsync("cosmicworks");
Console.WriteLine($"New Database:\tId: {database.Id}");

Container container = await database.CreateContainerIfNotExistsAsync("products", "/categoryId", 400);
Console.WriteLine($"New Container:\tId: {container.Id}");