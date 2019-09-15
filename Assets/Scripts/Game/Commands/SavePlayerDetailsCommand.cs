using Iniectio.Lite;

namespace Everest.PuzzleGame
{
    public class SavePlayerDetailsCommand : Command<string>
    {
        [Inject] private LoadPlayerResponseSignal       loadPlayerResponseSignal { get; set; }
        [Inject] private IGameUserService               gameUserService { get; set; }
        [Inject] private IPlayer                        player { get; set; }

        public override void Execute(string userName)
        {
            //Debug.Log("Calling---------------");
            player.UpdateUserName(userName);
            //Debug.Log("user " + player.UserName);
            gameUserService.SavePlayer(userName, player.GetSerializableData(), (username, data) => {

                UnityEngine.PlayerPrefs.SetString("UserName", username);
                //Debug.Log(data.UserName);
                player.Init(data);

            }, (ex) => {
                //Debug.Log(ex.Message);
            });
        }
    }
}