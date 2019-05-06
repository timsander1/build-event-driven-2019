using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeaderboardChangeFeed
{
   public class Player
    {
        public string id { get; set; }
        public int score { get; set; }

        public Player(string id, int score)
        {
            this.id = id;
            this.score = score;
        }
    }
}
