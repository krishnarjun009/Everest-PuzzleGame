using Iniectio.Lite;

namespace Everest.PuzzleGame
{
    public class OnDragSignal : Signal<UnityEngine.Vector2> { }
    public class GameOverSignal : Signal { }
    public class GameOverRequestSignal : Signal { }
    public class StartGameSignal : Signal { }
    public class RestartGameSignal : Signal { }
    public class LoadPlayerRequestSignal : Signal<string> { }
    public class LoadPlayerResponseSignal : Signal { }
    public class SavePlayerRequestSignal : Signal<string> { }
    public class EnableAskUserNameScreenSignal : Signal { }
    public class GameUpdateSignal : Signal { }
    public class SetupBoardSignal : Signal { }
    public class UpdateProgressBarSignal : Signal <float> { }
    public class EnableMainMenuSignal : Signal<bool> { }
    public class LoadHighScoreRequestSignal : Signal { }
    public class SaveUserInLeaderBoardRequestSignal : Signal { }
    public class LoadHighScoresResponseSignal : Signal<LeaderBoardUserData[]> { }
    public class EnableHighScoresScreenSignal : Signal<bool> { }
    public class EnableInputSignal : Signal { }
}
