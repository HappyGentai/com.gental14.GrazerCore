using UnityEngine;
using GrazerCore.GameElements.States;
using GrazerCore.Effects;
using GrazerCore.Factories;
using UnityEngine.Events;
using GrazerCore.Interfaces;

namespace GrazerCore.GameElements
{
    public abstract class Enemy : MonoBehaviour, IRecycleable, IDamageable, IInvincible, IShootable
    {
        [SerializeField]
        protected Transform m_MoveTarget = null;
        public abstract Transform MoveTarget { get; protected set; }
        [SerializeField]
        protected CircleCollider2D m_HitBox = null; 
        public abstract CircleCollider2D HitBox { get; protected set; }
        [Header("State")]
        [SerializeField]
        protected float m_MaxHP = 100;
        public abstract float MaxHP { get; protected set; }
        [SerializeField]
        protected float m_HP = 0;
        public virtual float HP
        {
            get { return m_HP; }
            set
            {
                m_HP = value;
                if (m_HP >= m_MaxHP)
                {
                    m_HP = m_MaxHP;
                }
                else if (m_HP < 0)
                {
                    m_HP = 0;
                }
                _OnHPChange.Invoke(m_HP);
                if (m_HP <= 0)
                {
                    Die();
                }
            }
        }
        [SerializeField]
        protected float m_MoveSpeed = 1f;
        public virtual float MoveSpeed {
            get {
                return m_MoveSpeed;
            }
            protected set
            {
                m_MoveSpeed = value;
            }
         }
        [SerializeField]
        protected float m_CloseDamage = 1;
        [SerializeField]
        protected LayerMask m_CloseDamageTarget = 0;
        [Header("Launcher")]
        [SerializeField]
        protected Launcher[] m_Launchers = null;
        public virtual Launcher[] Launchers
        {
            get { return m_Launchers; }
            set
            {
                m_Launchers = value;
            }
        }

        protected bool isInvincible = false;

        protected StateController StateController = null;

        [Header("DieEffect")]
        [SerializeField]
        protected SFXEffecter m_DieEffect = null;
        [Header("DieDrop")]
        [SerializeField]
        protected PickUpObject m_DieDrop = null;

        [Header("Events")]
        protected UnityEvent<float> _OnHPChange = new UnityEvent<float>();
        public virtual UnityEvent<float> OnHPChange
        {
            get { return _OnHPChange; }
            protected set
            {
                _OnHPChange = value;
            }
        }
        [SerializeField]
        protected UnityEvent<float> m_OnGetDamaged = new UnityEvent<float>();
        public virtual UnityEvent<float> OnGetDamaged { get { return m_OnGetDamaged; } protected set { } }
        [SerializeField]
        protected UnityEvent _OnEnemyDie = new UnityEvent();
        public virtual UnityEvent OnEnemyDie { get { return _OnEnemyDie; } protected set { } }
        protected UnityEvent<Enemy> onRecycle = new UnityEvent<Enemy>();
        public  UnityEvent<Enemy> OnRecycle
        {
            get { return onRecycle; }
        }

        public virtual void WakeUpObject()
        {
            HP = MaxHP;
            m_HitBox.enabled = true;
        }

        public virtual void SleepObject()
        {
            m_HitBox.enabled = false;
            StateController.SetState(null);
        }

        public virtual void Initialization()
        {
            StateController = new StateController();
        }

        /// <summary>
        /// Used to call enemy to start(Set start state)
        /// </summary>
        public abstract void StartAction();

        protected virtual void Update()
        {
            StateController.Track();
        }

        public virtual void Recycle()
        {
            OnRecycle.Invoke(this);
        }

        public virtual void SetInvincible(bool _isInvincible)
        {
            isInvincible = _isInvincible;
        }

        public virtual void CanShoot(bool _canShoot)
        {
            var launcherCount = Launchers.Length;
            for (int index = 0; index < launcherCount; ++index)
            {
                var launcher = Launchers[index];
                launcher.LauncherLock = !_canShoot;
            }
        }

        public virtual void GetHit(float dmg)
        {
            if (isInvincible)
            {
                return;
            }
            HP -= dmg;
            m_OnGetDamaged?.Invoke(dmg);
        }

        protected virtual void Die()
        {
            if (m_DieEffect != null)
            {
                var dieEffect = EffectFactory.GetEffect(m_DieEffect);
                dieEffect.transform.localPosition = this.MoveTarget.localPosition;
                dieEffect.StartSFX();
            }
            if (m_DieDrop != null)
            {
                var dieDrop = PickUpObjectFactory.GetPickUpObject(m_DieDrop);
                dieDrop.transform.localPosition = this.MoveTarget.localPosition;
            }
            OnEnemyDie.Invoke();
            Recycle();
        }

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            var targetGO = collision.gameObject;
            if (((1 << targetGO.layer) & m_CloseDamageTarget) == 0)
            {
                return;
            }

            var damageable = targetGO.GetComponent<IDamageable>();
            damageable?.GetHit(m_CloseDamage);
        }
    }
}
