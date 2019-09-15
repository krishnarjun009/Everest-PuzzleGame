using System.Collections.Generic;

namespace Everest.PuzzleGame
{
    public struct LeaderBoardUserData
    {
        public string   Username;
        public int      BestScore;
    }

    public class LeaderBoard
    {
        [Newtonsoft.Json.JsonProperty("data")]
        public Dictionary<string, LeaderBoardUserData> data { get; }

        public LeaderBoard()
        {
            data = new Dictionary<string, LeaderBoardUserData>();
        }
    }
}
