using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace SkateHero.UIs
{
    public class AudioVolumeUI : BasicUI
    {
        [SerializeField]
        private AudioMixer m_AudioMixer = null;
        [SerializeField]
        private string m_MainName = "";
        [SerializeField]
        private Slider m_MinSlider = null;
        [SerializeField]
        private string m_BGMName = "";
        [SerializeField]
        private Slider m_BGMSlider = null;
        [SerializeField]
        private string m_SFXName = "";
        [SerializeField]
        private Slider m_SFXSlider = null;

        protected override void DoInitialize()
        {
            m_MinSlider.value = GetVolumeValue(m_MainName);
            m_BGMSlider.value = GetVolumeValue(m_BGMName);
            m_SFXSlider.value = GetVolumeValue(m_SFXName);
        }

        public override void Close()
        {
            base.Close();
            //  Audio setting
        }

        public void SetMainVolume(float value)
        {
            if (!IsOpen)
            {
                return;
            }
            //m_AudioMixer.SetFloat(m_MainName, Mathf.Log10(value) * 20);
            m_AudioMixer.SetFloat(m_MainName, value);
        }

        public void SetBGMVolume(float value)
        {
            if (!IsOpen)
            {
                return;
            }
            //m_AudioMixer.SetFloat(m_BGMName, Mathf.Log10(value) * 20);
            m_AudioMixer.SetFloat(m_BGMName, value);
        }

        public void SetSFXVolume(float value)
        {
            if (!IsOpen)
            {
                return;
            }
            //m_AudioMixer.SetFloat(m_SFXName, Mathf.Log10(value) * 20);
            m_AudioMixer.SetFloat(m_SFXName, value);
        }

        private float GetVolumeValue(string name)
        {
            var volume = 0f;
            m_AudioMixer.GetFloat(name, out volume);
            //volume = Mathf.Pow(10, volume);
            return volume;
        }

        /*
         *  ���󭵶q�d��]�w
         *  AudioMixer���d��O-80~20�A�M�b�ϥΪ̳]�w�W�D�`������
         *  �]�����M�ק�AudioMixer���Ѽ�0��@�O���q���̤j��(100%)
         *  -80���̧C�Ȩ̦��ӹ�{�����W��0~1���d��Ӿޱ�
         *  ���L�b�d��]�w�W��ĳ�̧C�ȬO���ͪ��s����(0.001)�Ӥ��O0
         *  �H�קK�o�Ͱ��D�C
         *  ������z�i�H�ѦҦ��Q��#12���^��:
         *  https://forum.unity.com/threads/changing-audio-mixer-group-volume-with-ui-slider.297884/
         */
    }
}
