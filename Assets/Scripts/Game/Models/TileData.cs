
using Newtonsoft.Json;

namespace Everest.PuzzleGame
{
    [System.Serializable]
    public class TileData : ITile
    {
        [JsonProperty("Row")]                   public int          Row { get; private set; }
        [JsonProperty("Column")]                public int          Column { get; private set; }
        [JsonProperty("X")]                     public float        X { get; private set; }
        [JsonProperty("Y")]                     public float        Y { get; private set; }
        [JsonProperty("ExpectedRow")]           public int          ExpectedRow { get; private set; }
        [JsonProperty("ExpectedColumn")]        public int          ExpectedColumn { get; private set; }
        [JsonProperty("Value")]                 public int          Value { get; private set; }
        [JsonProperty("IsEmpty")]               public bool         IsEmpty { get;}
        [JsonIgnore]                            public ITileView    Tile { get; }

        public TileData(int row, int col, int eRow, int eCol, int value, ITileView tile, float x, float y, bool isEmpty)
        {
            Row = row;
            ExpectedRow = eRow;
            ExpectedColumn = eCol;
            Column = col;
            IsEmpty = isEmpty;
            Tile = tile;
            Value = value;
            X = x;
            Y = y;
        }

        public TileData(int value, ITileView tile, bool isEmpty)
        {
            Value = value;
            Tile = tile;
            IsEmpty = isEmpty;
        }

        public void SetCurrentRow(int row)
        {
            Row = row;
        }

        public void SetCurrentColumn(int col)
        {
            Column = col;
        }

        public void Disable()
        {
            Tile.Disable();
        }

        public bool IsTileSolved() => (ExpectedRow == Row && ExpectedColumn == Column);
    }
}
