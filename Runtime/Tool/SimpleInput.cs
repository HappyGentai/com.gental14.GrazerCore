using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.Events;

namespace GrazerCore.Tool
{
    public class SimpleInput : MonoBehaviour
    {
        [SerializeField]
        private Transform m_MoveObject = null;
        [SerializeField]
        private float m_Speed = 1f;
        [SerializeField]
        private UnityEvent m_OnStartTrigger = new UnityEvent();
        [SerializeField]
        private UnityEvent m_OnTrigger = new UnityEvent();
        [SerializeField]
        private UnityEvent m_OnReleaseTrigger = new UnityEvent();
        [Header("Control")]
        [SerializeField]
        protected InputAction m_MoveAction;
        [SerializeField]
        protected InputAction m_TriggerAction;

        private void Start()
        {
            m_MoveAction.Enable();
            m_TriggerAction.Enable();
            m_TriggerAction.started += (ctx) => {
                m_OnStartTrigger?.Invoke();
            };
            m_TriggerAction.canceled += (ctx) => {
                m_OnReleaseTrigger?.Invoke();
            };
        }

        private void Update()
        {
            if (m_TriggerAction.IsPressed())
            {
                m_OnTrigger?.Invoke();
            }

            var moveValue = m_MoveAction.ReadValue<Vector2>();
            //  Normalize move value
            moveValue = moveValue.normalized;
            moveValue *= m_Speed;
            m_MoveObject.localPosition += (Vector3)moveValue * Time.deltaTime;
        }
    }

}
