namespace CosmosDb.Logging;
internal class BatchOperations
{
    private record Product(string id, string name, string categoryId);

    readonly CosmosClient client;

    public BatchOperations(CosmosClient client) => this.client = client;

    public async Task Execute()
    {
        Database database = await client.CreateDatabaseIfNotExistsAsync("cosmicworks");
        Container container = await database.CreateContainerIfNotExistsAsync("products", "/categoryId", 400);

        Product saddle = new("0120", "Worn Saddle", "9603ca6c-9e28-4a02-9194-51cdb7fea816");
        Product handlebar = new("012A", "Rusty Handlebar", "9603ca6c-9e28-4a02-9194-51cdb7fea816");

        PartitionKey partitionKey = new("9603ca6c-9e28-4a02-9194-51cdb7fea816");

        TransactionalBatch batch = container.CreateTransactionalBatch(partitionKey)
            .CreateItem<Product>(saddle)
            .CreateItem<Product>(handlebar);

        using TransactionalBatchResponse responseCreate = await batch.ExecuteAsync();

        Console.WriteLine($"Status:\t{responseCreate.StatusCode}");



        Product light = new("012B", "Flickering Strobe Light", "9603ca6c-9e28-4a02-9194-51cdb7fea816");
        Product helmet = new("012C", "New Helmet", "0feee2e4-687a-4d69-b64e-be36afc33e74");

        batch = container.CreateTransactionalBatch(partitionKey)
            .CreateItem<Product>(light)
            .CreateItem<Product>(helmet);

        using TransactionalBatchResponse responseError = await batch.ExecuteAsync();

        Console.WriteLine($"Status:\t{responseError.StatusCode}");
    }
}
