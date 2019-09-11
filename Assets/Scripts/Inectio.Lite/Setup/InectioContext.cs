using Everest.PuzzleGame;

namespace Iniectio.Lite
{
    public class InectioContext : RootContext
    {
        public InectioContext() : base()
        {
            
        }
        
		public override void mapBindings()
		{
            injectionBinder.Map<OnDragSignal>().ToSingle();
            injectionBinder.Map<GameOverSignal>().ToSingle();
            injectionBinder.Map<RestartGameSignal>().ToSingle();
        }
	}
}
