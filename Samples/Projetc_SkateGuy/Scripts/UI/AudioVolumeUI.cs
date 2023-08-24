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
         *  關於音量範圍設定
         *  AudioMixer的範圍是-80~20，套在使用者設定上非常不值關
         *  因此此專案把AudioMixer的參數0當作是音量的最大值(100%)
         *  -80為最低值依此來實現介面上用0~1的範圍來操控
         *  不過在範圍設定上建議最低值是用趨近於零的數(0.001)而不是0
         *  以避免發生問題。
         *  相關原理可以參考此討論#12的回覆:
         *  https://forum.unity.com/threads/changing-audio-mixer-group-volume-with-ui-slider.297884/
         */
    }
}
