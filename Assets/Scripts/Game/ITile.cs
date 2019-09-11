
namespace Everest.PuzzleGame
{
    public interface ITile
    {
        int             Row { get; }
        int             Column { get; }
        float           X { get; }
        float           Y { get; }
        int             ExpectedRow { get; }
        int             ExpectedColumn { get; }
        ITileView       Tile { get; }
        bool            IsEmpty { get; }

        bool IsTileSolved();
        void SetCurrentRow(int row);
        void SetCurrentColumn(int col);
    }

    public interface ITileView
    {

    }
}
