using Iniectio.Lite;

namespace Everest.PuzzleGame
{
    public class OnDragSignal : Signal<UnityEngine.Vector2, SwipeDirection> { }
    public class GameOverSignal : Signal { }
    public class StartGameSignal : Signal { }
    public class RestartGameSignal : Signal { }
    public class LoadPlayerRequestSignal : Signal { }
    public class LoadPlayerResponseSignal : Signal { }
    public class SavePlayerRequestSignal : Signal<string> { }
    public class EnableAskUserNameScreenSignal : Signal { }
    public class GameUpdateSignal : Signal { }
    public class SetupBoardSignal : Signal { }
}
