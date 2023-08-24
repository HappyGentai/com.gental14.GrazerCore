using UnityEngine;
using UnityEngine.Events;
using GrazerCore.GameElements;
using GrazerCore.Tool;
using SkateHero.GameElements.States.EnemyStates;
using SkateHero.GameElements.EnemyLogicData;
using GrazerCore.Interfaces;

namespace SkateHero.GameElements
{
    public class EnemyTypeCurveMove : Enemy, ILogicDataSetable
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
        [Tooltip("Will add current move target local position")]
        [SerializeField]
        private EnemyTypeCurveMoveLogicData m_LogicData = null;

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
            var startPos = MoveTarget.localPosition;
            var endPos = (Vector2)startPos + m_LogicData.CurveEndPos;
            var curveMoveState = new EnemyStateCurveMove(StateController, this, startPos, m_LogicData);
            var selfDestructionState = new EnemyStateSelfDestruction(StateController, this);
            curveMoveState.nextState = selfDestructionState;
            StateController.SetState(curveMoveState);
        }

        public string GetLogicData()
        {
            var logicDataPack = JsonUtility.ToJson(m_LogicData);
            return logicDataPack;
        }

        public void SetLogicData(string rawData)
        {
            var logicData = JsonUtility.FromJson<EnemyTypeCurveMoveLogicData>(rawData);
            m_LogicData = logicData;
        }

        private void OnDrawGizmos()
        {
            var startPos = MoveTarget.localPosition;
            var endPos = (Vector2)startPos + m_LogicData.CurveEndPos;
            Gizmos.color = new Color(1, 0, 0, 0.25f);
            Gizmos.DrawSphere(startPos, 0.25f);
            Gizmos.DrawSphere(endPos, 0.25f);
            for (int index = 1; index < 10; index++)
            {
                Gizmos.color = new Color(0, 1, 0, 1);
                var progressPos = LineLerp.CubicLerp((Vector2)startPos, m_LogicData.CurveAidPosA, m_LogicData.CurveAidPosB, endPos, index * 0.1f);
                Gizmos.DrawSphere(progressPos, 0.15f);
            }

            //  Draw search line
            Gizmos.color = Color.blue;
            var searchPos = startPos + (Vector3)m_LogicData.SearchDirection *
                m_LogicData.SearchLength;
            Gizmos.DrawLine(startPos, searchPos);
        }
    }
}
