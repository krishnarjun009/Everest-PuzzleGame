using Iniectio.Lite;
using UnityEngine.EventSystems;

namespace Everest.PuzzleGame
{
    public class BoardInputController : View, IPointerDownHandler
    {
        [Inject] private OnDragSignal m_OnDragSignal { get; set; }

        public void OnPointerDown(PointerEventData eventData)
        {
            m_OnDragSignal.Dispatch(eventData.position);
        }
    }
}
