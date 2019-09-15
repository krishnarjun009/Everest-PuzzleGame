using UnityEngine;
using Iniectio.Lite;
using UnityEngine.UI;

namespace Everest.PuzzleGame
{
    public class GamePlayScreen : View
    {
        [Inject] private IPlayer m_Player { get; set; }
        [Inject] private SetupBoardSignal m_SetupBoardSignal { get; set; }
        [Inject] private EnableMainMenuSignal m_EnableMainMenuSignal { get; set; }

        [SerializeField] private Text m_ScoreText;
        [SerializeField] private Text m_BestScoreText;
        [SerializeField] private Button m_BackBtn;

        private GameObject m_MainPanel;

        public override void OnRegister()
        {
            m_BackBtn.onClick.AddListener(OnBackClicked);
        }

        public override void OnRemove()
        {
            m_BackBtn.onClick.RemoveListener(OnBackClicked);
        }

        protected override void Start()
        {
            base.Start();
            m_MainPanel = transform.GetChild(0).gameObject;
            m_BestScoreText.text = m_Player.BestScore.ToString();
            m_ScoreText.text = m_Player.Score.ToString();
        }

        private void OnBackClicked()
        {
            m_EnableMainMenuSignal.Dispatch(true);
            m_MainPanel.SetActive(false);
        }

        [Listen(typeof(GameUpdateSignal))]
        private void OnUpdate() => m_ScoreText.text = m_Player.Score.ToString();

        [Listen(typeof(StartGameSignal))]
        private void OnGameStart()
        {
            m_MainPanel.SetActive(true);
            m_BestScoreText.text = m_Player.BestScore.ToString();
            m_ScoreText.text = m_Player.Score.ToString();
            m_SetupBoardSignal.Dispatch();
        }
    }
}
