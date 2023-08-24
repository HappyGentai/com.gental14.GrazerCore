using UnityEngine;

namespace SkateHero.UIs
{
    public class GameSettingUI : BasicUI
    {
        [SerializeField]
        private ControlRebindingUI m_ControlRebindingUIprefab = null;
        private ControlRebindingUI _ControlRebindingUI = null;
        [SerializeField]
        private AudioVolumeUI m_AudioVolumeUIPrefab = null;
        private AudioVolumeUI _AudioVolumeUI = null;

        protected override void DoInitialize()
        {
            _ControlRebindingUI = Instantiate<ControlRebindingUI>(m_ControlRebindingUIprefab);
            _AudioVolumeUI = Instantiate<AudioVolumeUI>(m_AudioVolumeUIPrefab);
            _ControlRebindingUI.Initialize();
            _AudioVolumeUI.Initialize();
            _ControlRebindingUI.OnUIClose.AddListener(() => {
                SetSelectedGameObject(m_SelectedUIOnOpen);
            });
            _AudioVolumeUI.OnUIClose.AddListener(() => {
                SetSelectedGameObject(m_SelectedUIOnOpen);
            });
            _ControlRebindingUI.Close();
            _AudioVolumeUI.Close();
        }

        public override void Open()
        {
            base.Open();
        }

        #region For ui button on click event.
        public void OpenControlRebindPage()
        {
            _ControlRebindingUI.Open();
        }

        public void OpenAudioVolumePage()
        {
            _AudioVolumeUI.Open();
        }
        #endregion

        public override void Close()
        {
            // optional, save setting from audio and control setting UI.
            if (_ControlRebindingUI.IsOpen)
            {
                _ControlRebindingUI.Close();
            }
            if (_AudioVolumeUI.IsOpen)
            {
                _AudioVolumeUI.Close();
            }
            base.Close();
        }
    }
}
