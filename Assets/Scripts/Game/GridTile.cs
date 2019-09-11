using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Everest.PuzzleGame
{
    public class GridTile : MonoBehaviour, ITile
    {
        [SerializeField] private Text m_ValueText;

        private Image m_Image;
        private RectTransform m_RectTransform;
        private bool m_Empty = false;

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

        internal float Scale
        {
            set
            {
                (transform as RectTransform).localScale = new Vector3(value, value, value);
            }
        }

        internal int SliblingIndex
        {
            get
            {
                return transform.GetSiblingIndex();
            }
            set
            {
                if (value >= 0)
                    transform.SetSiblingIndex(value);
            }
        }

        internal void SetView(int value)
        {
            m_ValueText.text = value.ToString();
        }

        internal void SetColor(Color color)
        {
            image.color = color;
        }

        internal void SetParent(Transform parent, bool isWorld)
        {
            transform.SetParent(parent, isWorld);
        }

        internal void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }

        internal void SetEmpty()
        {
            //image.enabled = false;
            m_Empty = true;
            m_ValueText.gameObject.SetActive(false);
        }

        public bool IsEmpty() => m_Empty;

        public bool IsTileSolved()
        {
            throw new System.NotImplementedException();
        }
    }
}
