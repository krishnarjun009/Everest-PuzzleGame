
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
        int             Value { get; }
        ITileView       Tile { get; }
        bool            IsEmpty { get; }

        bool IsTileSolved();
        void SetCurrentRow(int row);
        void SetCurrentColumn(int col);
        void Disable();
    }

    public interface ITileView
    {
        void Disable();
    }
}
