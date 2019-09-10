using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        public event System.Action<ITile, ITile> onShuffle;

        private TileData[,] m_Tiles;
        private TileTransform[,] m_TilePositions;

        public int Size { get; }
        public float TileSpacing { get; }
        public float Width { get; }
        public float Height { get; }
        public float CurrentTileSize { get; private set; }

        public int TileCount
        {
            get
            {
                if (m_Tiles == null) return 0;
                return m_Tiles.Length;
            }
        }

        public int Length
        {
            get => m_Tiles.GetLength(1);
        }

        public PuzzleGrid(int size, float tileSpace, float width, float height)
        {
            Size = size;
            Width = width;
            Height = height;
            TileSpacing = tileSpace;
            m_Tiles = new TileData[Size, Size];
            m_TilePositions = new TileTransform[Size, Size];
        }

        public void Init()
        {
            // Get the maximum width and height a tile can be for this board without overflowing the container
            float maxTileWidth = (Width - (Size - 1) * TileSpacing) / Size;
            float maxTileHeight = (Height - (Size - 1) * TileSpacing) / Size;

            CurrentTileSize = Mathf.Min(maxTileWidth, maxTileHeight);
            GenerateGrid(Size);
        }

        public void AddTile(ITile tile, int expectedRow, int expectedCol)
        {
            var position = GetTilePosition(expectedRow, expectedCol);
            var tileData = new TileData(expectedRow, expectedCol, tile, position.x, position.y);
            m_Tiles[expectedRow, expectedCol] = tileData;
        }

        public ITile GetTile(int row, int column)
        {
            if (row >= Size || column >= Size) return null;
            return m_Tiles[row, column].Tile;
        }

        public TileData GetTileData(int row, int column)
        {
            if (row >= Size || column >= Size) return null;
            return m_Tiles[row, column];
        }

        public TileTransform GetTilePosition(int row, int col)
        {
            return m_TilePositions[row, col];
        }

        public (ITile firstTile, ITile secondTile) SwapTiles(int firstRow, int firstCol, int secondRow, int secondCol)
        {
            var firstTile = m_Tiles[firstRow, firstCol];
            var secondTile = m_Tiles[secondRow, secondCol];

            if (!firstTile.Tile.IsEmpty() && !secondTile.Tile.IsEmpty()) return (null, null);

            //swap rows
            int temp = firstTile.Row;
            firstTile.SetCurrentRow(secondRow);
            secondTile.SetCurrentRow(temp);

            //swap columns
            temp = firstTile.Column;
            firstTile.SetCurrentRow(secondCol);
            secondTile.SetCurrentRow(temp);

            //swap tiles indices in array
            var tempTile = m_Tiles[firstRow, firstCol];
            m_Tiles[firstRow, firstCol] = m_Tiles[secondRow, secondCol];
            m_Tiles[secondRow, secondCol] = tempTile;

            return (firstTile.Tile, secondTile.Tile);
        }

        private TileTransform CalculateGridStartPosition()
        {
            float tileSize = CurrentTileSize / 2f;
            return new TileTransform((-Width / 2f) + tileSize, (Height / 2f) - tileSize);
        }

        private void GenerateGrid(int size)
        {
            float offset = CurrentTileSize + TileSpacing;
            int lengthRow = m_Tiles.GetLength(1);
            var start = CalculateGridStartPosition();
            float x = start.x;

            for (int i = 0; i < size * size; i++)
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

        //the Fisher-Yates algorithm
        public void Shuffle()
        {
            System.Random random = new System.Random();
            int lengthRow = m_Tiles.GetLength(1);

            for (int i = m_Tiles.Length - 1; i > 0; i--)
            {
                int i0 = i / lengthRow;
                int i1 = i % lengthRow;

                int j = random.Next(i + 1);
                int j0 = j / lengthRow;
                int j1 = j % lengthRow;

                var temp = m_Tiles[i0, i1];
                m_Tiles[i0, i1] = m_Tiles[j0, j1];
                m_Tiles[j0, j1] = temp;
                var tiles = SwapTiles(i0, i1, j0, j1);
                onShuffle?.Invoke(tiles.firstTile, tiles.secondTile);
            }
        }
    }
}
