using UnityEngine;
using System.Collections;

namespace GrazerCore.GameElements
{
    public class BulletTypeRotateChase : Bullet
    {
        [SerializeField]
        private float m_RotateSpeed = 0;
        [SerializeField]
        private LayerMask m_TargetLayer = 0;
        [SerializeField]
        private Vector2 m_ChaseRange = Vector2.one;
        private Transform m_ChaseTarget = null;
        [SerializeField]
        private float m_ChaseTime = 2f;
        private float lerpProgress = 0;
        [SerializeField]
        private bool m_DelayChase = false;
        [SerializeField]
        private float m_DelayTime = 0.5f;
        [SerializeField]
        private bool m_GraduallyAccelerate = false;
        [SerializeField]
        [Range(0, 1)]
        private float m_StartSpeedRate = 0.5f;
        private float speedScale = 0;
        [SerializeField]
        private float m_AddSpeedRatePerFrame = 0.01f;
        private Coroutine speedUpRoutine = null;

        public override void WakeUpBullet()
        {
            speedScale = 1;
            lerpProgress = 0;
            if (m_DelayChase)
            {
                StartCoroutine(DelayChaseing());
            } else
            {
                SearchTarget();
                DoSeekTimeCountDown();
            }

            if (m_GraduallyAccelerate)
            {
                speedUpRoutine = StartCoroutine(SpeedAdding());
            }
        }

        protected override void Update()
        {
            if (m_ChaseTarget != null)
            {
                if (m_ChaseTarget.gameObject.activeInHierarchy)
                {
                    var selfPos = this.transform.localPosition;
                    var targetPos = m_ChaseTarget.localPosition;
                    lerpProgress += Time.deltaTime * m_RotateSpeed;
                    var targetDir = targetPos - selfPos;
                    var angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
                    Quaternion targetQut = Quaternion.Euler(new Vector3(0, 0, angle));
                    transform.rotation = Quaternion.Lerp(transform.rotation, targetQut, lerpProgress);
                }
            }   
            this.transform.localPosition += transform.right * Time.deltaTime * m_MoveSpeed * speedScale;
        }

        protected override void BulletDead()
        {
            base.BulletDead();
            m_ChaseTarget = null;
            if (speedUpRoutine != null)
            {
                StopCoroutine(speedUpRoutine);
            }
        }

        protected void SearchTarget()
        {
            var selfPos = this.transform.localPosition;
            var findTarget = Physics2D.OverlapBox(Vector2.zero, m_ChaseRange, 0, m_TargetLayer);
            if (findTarget != null)
            {
                m_ChaseTarget = findTarget.transform;
            }
        }

        protected void DoSeekTimeCountDown()
        {
            StartCoroutine(SeekTimeCountDowning());
        }

        protected IEnumerator SeekTimeCountDowning()
        {
            yield return new WaitForSeconds(m_ChaseTime);
            m_ChaseTarget = null;
        }

        protected IEnumerator DelayChaseing()
        {
            yield return new WaitForSeconds(m_DelayTime);
            SearchTarget();
            DoSeekTimeCountDown();
        }

        protected IEnumerator SpeedAdding()
        {
            speedScale = m_StartSpeedRate;
            while(speedScale < 1)
            {
                yield return null;
                speedScale += Time.deltaTime * m_AddSpeedRatePerFrame;
            }
            speedScale = 1;
            speedUpRoutine = null;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1, 0, 0, 0.25f);
            Gizmos.DrawCube(Vector3.zero, m_ChaseRange);
        }
    }
}
