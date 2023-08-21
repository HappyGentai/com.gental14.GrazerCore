using System.Collections;
using UnityEngine;

namespace GrazerCore.Effects
{
    public class EffectTypeAnimation : SFXEffecter
    {
        [SerializeField]
        private Animator m_Animator = null;
        [SerializeField]
        private string m_SFXAnimeName = "";
        private Coroutine playRoutine = null;

        //  Test
        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Space))
            {
                StartSFX();
            }
        }

        public override void StartSFX()
        {
            base.StartSFX();
            m_Animator.Play(m_SFXAnimeName, 0, 0);
            StopPlaying();
            playRoutine = StartCoroutine(Playing());
        }

        public override void StopSFX()
        {
            StopPlaying();
        }

        private void StopPlaying()
        {
            if (playRoutine != null)
            {
                StopCoroutine(playRoutine);
            }
        }

        private IEnumerator Playing()
        {
            while(true)
            {
                yield return null;
                var currentState = m_Animator.GetCurrentAnimatorStateInfo(0);
                if (currentState.normalizedTime >= 1)
                {
                    OnEffectDone?.Invoke();
                    break;
                }
            }
        }
    }
}
