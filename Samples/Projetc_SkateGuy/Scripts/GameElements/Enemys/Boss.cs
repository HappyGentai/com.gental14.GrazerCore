using UnityEngine;
using GrazerCore.GameElements;
using SkateHero.GameElements.States.EnemyStates;
using UnityEngine.Events;

namespace SkateHero.GameElements
{
    public class Boss : Enemy
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

        [SerializeField]
        private Vector2 m_MoveToPoint = Vector2.zero;
        [Header("Attack A Weapon")]
        [SerializeField]
        private Launcher[] m_AttackALaunchers = null;
        [SerializeField]
        private float m_AttackADelayTime = 0;
        [SerializeField]
        private float m_AttackAFireTime = 0;
        [Header("Attack B Weapon")]
        [SerializeField]
        private Launcher[] m_AttackBLaunchers = null;
        [SerializeField]
        private float m_AttackBDelayTime = 0;
        [SerializeField]
        private float m_AttackBFireTime = 0;
        [Header("Attack C Weapon")]
        [SerializeField]
        private Launcher[] m_AttackCLaunchers = null;
        [SerializeField]
        private float m_AttackCDelayTime = 0;
        [SerializeField]
        private float m_AttackCFireTime = 0;

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
            var moveToPointState = new EnemyStateMoveToPoint(StateController, this, m_MoveToPoint);
            var bossAttackA = new BossStateNormalAttack(StateController, this, m_AttackALaunchers, m_AttackADelayTime, m_AttackAFireTime);
            var bossAttackB = new BossStateNormalAttack(StateController, this, m_AttackBLaunchers, m_AttackBDelayTime, m_AttackBFireTime);
            var bossAttackC = new BossStateNormalAttack(StateController, this, m_AttackCLaunchers, m_AttackCDelayTime, m_AttackCFireTime);
            moveToPointState.nextState = bossAttackA;
            bossAttackA.nextState = bossAttackB;
            bossAttackB.nextState = bossAttackC;
            bossAttackC.nextState = bossAttackA;
            StateController.SetState(moveToPointState);
        }
    }
}