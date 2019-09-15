using Iniectio.Lite;
using UnityEngine;
using DG.Tweening;

namespace Everest.PuzzleGame
{
    public class BoardController : View
    {
        [Inject] private OnDragSignal                               m_OnDragSignal { get; set; }
        [Inject] private GameOverSignal                             m_GameOverSignal { get; set; }
        [Inject] private RestartGameSignal                          m_RestartGameSignal { get; set; }
        [Inject] private GameUpdateSignal                           m_GameUpdateSignal { get; set; }
        [Inject] private SaveUserInLeaderBoardRequestSignal         m_SaveUserInLeaderBoardRequestSignal { get; set; }
        [Inject] private SavePlayerRequestSignal                    m_SavePlayerRequestSignal { get; set; }
        [Inject] private IPlayer                                    m_Player { get; set; }

        [SerializeField] private Canvas                             m_RootCanvas;
        [SerializeField] private int                                m_GridSize = 7;
        [SerializeField] private float                              m_TileSpacing = 14f;
        [SerializeField] private bool                               m_UserInit;

        private PuzzleGrid                                          m_Grid;
        private GridTile                                            m_TileTemplate;
        private Sequence                                            m_Sequence;

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
            m_OnDragSignal.AddListener(OnInputReceived);
            grid.onShuffle += OnShuffleTiles;
            grid.onShffleComplete += OnShuffleCompleted;
            grid.onSwap += OnSwipe;
        }

        public override void OnRemove()
        {
            m_OnDragSignal.RemoveListener(OnInputReceived);
            grid.onShuffle -= OnShuffleTiles;
            grid.onShffleComplete -= OnShuffleCompleted;
            grid.onSwap -= OnSwipe;
        }

        #endregion

        #region Unity Callbacks

        protected override void Start()
        {
            base.Start();
            if (!m_UserInit)
                InitBoardGame();
        }

        protected override void OnDisable()
        {
            
            base.OnDisable();
            grid.Destroy();
            //grid.Disable();
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

        [Listen(typeof(RestartGameSignal))]
        private void OnGameRestart()
        {
            m_Player.Restart();
            grid.Destroy();
            m_SavePlayerRequestSignal.Dispatch(m_Player.UserName);
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
            var values = m_Player.GridValues;

            grid.Init();

            if (values == null)
            {
                GenerateBoard();

                grid.Shuffle();
                grid.TryPuzzleSolvable();
                m_Player.UpdateTiles(grid.Values);
            }
            else
            {
                int lengthRow = grid.Length;
                for(int i = 0; i < values.Length; i++)
                    CreateGridTile(i / lengthRow, i % lengthRow, values[i], values[i] == -1);
            }
        }

        private void GenerateBoard()
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
        }

        private void CreateGridTile(int row, int col, int value = -1, bool isEmpty = false)
        {
            var tile = SetupTile(isEmpty, value);
            tile.transform.name = row + " - " + col;
            SetTilePositionInGrid(row, col, tile);
            grid.AddTile(tile, row, col, value, isEmpty);
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

        private GridTile GetEmptyTile() => Instantiate(m_TileTemplate);

        private void OnShuffleTiles(ITileView firstTile, ITileView secondTile)
        {
            SwapTilesPosition(firstTile as GridTile, secondTile as GridTile);
        }

        private void OnShuffleCompleted()
        {
            //do nothing
        }

        private void OnInputReceived(Vector2 position)
        {
            TryMoveTile(position);
        }

        private void TryMoveTile(Vector2 dragPosition)
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
                    TrySwipe(row, col);
                    break;
                }
            }

            //validate grid
            if (grid.IsGridSolved())
            {
                Debug.Log("Grid Solved");
                m_Player.UpdateBestScore(m_Player.Score);
                m_SaveUserInLeaderBoardRequestSignal.Dispatch();
                m_GameOverSignal.Dispatch();
            }
        }

        private void TrySwipe(int row, int col)
        {
            int neighbourRow = -1;
            int neighbourCol = -1;

            var neighbours = grid.GetNeighbourEmptyTile(row, col);
            neighbourRow = neighbours.neighbourRow;
            neighbourCol = neighbours.neighbourCol;

            //Checking if neighbour indices are out of grid bounds or not
            if (!grid.IsIndicesValid(neighbourRow, neighbourCol))
                return;
            if (!grid.IsTileEmpty(row, col) && !grid.IsTileEmpty(neighbourRow, neighbourCol))
                return;

            grid.SwapTiles(row, col, neighbourRow, neighbourCol, true);

            m_Player.UpdateScore(1);
            m_Player.UpdateTiles(grid.Values as int[]);
            m_SavePlayerRequestSignal.Dispatch(m_Player.UserName);
            m_GameUpdateSignal.Dispatch();
        }

        private void OnSwipe(ITileView firstTile, ITileView secondTile, bool tileAnimation)
        {
            if (!tileAnimation)
                SwapTilesPosition(firstTile as GridTile, secondTile as GridTile);
            else
                SwapTilesWithAnim(firstTile as GridTile, secondTile as GridTile);
        }

        private void SwapTilesWithAnim(GridTile firstTile, GridTile secondTile)
        {
            if(m_Sequence == null)
                m_Sequence = DOTween.Sequence();

            m_Sequence.Insert(0f, firstTile.rectTransform.DOMove(secondTile.transform.position, 0.35f)).
                Insert(0f, secondTile.rectTransform.DOMove(firstTile.transform.position, 0.35f)).Play();
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