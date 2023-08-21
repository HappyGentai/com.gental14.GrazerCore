using UnityEngine;
using UnityEngine.Events;
using GrazerCore.Interfaces;

namespace GrazerCore.GameElements
{
    public abstract class PickUpObject : MonoBehaviour, IRecycleable
    {
        [SerializeField]
        protected UnityEvent m_OnPciked = new UnityEvent();
        public UnityEvent OnPickDoen = new UnityEvent();

        /// <summary>
        /// When object picked or enter recycle field, call this.
        /// </summary>
        public virtual void Recycle()
        {
            OnPickDoen?.Invoke();
        }

        public virtual void OnPick()
        {
            m_OnPciked?.Invoke();
            Picked();
        }

        /// <summary>
        /// Do pick done effect
        /// </summary>
        public virtual void Picked()
        {
            Recycle();
        }

        /// <summary>
        /// Re set object state when re use.
        /// </summary>
        public abstract void ReSetState();
    }
}
