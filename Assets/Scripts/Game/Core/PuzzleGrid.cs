
using System;

namespace Everest.PuzzleGame
{
    [Serializable]
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
        public event Action<ITileView, ITileView>               onShuffle;
        public event Action<ITileView, ITileView, bool>         onSwap;
        public event Action<ITileView>                          onDestroy;
        public event Action                                     onShffleComplete;

        public int                                              Size { get; }
        public float                                            TileSpacing { get; }
        public float                                            Width { get; }
        public float                                            Height { get; }
        public float                                            CurrentTileSize { get; private set; }

        private int                                             emptyTileRow = -1;
        private int                                             emptyTileCol = -1;
        private int                                             fixedEmptyRow;
        private int                                             fixedEmptyCol;

        public int                                              TileCount { get => Tiles.Length; }
        public int                                              Length { get => Tiles.GetLength(1); }
        public int[]                                            Values { get; private set; }
        public ITile[,]                                         Tiles { get; private set; }
        public TileTransform[,]                                 Positions { get; private set; }

        public PuzzleGrid(int size, float tileSpace, float width, float height)
        {
            Size = size;
            Width = width;
            Height = height;
            TileSpacing = tileSpace;
            fixedEmptyRow = fixedEmptyCol = Size - 1;

            float padding = 10f;
            float maxTileWidth = (Width - (Size - 1) * TileSpacing - (4 * padding)) / Size;
            float maxTileHeight = (Height - (Size - 1) * TileSpacing - (4 * padding)) / Size;
            CurrentTileSize = GetMinValue(maxTileWidth, maxTileHeight);
        }

        #region Public Methods

        public void Init()
        {
            GenerateDefaultGrid(Size);
        }

        public void AddTile(ITileView tile, int row, int col, int value, bool isEmpty)
        {
            if(Tiles == null)
                Tiles = new TileData[Size, Size];
            
            if(Values == null)
                Values = new int[Size * Size];

            var position = GetTilePosition(row, col);
            Tiles[row, col] = new TileData(value, tile, isEmpty);

            int index = row * Size + col;
            Values[index] = value;
        }

        public ITileView GetTile(int row, int column)
        {
            if (!IsIndicesValid(row, column))
                throw new System.IndexOutOfRangeException();
            return Tiles[row, column].Tile;
        }

        public ITile GetTileData(int row, int column)
        {
            if (!IsIndicesValid(row, column))
                throw new System.IndexOutOfRangeException();
            return Tiles[row, column];
        }

        public TileTransform GetTilePosition(int row, int col)
        {
            if (!IsIndicesValid(row, col))
                throw new System.IndexOutOfRangeException();
            return Positions[row, col];
        }

        public bool IsIndicesValid(int row, int col)
        {
            return ((row >= 0 && row <= Size - 1) &&
                    (col >= 0 && col <= Size - 1));
        }

        public (int neighbourRow, int neighbourCol) GetNeighbourEmptyTile(int row, int col)
        {
            //top neighbour
            int neighbourRow = row - 1;
            int neighbourCol = col;
            if (IsTileEmpty(neighbourRow, neighbourCol))
                return (neighbourRow, neighbourCol);
            //bottom neighbour
            neighbourRow = row + 1;
            neighbourCol = col;
            if (IsTileEmpty(neighbourRow, neighbourCol))
                return (neighbourRow, neighbourCol);
            //left neighbour
            neighbourRow = row;
            neighbourCol = col - 1;
            if (IsTileEmpty(neighbourRow, neighbourCol))
                return (neighbourRow, neighbourCol);
            //right neighbour
            neighbourRow = row;
            neighbourCol = col + 1;
            if (IsTileEmpty(neighbourRow, neighbourCol))
                return (neighbourRow, neighbourCol);
            return (-1, -1);
        }

        public void Destroy()
        {
            for (int i = 0; i < Size; i++)
                for (int j = 0; j < Size; j++)
                    onDestroy?.Invoke(Tiles[i, j].Tile);

            ResetParams();
        }

