
namespace Everest.PuzzleGame
{
    public interface ITile
    {
        int             Value { get; }
        ITileView       Tile { get; }
        bool            IsEmpty { get; }
    }

    public interface ITileView
    {
       
    }
}
