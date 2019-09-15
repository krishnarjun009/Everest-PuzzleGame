using System;

namespace Everest.PuzzleGame
{
    [Serializable]
    public class PlayerData
    {
        public string                       UserName;
        public int                          Score;
        public int                          BestScore;
        public int                          EmptyTileIndex;
        public int[]                        Values;
    }

    public interface IPlayer
    {
        string                              UserName { get; }
        int                                 Score { get; }
        int                                 BestScore { get; }
        int                                 EmptyTileIndex { get; }
        int[]                               GridValues { get; }

        void Init(PlayerData data);
        void UpdateUserName(string username);
        PlayerData GetSerializableData();
        void UpdateTiles(int[] values);
        void UpdateScore(int score);
        void UpdateBestScore(int currentScore);
        void Restart();
    }

    public class Player : IPlayer
    {
        public string                       UserName { get; private set; }
        public int                          Score { get; private set; }
        public int                          BestScore { get; private set; }
        public int                          EmptyTileIndex { get; private set; }
        public int[]                        GridValues { get; private set; }

        public Player() { }

        public void Init(PlayerData data)
        {
            var d = data as PlayerData;
            UserName = d.UserName;
            Score = d.Score;
            BestScore = d.BestScore;
            GridValues = d.Values;
            EmptyTileIndex = d.EmptyTileIndex;
        }

        public PlayerData GetSerializableData()
        {
            return new PlayerData()
            {
                UserName = this.UserName,
                Score = this.Score,
                BestScore = this.BestScore,
                Values = this.GridValues,
            };
        }

        public void UpdateUserName(string username)
        {
            UserName = username;
        }

        public void UpdateScore(int score)
        {
            Score += score;
        }

        public void UpdateBestScore(int currentScore)
        {
            if (BestScore == 0 || BestScore > currentScore)
                BestScore = currentScore;
        }

        public void UpdateTiles(int[] values)
        {
            GridValues = values;
        }

        public void Restart()
        {
            GridValues = null;
            Score = 0;
        }
    }
}
