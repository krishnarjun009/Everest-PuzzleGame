using Iniectio.Lite;
using UnityEngine;

namespace Everest.PuzzleGame
{
    public class SaveUserInLeaderBoardCommand : Command
    {
        [Inject] private IGameUserService gameUserService { get; set; }
        [Inject] private IPlayer player { get; set; }
        [Inject] private LoadPlayerResponseSignal loadPlayerResponseSignal { get; set; }
        [Inject] private SavePlayerRequestSignal m_SavePlayerRequestSignal { get; set; }

        public override void Execute()
        {
            var data = new LeaderBoardUserData() { Username = player.UserName, BestScore = player.BestScore };

            gameUserService.UpdateUserInLeaderBoard(data, (obj) => { }, (ex) => {
                Debug.Log(ex.Message);
            });
        }
    }
}