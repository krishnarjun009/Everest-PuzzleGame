using Iniectio.Lite;
using UnityEngine;

namespace Everest.PuzzleGame
{
    public class LoadPlayerDetailsCommand : Command<string>
    {
        [Inject] private IGameUserService gameUserService { get; set; }
        [Inject] private IPlayer player { get; set; }
        [Inject] private LoadPlayerResponseSignal loadPlayerResponseSignal { get; set; }
        [Inject] private SavePlayerRequestSignal m_SavePlayerRequestSignal { get; set; }

        public override void Execute(string username)
        {
            if (username == player.UserName)
            {
                loadPlayerResponseSignal.Dispatch();
                return;
            }
            gameUserService.LoadPlayer(username, (data) => {

                if (data == null)
                {
                    data = new PlayerData()
                    {
                        UserName = username,
                        Score = 0,
                        BestScore = 0
                    };
                    player.Init(data);
                }
                else
                {
                    player.Init(data);
                }

                loadPlayerResponseSignal.Dispatch();

            }, (ex) => {

                Debug.Log(ex.Message);
                //m_SavePlayerRequestSignal.Dispatch(username);
                loadPlayerResponseSignal.Dispatch();
            });
           
            //gameUserService.SavePlayer("Guest", new PlayerData() { UserName = "Guest", Score = 100 });
        }
    }
}