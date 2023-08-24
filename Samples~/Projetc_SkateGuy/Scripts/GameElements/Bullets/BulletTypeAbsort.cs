using System.Collections;
using UnityEngine;
using GrazerCore.Factories;
using GrazerCore.Interfaces;
using GrazerCore.GameElements;

namespace SkateHero.GameElements
{
    public class BulletTypeAbsort : Bullet
    {
        [SerializeField]
        private CircleCollider2D m_Collider = null;
        [SerializeField]
        private LayerMask m_AbsortTarget = 0;
        [SerializeField]
        private LayerMask m_DamageTarget = 0;
        [SerializeField]
        private float m_CheckUpdateTime = 0.1f;
        [SerializeField]
        private float m_BulletLifeTime = 3f;
        [SerializeField]
        private int m_MaxAbsortCount = 50;
        [SerializeField]
        private float m_DamageUpRatePerAbsort = 0.5f;
        [SerializeField]
        private float m_SizeUpRatePerAbsort = 0.1f;

        private Coroutine absortCheckRoutine = null;
        private float originalDamage = 0;
        private Vector3 originalSize = Vector3.zero;

        private void Awake()
        {
            originalDamage = m_Damage;
            originalSize = this.transform.localScale;
        }

        public override void WakeUpBullet()
        {
            m_Damage = originalDamage;
            this.transform.localScale = originalSize;
            absortCheckRoutine = StartCoroutine(AbsortChecking());
        }

        protected override void BulletDead()
        {
            base.BulletDead();
            if (absortCheckRoutine != null)
            {
                StopCoroutine(absortCheckRoutine);
            }
        }

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            
        }

        private IEnumerator AbsortChecking()
        {
            var waitTime = new WaitForSeconds(m_CheckUpdateTime);
            var totalTime = 0f;
            var absortCount = 0;
            while (true)
            {
                yield return waitTime;
                totalTime += m_CheckUpdateTime;

                var targets = Physics2D.OverlapCircleAll
                    (this.transform.position,
                    m_Collider.radius * this.transform.localScale.x,
                    m_AbsortTarget);
                int targetCount = targets.Length;
                for (int index = 0; index < targetCount; ++index)
                {
                    var target = targets[index];
                    var recycleable = target.GetComponent<IRecycleable>();
                    var bullet = target.GetComponent<Bullet>();
                    var isRecycleable = false;
                    if (target != this.m_Collider)
                    {
                        if (bullet == null)
                        {
                            isRecycleable = RecycelCountAdd(recycleable);
                        } else if (bullet.m_BulletBelong != this.m_BulletBelong)
                        {
                            isRecycleable = RecycelCountAdd(recycleable);
                        }
                        if (isRecycleable)
                        {
                            absortCount++;
                            if (m_HitEffect != null)
                            {
                                var hitEffect = EffectFactory.GetEffect(m_HitEffect);
                                hitEffect.transform.localPosition = target.ClosestPoint(this.transform.position);
                                hitEffect.StartSFX();
                            }
                            if (m_PickUpObject != null)
                            {
                                var drop = PickUpObjectFactory.GetPickUpObject(m_PickUpObject);
                                drop.transform.localPosition = target.ClosestPoint(this.transform.position);
                            }
                        }
                    }
                }
                if (absortCount > m_MaxAbsortCount)
                {
                    absortCount = m_MaxAbsortCount;
                }
                m_Damage = originalDamage + absortCount * m_DamageUpRatePerAbsort;
                this.transform.localScale = originalSize + (absortCount * m_SizeUpRatePerAbsort) *Vector3.one;

                bool RecycelCountAdd(IRecycleable recycleable)
                {
                    if (recycleable != null)
                    {
                        recycleable.Recycle();
                        return true;
                    } else
                    {
                        return false;
                    }
                }

                //  Search danaged target
                var canDamageTargets = Physics2D.OverlapCircleAll(
                    this.transform.position,
                    m_Collider.radius * this.transform.localScale.x,
                    m_DamageTarget);
                int damageTargetCount = canDamageTargets.Length;
                for (int index = 0; index < damageTargetCount; ++index)
                {
                    var target = canDamageTargets[index];
                    var damageable = target.GetComponent<IDamageable>();
                    damageable?.GetHit(m_Damage);
                    if (damageable != null && m_HitEffect != null)
                    {
                        var hitEffect = EffectFactory.GetEffect(m_HitEffect);
                        hitEffect.transform.localPosition = target.ClosestPoint(this.transform.position);
                        hitEffect.StartSFX();
                    }
                    if (m_PickUpObject != null)
                    {
                        var drop = PickUpObjectFactory.GetPickUpObject(m_PickUpObject);
                        drop.transform.localPosition = target.ClosestPoint(this.transform.position);
                    }
                }

                if (totalTime >= m_BulletLifeTime)
                {
                    BulletDead();
                    break;
                }
            }
        }
    }
}
