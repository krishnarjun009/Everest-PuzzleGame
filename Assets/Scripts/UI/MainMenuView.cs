using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Iniectio.Lite;
using UnityEngine.UI;
using System;

namespace Everest.PuzzleGame
{
    public class MainMenuView : View
    {
        [Inject] private LoadPlayerRequestSignal m_LoadPlayerRequestSignal { get; set; }
        [Inject] private LoadPlayerResponseSignal m_LoadPlayerResponseSignal { get; set; }
        [Inject] private EnableAskUserNameScreenSignal m_EnableAskUserNameScreenSignal { get; set; }
        [Inject] private IPlayer m_Player { get; set; }

        [SerializeField] private Button m_PlayBtn;
        [SerializeField] private Button m_HighScoresBtn;
        [SerializeField] private Text m_BestScoreText;
        [SerializeField] private Text m_LogginedAsText;

        public override void OnRegister()
        {
            m_PlayBtn.onClick.AddListener(OnPlayClicked);
            m_HighScoresBtn.onClick.AddListener(OnHighScoresClicked);
            m_LoadPlayerResponseSignal.AddListener(OnWasPlayerLoaded);
        }

        public override void OnRemove()
        {
            m_PlayBtn.onClick.RemoveListener(OnPlayClicked);
            m_HighScoresBtn.onClick.RemoveListener(OnHighScoresClicked);
            m_LoadPlayerResponseSignal.RemoveListener(OnWasPlayerLoaded);
        }

        protected override void Awake()
        {
            base.Awake();
            m_LoadPlayerRequestSignal.Dispatch();
        }

        private void OnHighScoresClicked()
        {
            
        }

        private void OnPlayClicked()
        {
            m_EnableAskUserNameScreenSignal.Dispatch();
        }

        private void OnWasPlayerLoaded()
        {
            m_BestScoreText.text = m_Player.BestScore.ToString();
            m_LogginedAsText.text = "Loggined as : " + m_Player.UserName;
        }
    }
}