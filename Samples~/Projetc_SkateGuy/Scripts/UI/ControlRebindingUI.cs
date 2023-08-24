using UnityEngine;

namespace SkateHero.UIs {
    public class ControlRebindingUI : BasicUI
    {
        [SerializeField]
        private GameObject m_KeyboardRebindPage = null;
        [SerializeField]
        private GameObject m_SelectedUIOnKeyboardRebinding = null;
        [SerializeField]
        private GameObject m_GamepadRebindPage = null;
        [SerializeField]
        private GameObject m_SelectedUIOnGamepadRebinding = null;

        public override void Open()
        {
            base.Open();
            OpenKeyboardRebindingPage();
        }

        public override void Close()
        {
            base.Close();
            //  Save Rebinding result.
        }

        public void OpenKeyboardRebindingPage()
        {
            m_KeyboardRebindPage.SetActive(true);
            m_GamepadRebindPage.SetActive(false);
            SetSelectedGameObject(m_SelectedUIOnKeyboardRebinding);
        }

        public void OpenGamepadRebindingPage()
        {
            m_GamepadRebindPage.SetActive(true);
            m_KeyboardRebindPage.SetActive(false);
            SetSelectedGameObject(m_SelectedUIOnGamepadRebinding);
        }
    }
}
