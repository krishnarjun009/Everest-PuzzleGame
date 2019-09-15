using UnityEngine;
using Iniectio.Lite;
using UnityEngine.UI;

namespace Everest.PuzzleGame
{
    public class MainMenuView : View
    {
        [Inject] private EnableAskUserNameScreenSignal m_EnableAskUserNameScreenSignal { get; set; }
        [Inject] private EnableHighScoresScreenSignal m_EnableHighScoresScreenSignal { get; set; }
        [Inject] private IPlayer m_Player { get; set; }

        [SerializeField] private Button m_PlayBtn;
        [SerializeField] private Button m_HighScoresBtn;
        [SerializeField] private Text m_BestScoreText;
        [SerializeField] private Text m_LogginedAsText;

        private GameObject m_MainPanel;

        public override void OnRegister()
        {
            m_PlayBtn.onClick.AddListener(OnPlayClicked);
            m_HighScoresBtn.onClick.AddListener(OnHighScoresClicked);
        }

        public override void OnRemove()
        {
            m_PlayBtn.onClick.RemoveListener(OnPlayClicked);
            m_HighScoresBtn.onClick.RemoveListener(OnHighScoresClicked);
        }

        protected override void Awake()
        {
            base.Awake();
            m_MainPanel = transform.GetChild(0).gameObject;
        }

        protected override void Start()
        {
            base.Start();
            Init();
        }

        private void OnHighScoresClicked()
        {
            m_EnableHighScoresScreenSignal.Dispatch(true);
        }

        private void OnPlayClicked()
        {
            m_EnableAskUserNameScreenSignal.Dispatch();
        }

        private void Init()
        {
            m_BestScoreText.text = m_Player.BestScore.ToString();
            m_LogginedAsText.text = "Loggined as : " + m_Player.UserName;
            m_MainPanel.SetActive(true);
        }

        [Listen(typeof(EnableMainMenuSignal))]
        private void OnEnableView(bool enable)
        {
            m_MainPanel.SetActive(enable);
            m_BestScoreText.text = m_Player.BestScore.ToString();
            m_LogginedAsText.text = "Loggined as : " + m_Player.UserName;
        }
    }
}