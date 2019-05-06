using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LeaderboardChangeFeed
{
    public class UpdateMaterializedView
    {
        private Leaderboard leaderboard;
        private DocumentClient client;
        private string databaseName = "pinball-leaderboard";
        private string collectionName = "leaderboard";
        private static readonly Uri endpointUri = new Uri("<connection-string>");
        private static readonly string primaryKey = "<primary-key>";

        public UpdateMaterializedView()
        {
            this.leaderboard = new Leaderboard();
            this.client = new DocumentClient(endpointUri, primaryKey);

        }

        public async Task Update(IReadOnlyList<Document> newScoresList)
    {
            foreach (var newScore in newScoresList)
            {
                
                var newScoreEvent = JsonConvert.DeserializeObject<Score>(newScore.ToString());
                
                Uri databaseLink = UriFactory.CreateDatabaseUri(databaseName);
                Uri collectionLink = UriFactory.CreateDocumentCollectionUri(databaseName, collectionName);

                // Update player score

                Uri playerDocumentUri = UriFactory.CreateDocumentUri(databaseName, collectionName, newScoreEvent.playerId);

                try
                {
                    Document playerDoc = await client.ReadDocumentAsync(playerDocumentUri, new RequestOptions { PartitionKey = new PartitionKey(newScoreEvent.playerId) });
                    var existingPlayer = JsonConvert.DeserializeObject<Player>(playerDoc.ToString());
                    Player updatedPlayer = new Player(newScoreEvent.playerId, newScoreEvent.score + existingPlayer.score);
                    await client.UpsertDocumentAsync(collectionLink, updatedPlayer);
                }

                catch (DocumentClientException ex)
                {

                    if (ex.StatusCode == HttpStatusCode.NotFound)
                    {
                        Player newPlayer = new Player(newScoreEvent.playerId, newScoreEvent.score);
                        await client.UpsertDocumentAsync(collectionLink, newPlayer);
                    }

                }


                try
                {
                    Document playerDoc = await client.ReadDocumentAsync(playerDocumentUri, new RequestOptions { PartitionKey = new PartitionKey(newScoreEvent.playerId)});
                    var updatedPlayer = JsonConvert.DeserializeObject<Player>(playerDoc.ToString());

                    Uri leaderboardDocumentUri = UriFactory.CreateDocumentUri(databaseName, collectionName, "leaderboard");
                    Document leaderboardDoc = await client.ReadDocumentAsync(leaderboardDocumentUri, new RequestOptions { PartitionKey = new PartitionKey("leaderboard")});
                    var existingLeaderboardDoc = JsonConvert.DeserializeObject<Leaderboard>(leaderboardDoc.ToString());
                    Leaderboard existingLeaderboard = new Leaderboard(existingLeaderboardDoc.id, existingLeaderboardDoc.playersRanked);

                    if (existingLeaderboard.containsPlayer(updatedPlayer))
                    {
                        existingLeaderboard.updatePlayer(updatedPlayer);
                    }

                    else
                    {
                        existingLeaderboard.playersRanked.Add(updatedPlayer);
                    }

                    Leaderboard updatedLeaderboard = existingLeaderboard;
                    updatedLeaderboard.playersRanked = updatedLeaderboard.sortLeaderboard(updatedLeaderboard.playersRanked);
                    await client.UpsertDocumentAsync(collectionLink, updatedLeaderboard);
                }

                catch (DocumentClientException ex)
                {
                    if (ex.StatusCode == HttpStatusCode.NotFound)
                    {
                        Player newPlayer = new Player(newScoreEvent.playerId, newScoreEvent.score);

                        Leaderboard newLeaderboard = new Leaderboard();
                        newLeaderboard.playersRanked.Add(newPlayer);
                        await client.UpsertDocumentAsync(collectionLink, newLeaderboard);
                    }

                }
            }
        }

    }
}
