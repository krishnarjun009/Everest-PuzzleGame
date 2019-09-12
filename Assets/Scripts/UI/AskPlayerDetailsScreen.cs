using Iniectio.Lite;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Everest.PuzzleGame
{
    public class AskPlayerDetailsScreen : View
    {
        [Inject] private SavePlayerRequestSignal m_SavePlayerRequestSignal { get; set; }
        [Inject] private StartGameSignal m_StartGameSignal { get; set; }
        [Inject] private IPlayer m_Player { get; set; }

        [SerializeField] private Button m_PlayBtn;
        [SerializeField] private Button m_CloseBtn;
        [SerializeField] private Button m_ContinueAsSameBtn;
        [SerializeField] private Text m_InputText;
        [SerializeField] private Text m_LogginedAsText;

        private GameObject m_MainPanel;

        public override void OnRegister()
        {
            m_PlayBtn.onClick.AddListener(OnPlayClicked);
            m_CloseBtn.onClick.AddListener(OnCloseClicked);
            m_ContinueAsSameBtn.onClick.AddListener(OnContinueSameUser);
        }

        public override void OnRemove()
        {
            m_PlayBtn.onClick.RemoveListener(OnPlayClicked);
            m_CloseBtn.onClick.RemoveListener(OnCloseClicked);
            m_ContinueAsSameBtn.onClick.RemoveListener(OnContinueSameUser);
        }

        protected override void Awake()
        {
            base.Awake();
            m_MainPanel = transform.GetChild(0).gameObject;
        }

        private void OnPlayClicked()
        {
            if(!System.String.IsNullOrEmpty(m_InputText.text) && ValidateUserName(m_InputText.text))
            {
                m_SavePlayerRequestSignal.Dispatch(m_InputText.text);
            }
                
        }

        private void OnCloseClicked()
        {
            m_MainPanel.SetActive(false);
        }

        private void OnContinueSameUser()
        {
            m_StartGameSignal.Dispatch();
            OnCloseClicked();
        }

        private bool ValidateUserName(string username)
        {
            var rg = new System.Text.RegularExpressions.Regex(@"^[a-zA-Z0-9\s,]*$");
            return rg.IsMatch(username);
        }

        [Listen(typeof(EnableAskUserNameScreenSignal))]
        private void OnEnableScreen()
        {
            m_MainPanel.SetActive(true);
            Debug.Log(m_Player.UserName);
            m_LogginedAsText.text = "Continue as " + m_Player.UserName;
        }
    }
}
