
using Newtonsoft.Json;

namespace Everest.PuzzleGame
{
    public class TileData : ITile
    {
        public int          Value { get; private set; }
        public bool         IsEmpty { get;}
        public ITileView    Tile { get; }

        public TileData(int value, ITileView tile, bool isEmpty)
        {
            Value = value;
            Tile = tile;
            IsEmpty = isEmpty;
        }
    }
}