        public void SwapTiles(int firstRow, int firstCol, int secondRow, int secondCol, bool tileAnimation, bool skip = false)
        {
            var firstTile = Tiles[firstRow, firstCol];
            var secondTile = Tiles[secondRow, secondCol];

            if (!skip && !firstTile.IsEmpty && !secondTile.IsEmpty) return;

            SwapValues(firstRow * Size + firstCol, secondRow * Size + secondCol);
            SwapTileViews(firstRow, firstCol, secondRow, secondCol);
            InitEmptyTile(firstRow, firstCol);
            InitEmptyTile(secondRow, secondCol);
            onSwap?.Invoke(firstTile.Tile, secondTile.Tile, tileAnimation);
        }

        public void Shuffle()
        {
            var random = new Random();
            int lengthRow = Tiles.GetLength(1);

            for (int i = TileCount - 1; i > 0; i--)
            {
                int firstTileRow = i / lengthRow;
                int firstTilecol = i % lengthRow;

                int j = random.Next(i + 1);
                int secondTileRow = j / lengthRow;
                int secondTileCol = j % lengthRow;

                SwapTiles(firstTileRow, firstTilecol, secondTileRow, secondTileCol, false, true);
                InitEmptyTile(firstTileRow, firstTilecol);
                InitEmptyTile(secondTileRow, secondTileCol);
            }

            TryPuzzleSolvable();
            onShffleComplete?.Invoke();
        }

        public bool IsTileEmpty(int row, int col)
        {
            if (IsIndicesValid(row, col))
                return GetTileData(row, col).IsEmpty;
            return false;
        }

        public bool IsGridSolved() => CountInversions() == 0 && (emptyTileRow == fixedEmptyRow && emptyTileCol == fixedEmptyCol);

        #endregion

        #region Private Methods

        private void TryPuzzleSolvable()
        {
            int inversionCount = CountInversions();

            if (!IsPuzzleSolvable(inversionCount, Size, Size, emptyTileRow + 1))
            {
                if (emptyTileRow == 0 && emptyTileCol <= 1)
                    SwapTiles(Size - 1, Size - 2, Size - 1, Size - 1, false, true);
                else
                    SwapTiles(0, 0, 0, 1, false, true);
            }
        }

        private void InitEmptyTile(int row, int col)
        {
            if (IsTileEmpty(row, col))
            {
                emptyTileRow = row;
                emptyTileCol = col;
            }
        }

        private bool IsPuzzleSolvable(int inversionCount, int width, int height, int emptyTileRow)
        {
            if (width % 2 == 1)
                return inversionCount % 2 == 0;
            return (inversionCount + height - emptyTileRow) % 2 == 0;
        }

        private void SwapTileViews(int firstRow, int firstCol, int secondRow, int secondCol)
        {
            var temp = Tiles[firstRow, firstCol];
            Tiles[firstRow, firstCol] = Tiles[secondRow, secondCol];
            Tiles[secondRow, secondCol] = temp;
        }

        private void SwapValues(int firstIndex, int secondIndex)
        {
            int tempValue = Values[firstIndex];
            Values[firstIndex] = Values[secondIndex];
            Values[secondIndex] = tempValue;
        }

        private TileTransform CalculateGridStartPosition()
        {
            float padding = 10f;
            //finding top-left corner as the grid tile start position
            float tileSize = CurrentTileSize / 2f;
            return new TileTransform((-Width / 2f) + tileSize + padding * 2, (Height / 2f) - tileSize - padding * 2);
        }

        private int CountInversions()
        {
            var inversions = 0;

            for(int i = 0; i < TileCount - 1; i++)
            {
                for (int j = i + 1; j < TileCount; j++)
                {
                    if(Values[j] != -1 && Values[i] > Values[j]) inversions++;
                }
            }

            return inversions;
        }

        private void GenerateDefaultGrid(int size)
        {
            if(Positions == null)
                Positions = new TileTransform[Size, Size];

            if (Tiles == null)
                Tiles = new ITile[Size, Size];

            float offset = CurrentTileSize + TileSpacing ;
            int lengthRow = Tiles.GetLength(1);
            var start = CalculateGridStartPosition();
            float x = start.x;

            for (int i = 0; i < TileCount; i++)
            {
                int row = i / lengthRow;
                int col = i % lengthRow;
                Positions[row, col] = start;
                start.x += offset;

                if (col == Size - 1)
                {
                    start.x = x;
                    start.y -= offset;
                }
            }
        }

        private void ResetParams()
        {
            Tiles = null;
            Positions = null;
            Values = null;
        }

        private float GetMinValue(float a, float b) => a > b ? b : a;

        #endregion
    }
}
