using UnityEngine;
using System.Collections;
using GrazerCore.Factories;
using GrazerCore.Interfaces;

namespace GrazerCore.GameElements
{
    public class Laser : Bullet
    {
        [SerializeField]
        private float m_DamageCoolDown = 0.2f;
        [SerializeField]
        private float m_MinimaltLaserLength = 1;
        [SerializeField]
        private SpriteRenderer m_LaserRender = null;
        [SerializeField]
        private BoxCollider2D m_LaserCollider = null;
        [SerializeField][Tooltip("Define laser width(horizontal view)")]
        private float m_LaserSize = 5;
        [SerializeField]
        [Range(0, 1)][Tooltip("Scale laser collider with size")]
        private float m_HitBoxScale = 0.8f;
        [SerializeField]
        private float m_LaserMaxDistence = 10;
        [SerializeField][Tooltip("Effect target layer")]
        private LayerMask m_TargetLayer = 0;
        private float laserLength = 0;
        private float LaserLength
        {
            get { return laserLength; }
            set
            {
                if (value > m_LaserMaxDistence)
                {
                    laserLength = m_LaserMaxDistence;
                }   else if (value < m_MinimaltLaserLength)
                {
                    laserLength = m_MinimaltLaserLength;
                }   else
                {
                    laserLength = value;
                }

                m_LaserRender.size = new Vector2(laserLength, m_LaserSize);
                var laserOffsetX = m_LaserRender.size.x / 2;
                m_LaserCollider.offset = new Vector2(laserOffsetX,
                    m_LaserCollider.offset.y);
                m_LaserCollider.size = new Vector2(
                    m_LaserRender.size.x,
                    m_LaserRender.size.y * m_HitBoxScale
                );
            }
        }
        private float lengthToTarget = 0;
        private Transform hitTargetTransform = null;
        private Coroutine damageRoutine = null;

        private void OnDisable()
        {
            if (damageRoutine != null)
            {
                StopCoroutine(damageRoutine);
            }
        }

        public override void WakeUpBullet()
        {
            lengthToTarget = m_LaserMaxDistence;
            LaserLength = m_MinimaltLaserLength;
            damageRoutine = null;
            if (m_Penetrate)
            {
                damageRoutine = StartCoroutine(DamagingTypePenetrate());
            } else
            {
                damageRoutine = StartCoroutine(Damaging());
            }
        }

        public void StopLaser()
        {
            hitTargetTransform = null;
            BulletDead();
        }

        #region Clear Function
        /*
         *  Laser no need to use those function.
         */
        protected override void Update()
        {
            //  Do nothing
        }

        public override void Recycle()
        {
            //  Do nothing
        }

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            //  Do nothing
        }
        #endregion

        private void LaserHitCheck()
        {
            var selfPos = this.transform.position;
            var searchRadius = m_LaserSize / 2 * m_HitBoxScale;
            var searchDistence = m_LaserMaxDistence - m_LaserSize / 2;
            var hittarget = Physics2D.CircleCast(selfPos,
                searchRadius, transform.right, searchDistence, m_TargetLayer);
            hitTargetTransform = hittarget.transform;
            if (hitTargetTransform != null)
            {
                var toHitPoint = (Vector3)hittarget.point - selfPos;
                var newLengrh = Vector2.Dot(transform.right, toHitPoint.normalized)
                    * Vector2.Distance(selfPos, hittarget.point);
                lengthToTarget = newLengrh;
            } else
            {
                lengthToTarget = m_LaserMaxDistence;
            }
            //  Set laser size
            if (LaserLength > lengthToTarget)
            {
                LaserLength = lengthToTarget;
            }
            else if (LaserLength < lengthToTarget)
            {
                LaserLength += Time.deltaTime * m_MoveSpeed;
            }
        }

        private void LaserHitCheckTypePenetrate()
        {
            var selfPos = this.transform.position;
            var searchRadius = m_LaserSize / 2 * m_HitBoxScale;
            var searchDistence = LaserLength - m_LaserSize / 2;
            var hittargets = Physics2D.CircleCastAll(selfPos,
                searchRadius, transform.right, searchDistence, m_TargetLayer);
            var hitCount = hittargets.Length;
            for (int index = 0; index < hitCount; index++)
            {
                var chekTarget = hittargets[index].transform;
                if (m_BulletBelong != chekTarget.tag)
                {
                    var damageable = chekTarget.GetComponent<IDamageable>();
                    if (damageable != null)
                    {
                        if (m_OnBulletHit != null)
                        {
                            m_OnBulletHit.Invoke();
                        }
                        damageable.GetHit(m_Damage);
                        if (m_HitEffect != null)
                        {
                            var hitEffect = EffectFactory.GetEffect(m_HitEffect);
                            hitEffect.transform.position = chekTarget.position;
                            var moveDir = this.transform.right;
                            var setAngle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
                            hitEffect.transform.rotation = Quaternion.Euler(new Vector3(0, 0, setAngle));
                            hitEffect.StartSFX();
                        }
                        if (m_PickUpObject != null)
                        {
                            var drop = PickUpObjectFactory.GetPickUpObject(m_PickUpObject);
                            drop.transform.localPosition = chekTarget.position;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// For laser with no penetrate
        /// </summary>
        private void DoDamage()
        {
            if (m_OnBulletHit != null)
            {
                m_OnBulletHit.Invoke();
            }

            var damageable = hitTargetTransform.GetComponent<IDamageable>();
            if (hitTargetTransform.gameObject.activeInHierarchy)
            {
                damageable?.GetHit(m_Damage);
            }
            if (m_HitEffect != null)
            {
                var hitEffect = EffectFactory.GetEffect(m_HitEffect);
                hitEffect.transform.position = this.transform.position + this.transform.right * LaserLength;
                var moveDir = this.transform.right;
                var setAngle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
                hitEffect.transform.rotation = Quaternion.Euler(new Vector3(0, 0, setAngle));
                hitEffect.StartSFX();
            }
            if (m_PickUpObject != null)
            {
                var drop = PickUpObjectFactory.GetPickUpObject(m_PickUpObject);
                drop.transform.localPosition = this.transform.position + this.transform.right * LaserLength;
            }
        }

        private IEnumerator Damaging()
        {
            var checkCounter = 0f;
            while (true)
            {
                yield return null;
                LaserHitCheck();

                if (checkCounter < m_DamageCoolDown)
                {
                    checkCounter += Time.deltaTime;
                }
                else
                {
                    checkCounter = 0;
                    //  Damage check
                    if (hitTargetTransform != null)
                    {
                        DoDamage();
                    }
                }
            }
        }

        private IEnumerator DamagingTypePenetrate()
        {
            var checkCounter = 0f;
            while (true)
            {
                yield return null;

                if (LaserLength < m_LaserMaxDistence)
                {
                    LaserLength += Time.deltaTime * m_MoveSpeed;
                }
                else if (LaserLength >= m_LaserMaxDistence)
                {
                    LaserLength = m_LaserMaxDistence;
                }

                if (checkCounter < m_DamageCoolDown)
                {
                    checkCounter += Time.deltaTime;
                } else
                {
                    checkCounter = 0;
                    LaserHitCheckTypePenetrate();
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            var selfPos = this.transform.position;
            Gizmos.DrawLine(selfPos, selfPos + (Vector3)(transform.right * m_LaserMaxDistence));
            Gizmos.DrawWireSphere(selfPos + (Vector3)(transform.right * (m_LaserMaxDistence - m_LaserSize / 2)), m_LaserSize / 2 * m_HitBoxScale);
        }
    }
}
