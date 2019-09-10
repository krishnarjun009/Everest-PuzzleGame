using Iniectio.Lite;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Everest.PuzzleGame
{
    public enum SwipeDirection
    {
        Left,
        Right,
        Top,
        Bottom,
        Auto
    }

    public class BoardInputController : View, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler, IPointerUpHandler
    {
        [Inject] private OnDragSignal m_OnDragSignal { get; set; }

        private Vector2 m_LastPosition;
        private bool m_BlockInput = false;

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (m_BlockInput) return;
            m_LastPosition = eventData.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (m_BlockInput) return;
            var direction = eventData.position - m_LastPosition;
            var isXDirection = Mathf.Abs(direction.x) > Mathf.Abs(direction.y);
            if(isXDirection)
            {
                if(direction.x > 0)
                {
                    //Debug.Log("Swiping right");
                    m_BlockInput = true;
                    m_OnDragSignal.Dispatch(eventData.position, SwipeDirection.Right);
                }
                else if(direction.x < 0)
                {
                    //Debug.Log("Swiping left");
                    m_BlockInput = true;
                    m_OnDragSignal.Dispatch(eventData.position, SwipeDirection.Left);
                }
            }
            else
            {
                if (direction.y > 0)
                {
                    //Debug.Log("Swiping up");
                    m_BlockInput = true;
                    m_OnDragSignal.Dispatch(eventData.position, SwipeDirection.Top);
                }
                else if(direction.y < 0)
                {
                    //Debug.Log("Swiping down");
                    m_BlockInput = true;
                    m_OnDragSignal.Dispatch(eventData.position, SwipeDirection.Bottom);
                }
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (m_BlockInput)
            {
                m_LastPosition = Vector2.zero;
                m_BlockInput = false;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            //m_BlockInput = true;
           // m_OnDragSignal.Dispatch(eventData.position, SwipeDirection.Auto);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
           // m_BlockInput = false;
        }
    }
}
