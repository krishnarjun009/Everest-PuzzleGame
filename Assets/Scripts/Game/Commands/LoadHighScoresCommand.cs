using Iniectio.Lite;
using System.Linq;

namespace Everest.PuzzleGame
{
    public class LoadHighScoresCommand : Command
    {
        [Inject] private IGameUserService gameUserService { get; set; }
        [Inject] private LoadHighScoresResponseSignal m_LoadHighScoresResponseSignal { get; set; }

        public override void Execute()
        {
            gameUserService.LoadLeaderBoard((data) => {

                if (data != null && data.Count >= 0)
                {
                    var arr = data.Values.ToArray();
                    HelperClass.Sort(arr, 0, data.Count - 1);
                    m_LoadHighScoresResponseSignal.Dispatch(arr);
                }

            }, null);
        }
    }
}