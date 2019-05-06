using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaderboardChangeFeed
{
    public class Leaderboard
    {
        public string id{ get; set; }
        public List<Player> playersRanked { get; set; }
        public Leaderboard(string id, List<Player> playersRanked)
        {
            this.id = id;
            this.playersRanked = playersRanked;
        }

        public Leaderboard()
        {
            this.id= "leaderboard";
            this.playersRanked = new List<Player>();
        }

        public List<Player> sortLeaderboard(List<Player> playersUnranked)
        {
            List<Player> playersRanked = playersUnranked.OrderBy(i => i.score).Take(10).ToList();
            return playersRanked;
        }

        public Boolean containsPlayer(Player player)
        {
            foreach (Player existingPlayer in this.playersRanked)
            {
                if (player.id == existingPlayer.id)
                {
                    return true;
                }             
            }
            return false;
        }
        public void updatePlayer (Player player)
        {
            foreach(Player existingPlayer in this.playersRanked)
            {
                if(player.id == existingPlayer.id)
                {
                    existingPlayer.score = player.score;
                }
            }
        }
    }
}
