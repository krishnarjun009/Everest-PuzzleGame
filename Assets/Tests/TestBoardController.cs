using System.Collections.Generic;
using Everest.PuzzleGame;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Tests
{
    public class TestBoardController
    {
        BoardController controller;
        GridLayoutGroup layout;

        [SetUp]
        public void Setup()
        {
            //setting up grid components
            var grid = new GameObject("BoardController");
            layout = grid.AddComponent<GridLayoutGroup>();
            controller = grid.AddComponent<BoardController>();
        }

        [Test]
        public void TestCreateBoard()
        {
            //initializing board
            controller.InitBoardGame();

            //validating board elements count
            Assert.Greater(controller.transform.childCount, 0);
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
