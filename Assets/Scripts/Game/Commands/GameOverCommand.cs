using Iniectio.Lite;

namespace Everest.PuzzleGame
{
    public class GameOverCommand : Command
    {
        [Inject] private IPlayer player { get; set; }
        [Inject] private GameOverSignal gameOverSignal { get; set; }
        [Inject] private SaveUserInLeaderBoardRequestSignal saveUserInLeaderBoardRequestSignal { get; set; }
        [Inject] private SavePlayerRequestSignal savePlayerRequestSignal { get; set; }

        public override void Execute()
        {
            player.UpdateBestScore(player.Score);
            saveUserInLeaderBoardRequestSignal.Dispatch();
            player.UpdateTiles(null);
            player.Clear();
            savePlayerRequestSignal.Dispatch(player.UserName);
            gameOverSignal.Dispatch();
        }
    }
}