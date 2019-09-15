using Iniectio.Lite;
using UnityEngine;

namespace Everest.PuzzleGame
{
    public class ApplicationHandler : View
    {
        [Inject] private LoadPlayerRequestSignal                m_LoadPlayerRequestSignal { get; set; }
        [Inject] private LoadPlayerResponseSignal               m_LoadPlayerResponseSignal { get; set; }
        [Inject] private AsyncLoaderController                  m_AsyncLoaderController { get; set; }
        [Inject] private UpdateProgressBarSignal                m_UpdateProgressBarSignal { get; set; }
        [Inject] private SavePlayerRequestSignal                m_SavePlayerRequestSignal { get; set; }
        [Inject] private SaveUserInLeaderBoardRequestSignal     m_SaveUserInLeaderBoardRequestSignal { get; set; }
        [Inject] private IPlayer                                m_Player { get; set; }

        public override void OnRegister()
        {
            m_LoadPlayerResponseSignal.AddListener(OnPlayerWasLoaded);
        }

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(this);
        }

        protected override void Start()
        {
            base.Start();
            m_LoadPlayerRequestSignal.Dispatch(PlayerPrefs.GetString("UserName", "Guest"));

            //m_SaveUserInLeaderBoardRequestSignal.Dispatch();//test
        }

        [Listen(typeof(LoadPlayerResponseSignal))]
        private void OnPlayerWasLoaded()
        {
            m_AsyncLoaderController.LoadScene("GameScene", false, this, (value) => {

                m_UpdateProgressBarSignal.Dispatch(value);

            });
            m_LoadPlayerResponseSignal.RemoveListener(OnPlayerWasLoaded);
        }

        [ContextMenu("Delete Pref")]
        public void Delete()
        {
            PlayerPrefs.DeleteAll();
        }
    }
}
