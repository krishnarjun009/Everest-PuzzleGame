using Iniectio.Lite;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Everest.PuzzleGame
{
    public class BoardController : View
    {
        [Inject] private OnDragSignal m_OnDragSignal { get; set; }

        [SerializeField] private Canvas m_RootCanvas;
        [SerializeField] private int m_GridSize = 7;
        [SerializeField] private float m_TileSpacing = 14f;
        [Header("Others")] [SerializeField] private bool m_UserInit;

        private PuzzleGrid m_Grid;
        private GridTile m_TileTemplate;

        private PuzzleGrid grid
        {
            get
            {
                if (m_Grid == null)
                {
                    var trans = transform as RectTransform;
                    m_Grid = new PuzzleGrid(m_GridSize, m_TileSpacing, trans.rect.width, trans.rect.height);
                }
                return m_Grid;
            }
        }

        #region IniectioLite Callbacks

        public override void OnRegister()
        {
            m_OnDragSignal.AddListener(OnDragInsideGrid);
            grid.onShuffle += OnShuffleTiles;
        }

        public override void OnRemove()
        {
            m_OnDragSignal.RemoveListener(OnDragInsideGrid);
            grid.onShuffle -= OnShuffleTiles;
        }

        #endregion

        #region Unity Callbacks

        protected override void Start()
        {
            base.Start();
            if (!m_UserInit)
                InitBoardGame();
        }

        #endregion

        #region Public Methods

        #endregion

        #region Private Methods

        public void InitBoardGame()
        {
            LoadResources();
            SetupTiles();
        }

        private void LoadResources()
        {
            m_TileTemplate = Resources.Load<GridTile>("Prefabs/GridTile");
        }

        private void SetupTiles()
        {
            int size = grid.Size;

            for (int i = 0; i < size; i++)
            {
                int col = i == size - 1 ? size - 1 : size;

                for (int j = 0; j < col; j++)
                    CreateGridTile(i, j, (size * i) + j + 1);
                //setting up last tile data which should be empty tile
                if (i == size - 1)
                    CreateGridTile(i, size - 1, -1, true);
            }

            //shuffle all tiles at final step
            grid.Shuffle();
        }

        private void CreateGridTile(int row, int col, int value = -1, bool isEmpty = false)
        {
            var tile = SetupTile(isEmpty, value);
            SetTilePositionInGrid(row, col, tile);
            grid.AddTile(tile, row, col);
        }

        private GridTile SetupTile(bool isEmpty, int value = -1)
        {
            var tile = GetEmptyTile();
            if (isEmpty) tile.SetEmpty();
            else tile.SetView(value);
            tile.SetColor(isEmpty ? BoardUtils.GetDefaultColor() : BoardUtils.GetRandomColor());
            tile.SetParent(transform, false);
            tile.SetActive(true);
            return tile;
        }

        private void SetTilePositionInGrid(int row, int col, GridTile tile)
        {
            var pos = grid.GetTilePosition(row, col);
            tile.rectTransform.localPosition = new Vector3(pos.x, pos.y, 0f);
            tile.rectTransform.sizeDelta = new Vector2(grid.CurrentTileSize, grid.CurrentTileSize);
        }

        private GridTile GetEmptyTile()
        {
            var tile = Instantiate(m_TileTemplate);
            return tile;
        }

        private void OnShuffleTiles(ITile firstTile, ITile secondTile)
        {
            SwapTilesPosition(firstTile as GridTile, secondTile as GridTile);
        }

        private void OnDragInsideGrid(Vector2 position, SwipeDirection direction)
        {
            TryMoveTile(position, direction);
        }

        private void TryMoveTile(Vector2 dragPosition, SwipeDirection direction)
        {
            int count = grid.TileCount;
            int lengthRow = grid.Length;

            for (int i = 0; i < count; i++)
            {
                int row = i / lengthRow;
                int col = i % lengthRow;

                var tile = grid.GetTileData(row, col);
                var gridTile = tile.Tile as GridTile;
                var tilePosition = gridTile.transform.position;
                float scaleTileSize = grid.CurrentTileSize * m_RootCanvas.scaleFactor * 0.75f;

                //taking boundaries of rect to detect collision whether touch position is on the tile or not
                float top = tilePosition.y + scaleTileSize / 2f;
                float bottom = tilePosition.y - scaleTileSize / 2f;
                float left = tilePosition.x - scaleTileSize / 2f;
                float right = tilePosition.x + scaleTileSize / 2f;

                // Check if the mouse if over this tile
                if (dragPosition.x > left &&
                    dragPosition.x < right &&
                    dragPosition.y > bottom &&
                    dragPosition.y < top)
                {
                    TrySwipe(row, col, direction);
                    break;
                }
            }
        }

        private void TrySwipe(int row, int col, SwipeDirection direction)
        {
            int neighbourRow = -1;
            int neighbourCol = -1;

            switch (direction)
            {
                case SwipeDirection.Top:
                    neighbourRow = row - 1;
                    neighbourCol = col;
                    break;
                case SwipeDirection.Bottom:
                    neighbourRow = row + 1;
                    neighbourCol = col;
                    break;
                case SwipeDirection.Left:
                    neighbourRow = row;
                    neighbourCol = col - 1;
                    break;
                case SwipeDirection.Right:
                    neighbourRow = row;
                    neighbourCol = col + 1;
                    break;
                case SwipeDirection.Auto:
                    break;
            }

            //Checking if neighbour indices are out of grid bounds or not
            if (!grid.IsIndicesValid(neighbourRow, neighbourCol))
                return;

            //swapping tiles such as internal data but not visually like position
            var tiles = grid.SwapTiles(row, col, neighbourRow, neighbourCol);

            //tiles might be null if neighbour tiles are not empty
            if (tiles.firstTile == null || tiles.secondTile == null) return;

            var firstTile = tiles.firstTile as GridTile;
            var secondTile = tiles.secondTile as GridTile;
            //swapping positions for visual update
            SwapTilesPosition(firstTile, secondTile);
        }

        private void HasEmptyNeighbour(int row, int col)
        {
            //top neighbour
            int neighbourRow = row - 1;
            int neighbourCol = col;
        }

        private void SwapTilesPosition(GridTile firstTile, GridTile secondTile)
        {
            var temp = firstTile.transform.position;
            firstTile.transform.position = secondTile.transform.position;
            secondTile.transform.position = temp;
        }

        #endregion
    }
}