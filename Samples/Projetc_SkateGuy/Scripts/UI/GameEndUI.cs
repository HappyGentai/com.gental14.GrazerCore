using UnityEngine;
using TMPro;
using UnityEngine.Events;

namespace SkateHero.UIs
{
    public class GameEndUI : BasicUI
    {
        [SerializeField]
        private TextMeshProUGUI m_GameText = null;
        [SerializeField]
        private string m_WinTextKey = "";
        [SerializeField]
        private string m_LoseTextKey = "";
        public UnityEvent OnRestartGame = new UnityEvent();
        public UnityEvent OnBackToTitle = new UnityEvent();

        public void GameEnd(bool isWin)
        {
            Open();
            if (isWin)
            {
                m_GameText.text = m_WinTextKey;
            } else
            {
                m_GameText.text = m_LoseTextKey;
            }
        }

        public override void Open()
        {
            base.Open();
        }

        public override void Close()
        {
            base.Close();
        }

        #region Function for ui button
        public void RestartGame()
        {
            OnRestartGame?.Invoke();
            //this.Close();
        }

        public void BackToTitle()
        {
            OnBackToTitle?.Invoke();
            //this.Close();
        }
        #endregion
    }
}
