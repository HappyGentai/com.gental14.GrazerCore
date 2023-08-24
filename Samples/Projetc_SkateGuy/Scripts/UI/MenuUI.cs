using UnityEngine;
using GrazerCore.GameFlow;
using UnityEngine.Events;

namespace SkateHero.UIs
{
    public class MenuUI : BasicUI
    {
        [SerializeField]
        private GameSettingUI m_GameSettingUIPrefab = null;
        private GameSettingUI _GameSettingUI = null;
        public UnityEvent OnGameStart = new UnityEvent();

        protected override void DoInitialize()
        {
            _GameSettingUI = Instantiate<GameSettingUI>(m_GameSettingUIPrefab);
            _GameSettingUI.Initialize();
            _GameSettingUI.OnUIClose.AddListener(() => {
                SetSelectedGameObject(m_SelectedUIOnOpen);
            });
            _GameSettingUI.Close();
        }

        #region For ui button on click event.
        public void StartGame()
        {
            OnGameStart?.Invoke();
        }

        public void OpenSettingPage()
        {
            _GameSettingUI.Open();
        }

        public void CloseGame()
        {
            Application.Quit();
        }
        #endregion

        public override void Close()
        {
            if (_GameSettingUI.IsOpen)
            {
                _GameSettingUI.Close();
            }
            base.Close();
        }
    }
}
