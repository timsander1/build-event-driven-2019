using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

namespace leaderboarddata { 

    class Program
    {
        private static readonly Uri _endpointUri = new Uri("<your-account-connection-string>");
        private static readonly string _primaryKey = "<your-primary-key>";
        public static async Task Main(string[] args)
        {
            using (DocumentClient client = new DocumentClient(_endpointUri, _primaryKey))
            {
                for (int i = 0; i < 2000000; i++)
                {
                    await client.OpenAsync();
                    Uri leaderboardCollection = UriFactory.CreateDocumentCollectionUri("pinball-leaderboard", "leaderboard");

                    var Scores = new Bogus.Faker<Score>()
                  .RuleFor(u => u.score, f => f.Random.Number(0, 10) * f.Random.Number(0, 10))
                  .RuleFor(u => u.playerId, f => f.Name.FirstName())
                  .Generate(100);

                    foreach (var score in Scores)
                    {
                        ResourceResponse<Document> resultEmail = await client.CreateDocumentAsync(leaderboardCollection, score);
                    }
                }
            }
        }
    }
}
