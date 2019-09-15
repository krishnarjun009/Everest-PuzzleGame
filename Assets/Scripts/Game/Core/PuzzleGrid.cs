
using System;

namespace Everest.PuzzleGame
{
    [System.Serializable]
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
        public event Action<ITileView, ITileView> onShuffle;
        public event Action<ITileView, ITileView, bool> onSwap;
        public event Action onShffleComplete;

        public int                              Size { get; }
        public float                            TileSpacing { get; }
        public float                            Width { get; }
        public float                            Height { get; }
        public float                            CurrentTileSize { get; private set; }

        private AVLTree m_AVLTree;
        private Node root;
        private bool m_IsProgressData = false;
        private int emptyTileRow = -1, emptyTileCol = -1;
        private int fixedEmptyRow, fixedEmptyCol;

        public int TileCount { get => Tiles.Length; }
        public int Length { get => Tiles.GetLength(1); }
        public TileTransform[,] Positions { get; private set; }
        public ITile[,] Tiles { get; private set; }
        public int[] Values { get; }

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
            fixedEmptyRow = fixedEmptyCol = Size - 1;
            Tiles = new TileData[Size, Size];
            Positions = new TileTransform[Size, Size];
            int length = Size * Size;
            Values = new int[length];
            m_AVLTree = new AVLTree();
            root = null;
            float padding = 10f;

            // Get the maximum width and height a tile can be for this board without overflowing the container
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
            var position = GetTilePosition(row, col);
            Tiles[row, col] = new TileData(value, tile, isEmpty);

            int index = row * Size + col;
            Values[index] = value;
        }

        private void InitEmptyTile(int row, int col)
        {
            if(IsTileEmpty(row, col))
            {
                emptyTileRow = row;
                emptyTileCol = col;
            }
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

        private bool IsPuzzleSolvable(int inversionCount, int width, int height, int emptyTileRow)
        {
            if (width % 2 == 1)
                return inversionCount % 2 == 0;
            return (inversionCount + height - emptyTileRow) % 2 == 0;
        }

        public void TryPuzzleSolvable()
        {
            int inversionCount = CountInversions();

            UnityEngine.Debug.Log(inversionCount + " Count ");

            if (!IsPuzzleSolvable(inversionCount, Size, Size, emptyTileRow + 1))
            {
                //if one of the swapped tiles is the empty tile
                if (emptyTileRow == 0 && emptyTileCol <= 1)
                    //swapping last two tiles to protect inversion count
                    SwapTiles(Size - 1, Size - 2, Size - 1, Size - 1, false, true);
                else
                    SwapTiles(0, 0, 0, 1, false, true);//swap first two tiles to get even inversion count
            }
        }

        public bool IsGridSolved() => CountInversions() == 0 && (emptyTileRow == fixedEmptyRow && emptyTileCol == fixedEmptyCol);

        public void Destroy()
        {
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    UnityEngine.Object.Destroy((Tiles[i, j].Tile as GridTile).gameObject);
                }
            }

            Tiles = new TileData[Size, Size];
            Positions = new TileTransform[Size, Size];
        }

        public void Disable()
        {
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    Tiles[i, j].Disable();
                }
            }
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

        //Fisher Random algorithm
        public void Shuffle()
        {
            var random = new System.Random();
            int lengthRow = Tiles.GetLength(1);

            for (int i = TileCount - 1; i > 0; i--)
            {
                int i0 = i / lengthRow; // first tile row
                int i1 = i % lengthRow; // first tile column

                int j = random.Next(i + 1); // rndom column index
                int j0 = j / lengthRow; // second tile row
                int j1 = j % lengthRow; // second tile column

                //swapping tiles
                SwapTiles(i0, i1, j0, j1, false, true);
                InitEmptyTile(i0, i1);
                InitEmptyTile(j0, j1);
            }

            onShffleComplete?.Invoke();
        }

        public bool IsTileEmpty(int row, int col)
        {
            if (IsIndicesValid(row, col))
                return GetTileData(row, col).IsEmpty;
            return false;
        }

        #endregion

        #region Private Methods

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
                //if (Values[i] == -1) continue;
                for (int j = i + 1; j < TileCount; j++)
                {
                    if(Values[j] != -1 && Values[i] > Values[j])
                    {
                       // UnityEngine.Debug.Log("Pair "+ Values[i] + "-" + Values[j]);
                        inversions++;
                    }
                }
            }

           //UnityEngine.Debug.Log("Inversin count " + inversions);
            return inversions;
        }

        [Obsolete]
        private int GetInversionCount(int[] values)
        {
            int inversionCount = 0;
            root = null;
            foreach (var value in values)
            {
                if (value == -1) continue;//skip enpty tile
                root = m_AVLTree.Insert(root, value, out int temp);
                inversionCount += temp;
            }

            return inversionCount;
        }

        private void GenerateDefaultGrid(int size)
        {
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

        private float GetMinValue(float a, float b) => a > b ? b : a;

        #endregion
    }
}
