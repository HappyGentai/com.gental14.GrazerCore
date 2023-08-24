using UnityEngine.Events;
using UnityEngine;
using SkateHero.GameElements.States.EnemyStates;
using GrazerCore.GameElements;
using SkateHero.GameElements.EnemyLogicData;
using GrazerCore.Interfaces;

namespace SkateHero.GameElements
{
    public class EnemyTypeSinMove : Enemy, ILogicDataSetable
    {
        public override Transform MoveTarget
        {
            get { return m_MoveTarget; }
            protected set { }
        }
        public override float MaxHP
        {
            get { return m_MaxHP; }
            protected set { }
        }
        public override CircleCollider2D HitBox
        {
            get { return m_HitBox; }
            protected set { }
        }
        public override float MoveSpeed
        {
            get { return m_MoveSpeed; }
            protected set { }
        }
        public override Launcher[] Launchers
        {
            get { return m_Launchers; }
            set { m_Launchers = value; }
        }

        public override UnityEvent<float> OnHPChange
        {
            get { return _OnHPChange; }
            protected set { }
        }

        public override UnityEvent OnEnemyDie
        {
            get { return _OnEnemyDie; }
            protected set { }
        }

        [Header("Logic value")]
        [SerializeField]
        private EnemyTypeSinMoveLogicData m_LogicData = null;

        [Header("Option")]
        [SerializeField]
        private bool m_PlayWhenStart = false;

        protected void Start()
        {
            if (m_PlayWhenStart)
            {
                Initialization();
                StartAction();
            }
        }

        public override void StartAction()
        {
            WakeUpObject();
            var sinMoveState = new EnemyStateSinMove(StateController, this, m_LogicData.MoveDirection, m_LogicData.SinHalfHeigh, m_LogicData.AttackWhenMove);
            StateController.SetState(sinMoveState);
        }

        public string GetLogicData()
        {
            var logicDataPack = JsonUtility.ToJson(m_LogicData);
            return logicDataPack;
        }

        public void SetLogicData(string rawData)
        {
            var logicData = JsonUtility.FromJson<EnemyTypeSinMoveLogicData>(rawData);
            m_LogicData = logicData;
        }

        private void OnDrawGizmos()
        {
            var selfPos = this.transform.localPosition;
            var endPoint = selfPos + (Vector3)m_LogicData.MoveDirection * MoveSpeed; 
            Gizmos.color = Color.green;
            Gizmos.DrawLine(selfPos, endPoint);
            Gizmos.color = Color.red;
            var pV = Vector2.Perpendicular(endPoint - selfPos).normalized * m_LogicData.SinHalfHeigh;
            Gizmos.DrawLine(endPoint, endPoint + (Vector3)pV);
        }
    }
}
