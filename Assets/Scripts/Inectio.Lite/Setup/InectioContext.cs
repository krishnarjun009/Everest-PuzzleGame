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
            injectionBinder.Map<LoadPlayerResponseSignal>().ToSingle();
            injectionBinder.Map<IGameUserService, GameUserService>().ToSingle();
            injectionBinder.Map<IPlayer, Player>().ToSingle();
            injectionBinder.Map<AsyncLoaderController>();
            injectionBinder.Map<EnableAskUserNameScreenSignal>();
            injectionBinder.Map<StartGameSignal>();
            injectionBinder.Map<GameUpdateSignal>();
            injectionBinder.Map<SetupBoardSignal>();
            injectionBinder.Map<UpdateProgressBarSignal>();
            injectionBinder.Map<EnableMainMenuSignal>();
            injectionBinder.Map<LoadHighScoresResponseSignal>();
            injectionBinder.Map<EnableHighScoresScreenSignal>();

            commandBinder.Map<SaveUserInLeaderBoardRequestSignal, SaveUserInLeaderBoardCommand>().Pooled();
            commandBinder.Map<LoadHighScoreRequestSignal, LoadHighScoresCommand>().Pooled();
            commandBinder.Map<LoadPlayerRequestSignal, LoadPlayerDetailsCommand>().Pooled();
            commandBinder.Map<SavePlayerRequestSignal, SavePlayerDetailsCommand>().Pooled();
            commandBinder.Map<GameOverRequestSignal, GameOverCommand>().Pooled();
        }
	}
}
