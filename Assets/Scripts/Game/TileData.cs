﻿
namespace Everest.PuzzleGame
{
    public class TileData
    {
        public int Row { get; private set; }
        public int Column { get; private set; }
        public int ExpectedRow { get; private set; }
        public int ExpectedColumn { get; private set; }
        public float X { get; private set; }
        public float Y { get; private set; }
        public UnityEngine.Vector2 Position { get; }
        public ITile Tile { get; private set; }

        public TileData(int row, int col, ITile tile, float x, float y)
        {
            this.Row = row;
            this.Column = col;
            this.ExpectedRow = row;
            this.ExpectedColumn = col;
            this.Tile = tile;
            this.X = x;
            this.Y = y;
            this.Position = new UnityEngine.Vector2(x, y);
        }

        public void SetCurrentRow(int row)
        {
            this.Row = row;
        }

        public void SetCurrentColumn(int col)
        {
            this.Column = col;
        }

        public bool IsTileSolved() => (ExpectedRow == Row && ExpectedColumn == Column);
    }
}
