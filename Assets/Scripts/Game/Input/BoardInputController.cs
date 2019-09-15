using Iniectio.Lite;
using UnityEngine.EventSystems;

namespace Everest.PuzzleGame
{
    public class BoardInputController : View, IPointerDownHandler
    {
        [Inject] private OnDragSignal m_OnDragSignal { get; set; }

        private bool m_BlockInput = false;

        public void OnPointerDown(PointerEventData eventData)
        {
            if(!m_BlockInput)
                m_OnDragSignal.Dispatch(eventData.position);
        }

        [Listen(typeof(EnableInputSignal))]
        private void OnEnableInput() => m_BlockInput = false;
    }
}
