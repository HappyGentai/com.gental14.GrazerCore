using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using GrazerCore.GameElements.States;
using GrazerCore.Effects;
using GrazerCore.Factories;
using GrazerCore.Interfaces;

namespace GrazerCore.GameElements
{
    public abstract class PlayableObject : MonoBehaviour, IDamageable
    {
        [SerializeField]
        protected Transform m_MoveTarget = null;
        public abstract Transform MoveTarget { get; protected set; }

        [Header("State")]
        [SerializeField]
        protected float m_MaxHP = 100;
        public abstract float MaxHP { get; protected set; }
        [SerializeField]
        protected float m_HP = 0;
        public virtual float HP {
            get { return m_HP; }
            set
            {
                m_HP = value;
                if (m_HP >= m_MaxHP)
                {
                    m_HP = m_MaxHP;
                } else if (m_HP < 0)
                {
                    m_HP = 0;
                }
                _OnHPChange.Invoke(m_HP);
                if (m_HP <= 0)
                {
                    Die();
                    _OnPlayerDie.Invoke();
                }
            }
        }
        [SerializeField]
        protected bool m_Invincible = false;
        public virtual bool Invincible
        {
            get { return m_Invincible; }
            set
            {
                m_Invincible = value;
            }
        }
        [SerializeField]
        protected float m_MaxGrazeCounter = 100;
        public abstract float MaxGrazeCounter { get; protected set; }
        [SerializeField]
        protected float grazeCounter = 0;
        public virtual float GrazeCounter
        {
            get { return grazeCounter; }
            set
            {
                grazeCounter = value;
                if (grazeCounter >= m_MaxGrazeCounter)
                {
                    grazeCounter = m_MaxGrazeCounter;
                }
                else if (grazeCounter < 0)
                {
                    grazeCounter = 0;
                }
                _OnGrazeCounterChange.Invoke(grazeCounter);
            }
        }

        [Header("Move")]
        [SerializeField]
        protected float m_MoveSpeed = 3f;
        public abstract float MoveSpeed { get; protected set; }

        [SerializeField]
        [Range(0, 1f)]
        protected float m_FocusModeScaleRate = 0.6f;
        public abstract float FocusModeScaleRate { get; protected set; }
        protected bool isFocusMode = false;
        public abstract bool IsFocusMode { get; set; }

        [SerializeField]
        protected LayerMask m_BorderLayer = 0;
        [SerializeField]
        protected float m_BorderCheckUp = 1;
        [SerializeField]
        protected float m_BorderCheckDown = 1;
        [SerializeField]
        protected float m_BorderCheckLeft = 1;
        [SerializeField]
        protected float m_BorderCheckRight = 1;

        [Header("Control")]
        [SerializeField]
        private InputActionReference m_MoverActionRef = null;
        protected InputAction m_MoveAction;
        public InputAction MoveAction { get {return m_MoveAction;} set { m_MoveAction = value; } }
        [SerializeField]
        private InputActionReference m_FocusModeActionRef = null;
        protected InputAction m_FocusModeAction;
        public InputAction FocusModeAction { get { return m_FocusModeAction; } set { m_FocusModeAction = value; } }
        [SerializeField]
        private InputActionReference m_FireActionRef = null;
        protected InputAction m_FireAction;
        public InputAction FireAction { get { return m_FireAction; } set { m_FireAction = value; } }

        [Header("Launcher")]
        [SerializeField]
        protected Launcher[] m_Launchers = null;
        public abstract Launcher[] Launchers { get; set; }

        [Header("GrazeCheck")]
        [SerializeField]
        protected float m_EngryAddPerGraze = 0.1f;
        [SerializeField]
        protected float m_GrazeCheckTiming = 0.1f;
        [SerializeField]
        protected float m_GrazeCheckRadius = 2f;
        [SerializeField]
        protected LayerMask m_GrazeCheckLayer = 0;
        [SerializeField]
        protected SFXEffecter m_GrazeEffect = null;
        protected StateController StateController = null;

        [Header("Events")]
        protected UnityEvent<float> _OnHPChange = new UnityEvent<float>();
        public abstract UnityEvent<float> OnHPChange { get; protected set; }
        protected UnityEvent<float> _OnGrazeCounterChange = new UnityEvent<float>();
        public abstract UnityEvent<float> OnGrazeCounterChange { get; protected set; }
        [SerializeField]
        protected UnityEvent<Collider2D[]> _OnGraze = new UnityEvent<Collider2D[]>();
        public abstract UnityEvent<Collider2D[]> OnGraze { get; protected set; }
        protected UnityEvent _OnPlayerDie = new UnityEvent();
        public abstract UnityEvent OnPlayerDie { get; protected set; }
        [SerializeField]
        protected UnityEvent<PlayableObject, float> _OnGetDamage = new UnityEvent<PlayableObject, float>();
        public abstract UnityEvent<PlayableObject, float> OnGetDamage { get; protected set; }
        protected UnityEvent _OnPlayerWakeUp = new UnityEvent();
        public virtual UnityEvent OnPlayerWakeUp { get { return _OnPlayerWakeUp; } protected set { } }
        protected UnityEvent _OnPlayerSleep = new UnityEvent();
        public virtual UnityEvent OnPlayerSleep { get { return _OnPlayerSleep; } protected set { } }

