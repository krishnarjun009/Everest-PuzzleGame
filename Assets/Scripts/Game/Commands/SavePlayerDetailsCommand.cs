using Iniectio.Lite;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Everest.PuzzleGame
{
    public class SavePlayerDetailsCommand : Command<string>
    {
        [Inject] private LoadPlayerResponseSignal loadPlayerResponseSignal { get; set; }
        [Inject] private IGameUserService gameUserService { get; set; }
        [Inject] private IPlayer player { get; set; }

        public override void Execute(string userName)
        {
            Debug.Log("user " + player.UserName);
            player.UpdateUserName(userName);
            gameUserService.SavePlayer(userName, player.GetSerializableData(), (username, data) => {

                PlayerPrefs.SetString("UserName", username);
                if(player.UserName == "Guest")
                    player.Init(data);

                loadPlayerResponseSignal.Dispatch();

            }, null);
        }
    }
}