using CosmosDb.Logging.Entities;
using static Microsoft.Azure.Cosmos.Container;

namespace CosmosDb.Logging;
internal class ProcessChangeFeedHandlers
{
    readonly CosmosClient Client;

    public ProcessChangeFeedHandlers(CosmosClient client) => Client = client;


    public async Task Execute()
    {
        Database database = Client.GetDatabase("cosmicworks");
        Container leaseContainer = await database.CreateContainerIfNotExistsAsync("productslease", "/categoryId", 400);
        Container sourceContainer = Client.GetContainer("cosmicworks", "products");

        ChangesHandler<Product> handleChanges = async (
            IReadOnlyCollection<Product> changes,
            CancellationToken cancellationToken
        ) =>
        {
            Console.WriteLine($"START\tHandling batch of changes...");
            foreach(Product product in changes)
            {
                await Console.Out.WriteLineAsync($"Detected Operation:\t[{product.Id}]\t{product.Name}");
            }
        };

        ChangeFeedProcessorBuilder builder = sourceContainer.GetChangeFeedProcessorBuilder<Product>(
            processorName: "productsProcessor",
            onChangesDelegate: handleChanges
        );

        ChangeFeedProcessor processor = builder
            .WithInstanceName("consoleApp")
            .WithLeaseContainer(leaseContainer)
            .Build();
        await processor.StartAsync();
        Console.WriteLine($"RUN\tListening for changes...");
        Console.WriteLine("Press any key to stop");
        Console.ReadKey();
        await processor.StopAsync();
    }
}
