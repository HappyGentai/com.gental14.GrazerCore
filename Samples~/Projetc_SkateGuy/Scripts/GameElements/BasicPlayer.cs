using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using GrazerCore.GameElements;
using SkateHero.Skills;
using SkateHero.GameElements.States.PlayerStates;

namespace SkateHero.GameElements
{
    public class BasicPlayer : PlayableObject
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
        public override float MaxGrazeCounter
        {
            get { return m_MaxGrazeCounter; }
            protected set { }
        }
        public override float MoveSpeed
        {
            get { return m_MoveSpeed; }
            protected set { }
        }
        public override float FocusModeScaleRate
        {
            get { return m_FocusModeScaleRate; }
            protected set { }
        }
        public override bool IsFocusMode
        {
            get { return isFocusMode; }
            set { isFocusMode = value; }
        }
        public override Launcher[] Launchers
        {
            get { return m_Launchers; }
            set { m_Launchers = value; }
        }

        public override UnityEvent<float> OnHPChange {
            get { return _OnHPChange; }
            protected set { }
        }

        public override UnityEvent<float> OnGrazeCounterChange
        {
            get { return _OnGrazeCounterChange; }
            protected set { }
        }

        public override UnityEvent<Collider2D[]> OnGraze
        {
            get { return _OnGraze; }
            protected set { }
        }

        public override UnityEvent OnPlayerDie
        {
            get { return _OnPlayerDie; }
            protected set { }
        }

        public override UnityEvent<PlayableObject, float> OnGetDamage
        {
            get { return _OnGetDamage; }
            protected set { }
        }

        [Header("Control")]
        [SerializeField]
        private InputActionReference m_MoverActionRef = null;
        protected InputAction m_MoveAction;
        public InputAction MoveAction { get { return m_MoveAction; } set { m_MoveAction = value; } }
        [SerializeField]
        private InputActionReference m_FocusModeActionRef = null;
        protected InputAction m_FocusModeAction;
        public InputAction FocusModeAction { get { return m_FocusModeAction; } set { m_FocusModeAction = value; } }
        [SerializeField]
        private InputActionReference m_FireActionRef = null;
        protected InputAction m_FireAction;
        public InputAction FireAction { get { return m_FireAction; } set { m_FireAction = value; } }

        [Header("Additional Value")]
        [SerializeField]
        private float m_MaxExGauge = 0;
        public float MaxExGauge
        {
            get { return m_MaxExGauge;}
            set { m_MaxExGauge = value; }
        }
        [SerializeField]
        private float m_ExGuage = 0;
        public float ExGuage
        {
            get { return m_ExGuage; }
            set
            {
                if (value > MaxExGauge)
                {
                    m_ExGuage = MaxExGauge;
                } else if (value < 0)
                {
                    m_ExGuage = 0;
                } else
                {
                    m_ExGuage = value;
                }
                OnExGaugeChange?.Invoke(m_ExGuage);
            }
        }
        public UnityEvent<float> OnExGaugeChange = new UnityEvent<float>();


        private UnityEvent<PlayableObject, GameObject> m_OnHitBoxCollision = 
            new UnityEvent<PlayableObject, GameObject>();
        public UnityEvent<PlayableObject, GameObject> OnHitBoxCollision
        {
            get { return m_OnHitBoxCollision; }
        }
        public override bool Invincible {
            get => base.Invincible;
            set
            {
                base.Invincible = value;
            }
        }

        [Header("Skills")]
        [SerializeField]
        private SkillTrigger[] m_SkillTriggers = null;
        public SkillTrigger[] SkillTriggers
        {
            get { return m_SkillTriggers; }
            private set { }
        }
        [Header("ExSkills")]
        [SerializeField]
        private SkillTrigger m_ExSkillTrigger = null;

        [Header("Option")]
        [SerializeField]
        private bool m_PlayOnAwake = false;

        protected void Awake()
        {
            if (m_PlayOnAwake)
            {
                Initialization();
                WakeUpObject();
            }  
        }

        public override void Initialization()
        {
            base.Initialization();
            //  Set action
            MoveAction = m_MoverActionRef.action;
            FocusModeAction = m_FocusModeActionRef.action;
            FireAction = m_FireActionRef.action;

            m_FocusModeAction.started += (ctx) => {
                isFocusMode = true;
            };
            m_FocusModeAction.canceled += (ctx) => {
                isFocusMode = false;
            };
            m_FireAction.started += (ctx) => {
                var launcherCount = Launchers.Length;
                for (int index = 0; index < launcherCount; ++index)
                {
                    var launcher = Launchers[index];
                    if (launcher.IsWorking)
                    {
                        launcher.StartTrigger();
                    }
                }
            };
            m_FireAction.canceled += (ctx) => {
                var launcherCount = Launchers.Length;
                for (int index = 0; index < launcherCount; ++index)
                {
                    var launcher = Launchers[index];
                    if (launcher.IsWorking)
                    {
                        launcher.ReleaseTrigger();
                    }
                }
            };
        }

        public override void WakeUpObject()
        {
            this.gameObject.SetActive(true);
            base.WakeUpObject();
            m_MoveAction.Enable();
            m_FocusModeAction.Enable();
            m_FireAction.Enable();

            ExGuage = 0;
            //  Wake up skill triggers
            var skillTriggerCount = m_SkillTriggers.Length;
            for (int index = 0; index < skillTriggerCount; ++index)
            {
                var skilltrigger = m_SkillTriggers[index];
                skilltrigger.AwakeTrigger();
            }
            //  Wake up ExSkill trigger
            m_ExSkillTrigger.AwakeTrigger();
            var basicPlayerControl = new PlayerStateBasicControl(StateController, this);
            StateController.SetState(basicPlayerControl);
        }

        public override void SleepObject()
        {
            base.SleepObject();
            m_MoveAction.Disable();
            m_FocusModeAction.Disable();
            m_FireAction.Disable();

            //  Sleep skill triggers
            var skillTriggerCount = m_SkillTriggers.Length;
            for (int index = 0; index < skillTriggerCount; ++index)
            {
                var skilltrigger = m_SkillTriggers[index];
                skilltrigger.SleepTrigger();
            }
            //  Sleep up ExSkill trigger
            m_ExSkillTrigger.SleepTrigger();
        }

        protected override void Die()
        {
            base.Die();
            ////  Do die event, can call WakeUpObject to re set data
            //OnPlayerDie?.Invoke();
            this.gameObject.SetActive(false);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            OnHitBoxCollision?.Invoke(this, collision.gameObject);
        }
    }
}
