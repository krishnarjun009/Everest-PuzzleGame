using Iniectio.Lite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Everest.PuzzleGame
{
    public class OnDragSignal : Signal<UnityEngine.Vector2, SwipeDirection> { }
    public class GameOverSignal : Signal { }
    public class RestartGameSignal : Signal { }
}
