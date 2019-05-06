using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaderboardChangeFeed
{
    public class Score
    {
        public string id { get; set; }
        public int score { get; set; }

        public string playerId { get; set; }

        public Score(string id, int score, string playerId)
        {
            this.id = id;
            this.score = score;
            this.playerId = playerId;
        }
    }
}

