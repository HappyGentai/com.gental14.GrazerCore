using UnityEngine;
using UnityEngine.Events;
using GrazerCore.GameElements;
using SkateHero.GameElements.States.EnemyStates;
using SkateHero.GameElements.EnemyLogicData;
using GrazerCore.Interfaces;

namespace SkateHero.GameElements {
    public class EnemyTypeHitAndRun : Enemy, ILogicDataSetable
    {
        public override Transform MoveTarget
        {
            get { return m_MoveTarget; }
            protected set { }
        }
        public override float MaxHP {
            get{ return m_MaxHP; }
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
        private EnemyTypeHitAndRunLogicData m_LogicData = null;

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
            var moveToPointState = new EnemyStateMoveToPoint(StateController, this, m_LogicData.MoveTargetPoint);
            var attackWithTimeState = new EnemyStateAttackWithTime(StateController, this, m_LogicData.FireTime);
            var fleeState = new EnemyStateMove(StateController, this, m_LogicData.FleeDir);
            moveToPointState.nextState = attackWithTimeState;
            attackWithTimeState.nextState = fleeState;
            StateController.SetState(moveToPointState);
        }

        public string GetLogicData()
        {
            var logicDataPack = JsonUtility.ToJson(m_LogicData);
            return logicDataPack;
        }

        public void SetLogicData(string rawData)
        {
            var logicData = JsonUtility.FromJson<EnemyTypeHitAndRunLogicData>(rawData);
            m_LogicData = logicData;
        }
    }
}
