﻿
namespace Everest.PuzzleGame
{
    public struct TileTransform
    {
        public float x;
        public float y;

        public TileTransform(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public class PuzzleGrid
    {
        public event System.Action<ITileView, ITileView> onShuffle;

        private ITile[,]                        m_Tiles;
        private TileTransform[,]                m_TilePositions;

        public int                              Size { get; }
        public float                            TileSpacing { get; }
        public float                            Width { get; }
        public float                            Height { get; }
        public float                            CurrentTileSize { get; private set; }

        public int TileCount { get => m_Tiles.Length; }
        public int Length { get => m_Tiles.GetLength(1); }

        public int ActiveTileCount
        {
            get
            {
                int count = 0;
                for (int i = 0; i < Size; i++)
                {
                    for (int j = 0; j < Size; j++)
                    {
                        if (GetTileData(i, j) != null)
                            count++;
                    }
                }
                return count;
            }
        }

        public PuzzleGrid(int size, float tileSpace, float width, float height)
        {
            Size = size;
            Width = width;
            Height = height;
            TileSpacing = tileSpace;
            m_Tiles = new TileData[Size, Size];
            m_TilePositions = new TileTransform[Size, Size];
            Init();
        }

        #region Public Methods

        public void AddTile(ITileView tile, int expectedRow, int expectedCol, bool isEmpty)
        {
            //try throw exception
            var position = GetTilePosition(expectedRow, expectedCol);
            var tileData = new TileData(expectedRow, expectedCol, tile, position.x, position.y, isEmpty);
            m_Tiles[expectedRow, expectedCol] = tileData;
        }

        public ITileView GetTile(int row, int column)
        {
            if (!IsIndicesValid(row, column))
                throw new System.IndexOutOfRangeException();
            return m_Tiles[row, column].Tile;
        }

        public ITile GetTileData(int row, int column)
        {
            if (!IsIndicesValid(row, column))
                throw new System.IndexOutOfRangeException();
            return m_Tiles[row, column];
        }

        public TileTransform GetTilePosition(int row, int col)
        {
            if (!IsIndicesValid(row, col))
                throw new System.IndexOutOfRangeException();
            return m_TilePositions[row, col];
        }

        public bool IsIndicesValid(int row, int col)
        {
            return ((row >= 0 && row <= Size - 1) &&
                    (col >= 0 && col <= Size - 1));
        }

        public bool IsTileEmpty(int row, int col)
        {
            if (IsIndicesValid(row, col))
                return GetTileData(row, col).IsEmpty;
            return false;
        }

        public bool IsGridSolved()
        {
            int tilesSolved = 0;
            bool possible = true;
            for(int i = 0; i < Size; i++)
            {
                for(int j = 0; j < Size; j++)
                {
                    if (GetTileData(i, j).IsTileSolved())
                        tilesSolved++;
                    else
                    {
                        possible = false;
                        break;
                    }
                }
                if (!possible) break;
            }

            return tilesSolved == TileCount;
        }

        public (ITileView firstTile, ITileView secondTile) SwapTiles(int firstRow, int firstCol, int secondRow, int secondCol, bool skip = false)
        {
            var firstTile = m_Tiles[firstRow, firstCol];
            var secondTile = m_Tiles[secondRow, secondCol];

            if (!skip && !firstTile.IsEmpty && !secondTile.IsEmpty) return (null, null);

            //swap rows
            int temp = firstTile.Row;
            firstTile.SetCurrentRow(secondRow);
            secondTile.SetCurrentRow(temp);

            //swap columns
            temp = firstTile.Column;
            firstTile.SetCurrentColumn(secondCol);
            secondTile.SetCurrentColumn(temp);

            //swap tiles indices in array
            var tempTile = m_Tiles[firstRow, firstCol];
            m_Tiles[firstRow, firstCol] = m_Tiles[secondRow, secondCol];
            m_Tiles[secondRow, secondCol] = tempTile;

            return (firstTile.Tile, secondTile.Tile);
        }

        public void Shuffle()
        {
            var random = new System.Random();
            int lengthRow = m_Tiles.GetLength(1);

            for (int i = TileCount - 1; i > 0; i--)
            {
                int i0 = i / lengthRow; // first tile row
                int i1 = i % lengthRow; // first tile column

                //int j0 = UnityEngine.Random.Range(0, Size);
                //int j1 = UnityEngine.Random.Range(0, Size); ;
                int j = random.Next(i + 1); // rndom column index
                int j0 = j / lengthRow; // second tile row
                int j1 = j % lengthRow; // second tile column

                //swapping tiles
                var tiles = SwapTiles(i0, i1, j0, j1, true);
                //invoking event to update visually in monobehaviours
                onShuffle?.Invoke(tiles.firstTile, tiles.secondTile);
            }
        }

        #endregion

        #region Private Methods

        private void Init()
        {
            // Get the maximum width and height a tile can be for this board without overflowing the container
            float maxTileWidth = (Width - (Size - 1) * TileSpacing) / Size;
            float maxTileHeight = (Height - (Size - 1) * TileSpacing) / Size;
            CurrentTileSize = GetMinValue(maxTileWidth, maxTileHeight);
            GenerateGrid(Size);
        }

        private TileTransform CalculateGridStartPosition()
        {
            float tileSize = CurrentTileSize / 2f;
            return new TileTransform((-Width / 2f) + tileSize, (Height / 2f) - tileSize);
        }

        private void GenerateGrid(int size)
        {
            float offset = CurrentTileSize + TileSpacing ;
            int lengthRow = m_Tiles.GetLength(1);
            var start = CalculateGridStartPosition();
            float x = start.x;

            for (int i = 0; i < TileCount; i++)
            {
                int row = i / lengthRow;
                int col = i % lengthRow;
                m_TilePositions[row, col] = start;
                start.x += offset;

                if (col == Size - 1)
                {
                    start.x = x;
                    start.y -= offset;
                }
            }
        }

        private float GetMinValue(float a, float b) => a > b ? b : a;

        #endregion
    }
}
