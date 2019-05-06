using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Microsoft.Azure.WebJobs.Host;
using System.Threading.Tasks;


namespace MultiContainerPartitioning
{
    public static class MultiContainerFunc
    {
        [FunctionName("MultiContainerFunc")]
        public static async Task Run([CosmosDBTrigger(
            databaseName: "<your-db-name>",
            collectionName: "<source-collection-where-writes-go>",
            ConnectionStringSetting = "CosmosConnection",
            LeaseCollectionName = "<lease-collection>")]IReadOnlyList<Document> changes, [CosmosDB(
                databaseName: "<your-db-name>",
                collectionName: "<target-collection>",
                ConnectionStringSetting = "CosmosConnection")]
                IAsyncCollector<Document> docsToAdd,
            ILogger log)
        {        
            foreach (Document document in changes)
            {
                await docsToAdd.AddAsync(document);
            }
        }
    }
}
