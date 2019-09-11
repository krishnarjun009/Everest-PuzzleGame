
namespace Everest.PuzzleGame
{
    public class TileData : ITile
    {
        public int          Row { get; private set; }
        public int          Column { get; private set; }
        public float        X { get; private set; }
        public float        Y { get; private set; }
        public int          ExpectedRow { get;  }
        public int          ExpectedColumn { get; }
        public ITileView    Tile { get; }
        public bool         IsEmpty { get;}

        public TileData(int row, int col, ITileView tile, float x, float y, bool isEmpty)
        {
            Row = ExpectedRow = row;
            Column = ExpectedColumn = col;
            IsEmpty = isEmpty;
            Tile = tile;
            X = x;
            Y = y;
        }

        public void SetCurrentRow(int row)
        {
            Row = row;
        }

        public void SetCurrentColumn(int col)
        {
            Column = col;
        }

        public bool IsTileSolved() => (ExpectedRow == Row && ExpectedColumn == Column);
    }
}
