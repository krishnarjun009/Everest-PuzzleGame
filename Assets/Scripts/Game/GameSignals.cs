using Iniectio.Lite;

namespace Everest.PuzzleGame
{
    public class OnDragSignal : Signal<UnityEngine.Vector2, SwipeDirection> { }
    public class GameOverSignal : Signal { }
    public class RestartGameSignal : Signal { }
}
