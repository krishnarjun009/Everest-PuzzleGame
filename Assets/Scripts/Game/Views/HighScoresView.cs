using Iniectio.Lite;
using System.Collections.Generic;
using UnityEngine;

namespace Everest.PuzzleGame
{
    public class HighScoresView : View
    {
        [Inject] private LoadHighScoreRequestSignal m_LoadHighScoreRequestSignal { get; set; }
        [Inject] private IPlayer m_Player { get; set; }

        [SerializeField] private RectTransform m_ContentRect;
        [SerializeField] private LeaderBoardItemView m_ItemTemplate;
        [SerializeField] private UnityEngine.UI.Text m_BestScoreText;

        private List<LeaderBoardItemView> m_Pool = new List<LeaderBoardItemView>();
        private GameObject m_MainPanel;

        protected override void Awake()
        {
            base.Awake();
            m_MainPanel = transform.GetChild(0).gameObject;
        }

        [Listen(typeof(EnableHighScoresScreenSignal))]
        private void OnEnableView(bool enable)
        {
            m_MainPanel.SetActive(enable);
            if (enable)
            {
                m_BestScoreText.text = m_Player.BestScore.ToString();
                m_LoadHighScoreRequestSignal.Dispatch();
            }

        }

        [Listen(typeof(LoadHighScoresResponseSignal))]
        private void OnLoadedHighScoresResponse(LeaderBoardUserData[] data)
        {
            Disable();
            int sn = 1;
            foreach(var item in data)
            {
                var view = GetLeaderBoardItem();
                view.SetView(sn.ToString(), item.Username, item.BestScore.ToString());
                sn++;
                view.transform.SetParent(m_ContentRect, false);
                view.gameObject.SetActive(true);
                if(!m_Pool.Contains(view))
                    m_Pool.Add(view);
            }
        }

        private LeaderBoardItemView GetLeaderBoardItem()
        {
            foreach(var item in m_Pool)
            {
                if (!item.gameObject.activeInHierarchy)
                    return item;
            }

            return Instantiate<LeaderBoardItemView>(m_ItemTemplate);
        }

        private void Disable()
        {
            foreach (var item in m_Pool)
                item.gameObject.SetActive(false);
        }
    }
}