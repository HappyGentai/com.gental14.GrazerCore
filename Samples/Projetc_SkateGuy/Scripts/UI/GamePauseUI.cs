using UnityEngine.Events;

namespace SkateHero.UIs
{
    public class GamePauseUI : BasicUI
    {
        public UnityEvent OnRestartGame = new UnityEvent();
        public UnityEvent OnBackToTitle = new UnityEvent();

        #region Function for ui button
        public void RestartGame()
        {
            OnRestartGame?.Invoke();
            this.Close();
        }

        public void BackToTitle()
        {
            OnBackToTitle?.Invoke();
            this.Close();
        }
        #endregion
    }
}
