using System.Collections.Generic;
using UnityEngine;
using Iniectio.Lite;
using UnityEngine.UI;

namespace Everest.PuzzleGame
{
    public class GamePlayScreen : View
    {
        [Inject] private IPlayer m_Player { get; set; }
        [Inject] private SetupBoardSignal m_SetupBoardSignal { get; set; }

        [SerializeField] private Text m_ScoreText;
        [SerializeField] private Text m_BestScoreText;

        protected override void Start()
        {
            base.Start();
            m_BestScoreText.text = m_Player.BestScore.ToString();
            m_ScoreText.text = m_Player.Score.ToString();
        }

        [Listen(typeof(GameUpdateSignal))]
        private void OnUpdate()
        {
            m_Player.UpdateScore(1);
            Debug.Log("Calling");
            m_ScoreText.text = m_Player.Score.ToString();
        }

        [Listen(typeof(StartGameSignal))]
        private void OnGameStart()
        {
            transform.GetChild(0).gameObject.SetActive(true);//test
            m_BestScoreText.text = m_Player.BestScore.ToString();
            m_ScoreText.text = m_Player.Score.ToString();
            m_SetupBoardSignal.Dispatch();
        }
    }
}
