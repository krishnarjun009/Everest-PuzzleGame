using Iniectio.Lite;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Everest.PuzzleGame
{
    public class LoadPlayerDetailsCommand : Command
    {
        [Inject] private IGameUserService gameUserService { get; set; }
        [Inject] private IPlayer player { get; set; }
        [Inject] private LoadPlayerResponseSignal loadPlayerResponseSignal { get; set; }

        public override void Execute()
        {
            var userName = PlayerPrefs.GetString("UserName", "Guest");
            gameUserService.LoadPlayer(userName, (data) => {

                if (data == null)
                {
                    data = new PlayerData()
                    {
                        UserName = userName,
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

            }, null);
           
            //gameUserService.SavePlayer("Guest", new PlayerData() { UserName = "Guest", Score = 100 });
        }
    }
}