        /// <summary>
        /// Set player basic state data
        /// </summary>
        public virtual void WakeUpObject()
        {
            m_MoveAction.Enable();
            m_FocusModeAction.Enable();
            m_FireAction.Enable();
            HP = MaxHP;
            GrazeCounter = 0;
            Invincible = false;
            OnPlayerWakeUp?.Invoke();
        }

        public virtual void SleepObject()
        {
            m_MoveAction.Disable();
            m_FocusModeAction.Disable();
            m_FireAction.Disable();
            OnPlayerSleep?.Invoke();
        }

        public virtual void Initialization()
        {
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
            StateController = new StateController();
        }

        protected virtual void Update()
        {
            if (StateController != null)
            {
                StateController.Track();
            }
        }

        public virtual void BoderCheck(Vector3 currentPos)
        {
            var adjustPos = currentPos;
            var upHit = Physics2D.Raycast(currentPos,this.transform.up, m_BorderCheckUp, m_BorderLayer);
            if (upHit.collider != null)
            {
                var hitPoint = upHit.point;
                adjustPos.y = hitPoint.y - m_BorderCheckUp;
            } else
            {
                var downHit = Physics2D.Raycast(currentPos, -this.transform.up, m_BorderCheckDown, m_BorderLayer);
                if (downHit.collider != null)
                {
                    var hitPoint = downHit.point;
                    adjustPos.y = hitPoint.y + m_BorderCheckDown;
                }
            }

            var leftHit = Physics2D.Raycast(currentPos, -this.transform.right, m_BorderCheckLeft, m_BorderLayer);
            if (leftHit.collider != null)
            {
                var hitPoint = leftHit.point;
                adjustPos.x = hitPoint.x + m_BorderCheckLeft;
            } else
            {
                var rightHit = Physics2D.Raycast(currentPos, this.transform.right, m_BorderCheckRight, m_BorderLayer);
                if (rightHit.collider != null)
                {
                    var hitPoint = rightHit.point;
                    adjustPos.x = hitPoint.x - m_BorderCheckRight;
                }
            }

            // Final pos
            m_MoveTarget.localPosition = adjustPos;
        }

        public virtual IEnumerator GrazeChecking()
        {
            var checkDistence = new WaitForSeconds(m_GrazeCheckTiming);

            while(true)
            {
                yield return checkDistence;
                var currentPos = this.transform.localPosition;
                var checkTargets = Physics2D.OverlapCircleAll(currentPos, m_GrazeCheckRadius, m_GrazeCheckLayer);
                //  Filter player's bullet
                var findTargets = new List<Collider2D>();
                var checkCount = checkTargets.Length;
                for (int index = 0; index < checkCount; ++index)
                {
                    var target = checkTargets[index];
                    var bullet = target.GetComponent<Bullet>();
                    if (bullet != null)
                    {
                        if (bullet.m_BulletBelong != this.tag)
                        {
                            findTargets.Add(target);
                            if (m_GrazeEffect != null)
                            {
                                var grazeEffect = EffectFactory.GetEffect(m_GrazeEffect);
                                grazeEffect.transform.localPosition = target.ClosestPoint(this.transform.position);
                                grazeEffect.StartSFX();
                            }
                        }
                    }
                }

                var findCount = findTargets.Count;
                GrazeCounter += findCount * m_EngryAddPerGraze;
                if (findCount != 0)
                {
                    OnGraze.Invoke(findTargets.ToArray());
                }
            }
        }

        public virtual void GetHit(float dmg)
        {
            if (Invincible)
            {
                return;
            }
            HP -= dmg;
            OnGetDamage?.Invoke (this, dmg);
        }

        protected virtual void Die()
        {
            SleepObject();
        }

        protected virtual void OnDrawGizmos()
        {
            var centerPos = this.transform.localPosition;
            //  Draw graze field
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(centerPos, m_GrazeCheckRadius);

            Gizmos.color = Color.green;
            //  Border check Up
            Gizmos.DrawLine(centerPos, centerPos + Vector3.up * m_BorderCheckUp);
            //  Border check Down
            Gizmos.DrawLine(centerPos, centerPos + Vector3.down * m_BorderCheckDown);
            //  Border check Left
            Gizmos.DrawLine(centerPos, centerPos + Vector3.left * m_BorderCheckLeft);
            //  Border check Right
            Gizmos.DrawLine(centerPos, centerPos + Vector3.right * m_BorderCheckRight);
        }
    }
}
