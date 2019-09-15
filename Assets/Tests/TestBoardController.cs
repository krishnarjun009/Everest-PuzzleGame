using Everest.PuzzleGame;
using NUnit.Framework;
using UnityEngine;

namespace Tests
{
    public class TestBoardController
    {
        PuzzleGrid grid;
        int size = 7;

        [SetUp]
        public void Setup()
        {
            grid = new PuzzleGrid(7, 14f, 900, 900);
        }

        [Test]
        public void TestCreateBoard()
        {
            Assert.AreEqual(grid.TileCount, size * size);
        }


        [Test]
        public void TestIndicesValid()
        {
            int row = 0;
            int col = 0;

            var isValid = grid.IsIndicesValid(row, col);

            Assert.AreEqual(isValid, true);
        }

        [Test]
        public void TestIndicesAreNotValid()
        {
            int row = 0;
            int col = -1;

            var isValid = grid.IsIndicesValid(row, col);

            Assert.AreNotEqual(isValid, true);
        }

        [Test]
        public void TestAddTile()
        {
            
        }

        [Test]
        public void TestSwapTile()
        {
            //AddTile(0, 0);
            //AddTile(0, 1);

            //var beforeSwapTile0 = grid.GetTileData(0, 0);
            //var beforeSwapTile1 = grid.GetTileData(0, 1);
            //var tiles = grid.SwapTiles(0, 0, 0, 1, true);

            //Assert.IsNotNull(tiles.firstTile);
            //Assert.IsNotNull(tiles.secondTile);

            //var afterSwapTile0 = grid.GetTileData(0, 0);
            //var afterSwapTile1 = grid.GetTileData(0, 1);

            //Assert.AreSame(beforeSwapTile0, afterSwapTile1);
            //Assert.AreSame(beforeSwapTile1, afterSwapTile0);
        }

        public void AddTile(int row, int col)
        {
            var template = Resources.Load<GridTile>("Prefabs/GridTile");
            var view = MonoBehaviour.Instantiate<GridTile>(template);
            view.transform.name = row + " = " + col;

           // grid.AddTile(view, row, col, false);
        }
        //[Test]
        //public void TestSwipeLeftGuesture()
        //{
        //    var lastEventData = new PointerEventData(EventSystem.current);
        //    lastEventData.position = new Vector2(0f, 0f);
        //    controller.OnBeginDrag(lastEventData);

        //    var dragEventData = new PointerEventData(EventSystem.current);
        //    dragEventData.position = new Vector2(-3f, 0f);
        //    controller.OnDrag(dragEventData);

        //    var direction =  dragEventData.position - lastEventData.position;

        //    Assert.Greater(Mathf.Abs(direction.x), Mathf.Abs(direction.y));
        //    Assert.LessOrEqual(direction.x, 0);
        //}

        //[Test]
        //public void TestSwipeRightGuesture()
        //{
        //    var lastEventData = new PointerEventData(EventSystem.current);
        //    lastEventData.position = new Vector2(0f, 0f);
        //    controller.OnBeginDrag(lastEventData);

        //    var dragEventData = new PointerEventData(EventSystem.current);
        //    dragEventData.position = new Vector2(3f, 0f);
        //    controller.OnDrag(dragEventData);

        //    var direction = dragEventData.position - lastEventData.position;

        //    Assert.Greater(Mathf.Abs(direction.x), Mathf.Abs(direction.y));
        //    Assert.Greater(direction.x, 0);
        //}

        //[Test]
        //public void TestSwipeTopGuesture()
        //{
        //    var lastEventData = new PointerEventData(EventSystem.current);
        //    lastEventData.position = new Vector2(0f, 0f);
        //    controller.OnBeginDrag(lastEventData);

        //    var dragEventData = new PointerEventData(EventSystem.current);
        //    dragEventData.position = new Vector2(0f, 3f);
        //    controller.OnDrag(dragEventData);

        //    var direction = dragEventData.position - lastEventData.position;

        //    Assert.Greater(Mathf.Abs(direction.y), Mathf.Abs(direction.x));
        //    Assert.Greater(direction.y, 0);
        //}

        //[Test]
        //public void TestSwipeBottomGuesture()
        //{
        //    var lastEventData = new PointerEventData(EventSystem.current);
        //    lastEventData.position = new Vector2(0f, 0f);
        //    controller.OnBeginDrag(lastEventData);

        //    var dragEventData = new PointerEventData(EventSystem.current);
        //    dragEventData.position = new Vector2(0f, -3f);
        //    controller.OnDrag(dragEventData);

        //    var direction = dragEventData.position - lastEventData.position;

        //    Assert.Greater(Mathf.Abs(direction.y), Mathf.Abs(direction.x));
        //    Assert.LessOrEqual(direction.y, 0);
        //}

        //[Test]
        //public void TestValidateLeftSwipeIndexOutOfBoardSize()
        //{
        //    int i = 0;
        //    var row = Mathf.FloorToInt(i / layout.constraintCount);
        //    var col = i % layout.constraintCount;

        //    int nextIndex = row - 1; //moving row index 1 step to left side

        //    Assert.GreaterOrEqual(nextIndex, 0);
        //}
    }
}
