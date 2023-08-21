using UnityEngine;
using UnityEngine.Events;

namespace GrazerCore.Effects
{
    public abstract class SFXEffecter : MonoBehaviour
    {
        [SerializeField]
        protected UnityEvent m_OnEffectStart = new UnityEvent();
        public UnityEvent OnEffectStart
        {
            get { return m_OnEffectStart; }
        }
        [SerializeField]
        protected UnityEvent m_OnEffectDone = new UnityEvent();
        public UnityEvent OnEffectDone
        {
            get { return m_OnEffectDone; }
        }

        /// <summary>
        /// Call class start play sfx, when play done call OnEffectDone.Invoke
        /// </summary>
        public virtual void StartSFX()
        {
            OnEffectStart?.Invoke();
        }

        public abstract void StopSFX();
    }
}