using Iniectio.Lite;
using UnityEngine;
using DG.Tweening;
using System.Collections;

namespace Everest.PuzzleGame
{
    public class BoardController : View
    {
        [Inject] private OnDragSignal               m_OnDragSignal { get; set; }
        [Inject] private GameOverSignal             m_GameOverSignal { get; set; }
        [Inject] private RestartGameSignal          m_RestartGameSignal { get; set; }
        [Inject] private GameUpdateSignal           m_GameUpdateSignal { get; set; }

        [SerializeField] private Canvas             m_RootCanvas;
        [SerializeField] private int                m_GridSize = 7;
        [SerializeField] private float              m_TileSpacing = 14f;
        [SerializeField] private bool               m_UserInit;

        private PuzzleGrid                          m_Grid;
        private GridTile                            m_TileTemplate;

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
            grid.onShffleComplete += OnShuffleCompleted;
        }

        public override void OnRemove()
        {
            m_OnDragSignal.RemoveListener(OnDragInsideGrid);
            grid.onShuffle -= OnShuffleTiles;
            grid.onShffleComplete -= OnShuffleCompleted;
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

        [Listen(typeof(SetupBoardSignal))]
        private void OnGameStart()
        {
            InitBoardGame();
        }

        private void InitBoardGame()
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
            tile.transform.name = row + " - " + col;
            SetTilePositionInGrid(row, col, tile);
            grid.AddTile(tile, row, col, isEmpty);
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

        private void OnShuffleTiles(ITileView firstTile, ITileView secondTile)
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

            //validate grid
            if (grid.IsGridSolved())
            {
                Debug.Log("Grid Solved");
                m_GameOverSignal.Dispatch();
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
                    var neighbours = HasEmptyNeighbour(row, col);
                    neighbourRow = neighbours.neighbourRow;
                    neighbourCol = neighbours.neighbourCol;
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
            SwapTilesWithAnim(firstTile, secondTile);
            m_GameUpdateSignal.Dispatch();
            //SwapTilesPosition(firstTile, secondTile);
        }

        private (int neighbourRow, int neighbourCol) HasEmptyNeighbour(int row, int col)
        {
            //top neighbour
            int neighbourRow = row - 1;
            int neighbourCol = col;
            if (grid.IsTileEmpty(neighbourRow, neighbourCol))
                return (neighbourRow, neighbourCol);
            //bottom neighbour
            neighbourRow = row + 1;
            neighbourCol = col;
            if (grid.IsTileEmpty(neighbourRow, neighbourCol))
                return (neighbourRow, neighbourCol);
            //left neighbour
            neighbourRow = row;
            neighbourCol = col - 1;
            if (grid.IsTileEmpty(neighbourRow, neighbourCol))
                return (neighbourRow, neighbourCol);
            //right neighbour
            neighbourRow = row;
            neighbourCol = col + 1;
            if (grid.IsTileEmpty(neighbourRow, neighbourCol))
                return (neighbourRow, neighbourCol);
            return (-1, -1);
        }

        Sequence sequence;
        System.Collections.Generic.Stack<Sequence> tweens = new System.Collections.Generic.Stack<Sequence>();

        IEnumerator WaitForTweenCompletion(Sequence seq)
        {
            seq.Play();
            yield return seq.WaitForKill();
            Debug.Log("Tween completed");
            yield return new WaitForEndOfFrame();
            //OnShuffleCompleted();
        }

        private void OnShuffleCompleted()
        {
            if (tweens.Count == 0) return;
            var seq = tweens.Pop();
            StartCoroutine(WaitForTweenCompletion(seq));
        }

        private void SwapTilesWithAnim(GridTile firstTile, GridTile secondTile)
        {
            if(sequence == null)
                sequence = DOTween.Sequence();

            //Debug.Log(firstTile.transform.name + " = " + firstTile.transform.position);
            //Debug.Log(secondTile.transform.name + " = " + secondTile.transform.position);
            sequence.Insert(0f, firstTile.rectTransform.DOMove(secondTile.transform.position, 0.35f)).
                Insert(0f, secondTile.rectTransform.DOMove(firstTile.transform.position, 0.35f)).Play();

        }

        private void SwapTilesPosition(GridTile firstTile, GridTile secondTile)
        {
            var temp = firstTile.transform.position;
            firstTile.transform.position = secondTile.transform.position;
            secondTile.transform.position = temp;


            //if (firstTile.transform.name == secondTile.transform.name) return;
            //var sequence = DOTween.Sequence();

            //Debug.Log(firstTile.transform.name + " = " + firstTile.transform.position);
            //Debug.Log(secondTile.transform.name + " = " + secondTile.transform.position);
            //sequence.Pause().Append(firstTile.rectTransform.DOMove(secondTile.transform.position, 0.35f).Pause()).
            //    Append(secondTile.rectTransform.DOMove(firstTile.transform.position, 0.35f).Pause());

            //tweens.Push(sequence);

            //OnShuffleCompleted();

           // StartCoroutine(WaitForTweenCompletion(firstTile, secondTile));

            ////t1.rectTransform.DOKill();
            ////t2.rectTransform.DOKill();
            //firstTile.rectTransform.DOMove(tem2, 0.35f).OnComplete(() => {


            //});

            //secondTile.rectTransform.DOMove(temp, 0.35f).OnComplete(() => {


            //});

        }

        

        #endregion
    }
}