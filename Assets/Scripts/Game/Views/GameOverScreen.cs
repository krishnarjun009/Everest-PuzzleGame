using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Iniectio.Lite;

namespace Everest.PuzzleGame
{
    public class GameOverScreen : View
    {
        [Inject] private RestartGameSignal m_RestartGameSignal { get; set; }

        private GameObject m_MainPanel;

        protected override void Awake()
        {
            base.Awake();
            m_MainPanel = transform.GetChild(0).gameObject;
        }

        public void OnRestart()
        {
            m_MainPanel.SetActive(false);
            m_RestartGameSignal.Dispatch();
        }

        [Listen(typeof(GameOverSignal))]
        private void OnGameOver()
        {
            m_MainPanel.SetActive(true);
        }
    }
}
