using UnityEngine;
using GrazerCore.Tool;
using System.Collections;

namespace GrazerCore.GameElements
{
    public class BulletTypeSeek : Bullet
    {
        [SerializeReference]
        private float m_MaxSeekForce = 0.1f;
        [SerializeField]
        private LayerMask m_TargetLayer = 0;
        [SerializeField]
        private Vector2 m_SeekRange = Vector2.one;
        private Transform seekTarget = null;
        [SerializeField]
        private bool m_DelaySeek = false;
        [SerializeField]
        private float m_DelayTime = 0.5f;
        [SerializeField]
        private float m_SeekTime = 2f;

        public override void WakeUpBullet()
        {
            //  Search target
            if (!m_DelaySeek)
            {
                SeekTarget();
                DoSeekTimeCountDown();
            }
            else
            {
                StartCoroutine(DelaySeeking());
            }
        }

        protected override void Update()
        {
            var moveVel = MoveDir;
            if (seekTarget == null)
            {
                moveVel = MoveDir.normalized * m_MoveSpeed;
            } else if (seekTarget.gameObject.activeInHierarchy)
            {
                var selfPos = this.transform.localPosition;
                var targetPos = seekTarget.localPosition;
                MoveDir = SteeringBehaviors.Seek(selfPos, targetPos, MoveDir, m_MoveSpeed, m_MaxSeekForce);
                moveVel = MoveDir;
            } else
            {
                moveVel = moveVel.normalized;
                moveVel *= m_MoveSpeed;
            }
            this.transform.localPosition += (Vector3)moveVel * Time.deltaTime;
        }

        protected override void BulletDead()
        {
            base.BulletDead();
            seekTarget = null;
        }

        protected void SeekTarget()
        {
            var selfPos = this.transform.localPosition;
            var findTarget = Physics2D.OverlapBox(Vector2.zero, m_SeekRange, 0, m_TargetLayer);
            if (findTarget != null)
            {
                seekTarget = findTarget.transform;
            }
        }

        protected void DoSeekTimeCountDown()
        {
            StartCoroutine(SeekTimeCountDowning());
        }

        protected IEnumerator DelaySeeking()
        {
            yield return new WaitForSeconds(m_DelayTime);
            SeekTarget();
            DoSeekTimeCountDown();
        }

        protected IEnumerator SeekTimeCountDowning()
        {
            yield return new WaitForSeconds(m_SeekTime);
            seekTarget = null;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1, 0, 0, 0.25f);
            Gizmos.DrawCube(Vector3.zero, m_SeekRange);
        }
    }
}
