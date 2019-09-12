using System;
using System.Collections.Generic;

namespace Everest.PuzzleGame
{
    public interface IPlayerData { }

    [Serializable]
    public class PlayerData : IPlayerData
    {
        public string UserName;
        public int Score;
        public int BestScore;
    }

    public interface IPlayer
    {
        string UserName { get; }
        int Score { get; }
        int BestScore { get; }

        void Init(IPlayerData data);
        void UpdateUserName(string username);
        void SaveProgress();
        PlayerData GetSerializableData();
        void UpdateScore(int score);
    }

    public class Player : IPlayer
    {
        public string UserName { get; private set; }
        public int Score { get; private set; }
        public int BestScore { get; private set; }

        public Player() { }

        public void Init(IPlayerData data)
        {
            var d = data as PlayerData;
            UserName = d.UserName;
            Score = d.Score;
            BestScore = d.BestScore;
        }

        public void SaveProgress()
        {
            
        }

        public PlayerData GetSerializableData()
        {
            var data = new PlayerData()
            {
                UserName = this.UserName,
                Score = this.Score,
                BestScore = this.BestScore
            };

            return data;
        }

        public void UpdateUserName(string username)
        {
            UserName = username;
        }

        public void UpdateScore(int score)
        {
            Score += score;
        }
    }
}
