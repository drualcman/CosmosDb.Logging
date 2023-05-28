namespace CosmosDb.Logging;
internal class PlayWithIndexes
{    

    readonly Database Database;

    public PlayWithIndexes(Database database) => Database = database;

    public async Task Execute() 
    {
        IndexingPolicy policy = new() 
        {
            IndexingMode = IndexingMode.Consistent,
            Automatic = true
        };
        policy.ExcludedPaths.Add(
            new ExcludedPath { Path = "/*" }
        );
        policy.IncludedPaths.Add(
            new IncludedPath{ Path = "/Name/?" }
        ); 
        policy.IncludedPaths.Add(
            new IncludedPath{ Path = "/CategoryName/?" }
        );
        ContainerProperties options = new ("products", "/categoryId");
        options.IndexingPolicy = policy;
        Container container = await Database.CreateContainerIfNotExistsAsync(options);
        Console.WriteLine($"Container Created [{container.Id}]");
    }
}
