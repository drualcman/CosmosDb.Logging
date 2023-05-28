using CosmosDb.Logging.Entities;
using Microsoft.Azure.Cosmos;

namespace CosmosDb.Logging;
internal class PlayWithQueries
{
    readonly Container Container;

    public PlayWithQueries(Container container) => Container = container;

    public async Task Execute()
    {
        QueryDefinition query = new("SELECT TOP 10 * FROM products p");
        using(FeedIterator<Product> feedIterator = this.Container.GetItemQueryIterator<Product>(
            query,
            null,
            new QueryRequestOptions() { }))
        {
            while(feedIterator.HasMoreResults)
            {
                foreach(var item in await feedIterator.ReadNextAsync())
                {
                    Console.WriteLine($"[{item.Id}]\t{item.Name,35}\t{item.Price,15:C}");
                }
            }
        }

        string sql = "SELECT TOP 100 * FROM products p WHERE p.Price >= @lower AND p.Price <= @upper";
        query = new QueryDefinition(sql)
                .WithParameter("@lower", 500)
                .WithParameter("@upper", 700);

        QueryRequestOptions options = new()
        {
            MaxItemCount = 10
        };

        using(FeedIterator<Product> feedIterator = this.Container.GetItemQueryIterator<Product>(
                query,
                null,
                options))
        {
            int c = 0;
            while(feedIterator.HasMoreResults)
            {
                foreach(var item in await feedIterator.ReadNextAsync())
                {
                    c++;
                    Console.WriteLine($"where [{item.Id}]\t{item.Name,35}\t{item.Price,15:C}");
                }
            }
            Console.WriteLine($"total {c}");
        }
    }
}
