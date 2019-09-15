using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Everest.PuzzleGame
{
    public class GridTile : MonoBehaviour, ITileView
    {
        [SerializeField] private Text   m_ValueText;

        private Image                   m_Image;
        private RectTransform           m_RectTransform;


        private Image image
        {
            get
            {
                if (m_Image == null) m_Image = this.GetComponent<Image>();
                return m_Image;
            }
        }

        public RectTransform rectTransform
        {
            get
            {
                if (m_RectTransform == null) m_RectTransform = GetComponent<RectTransform>();
                return m_RectTransform;
            }
        }

        public void SetView(int value)
        {
            rectTransform.localScale = new Vector3(0f, 0f, 0f);
            m_ValueText.text = value.ToString();
        }

        public void SetRelative()
        {
            var width = rectTransform.rect.width;
            var height = rectTransform.rect.height;

        }

        public void SetColor(Color color)
        {
            image.color = color;
        }

        public void SetParent(Transform parent, bool isWorld)
        {
            transform.SetParent(parent, isWorld);
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
            if(active)
                PlayTween();
        }

        internal void SetEmpty()
        {
            rectTransform.localScale = new Vector3(0f, 0f, 0f);
            m_ValueText.gameObject.SetActive(false);
        }

        private void PlayTween()
        {
            rectTransform.DOScale(1f, 0.35f).SetDelay(0.25f);
        }
    }
}
