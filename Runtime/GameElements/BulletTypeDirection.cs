using UnityEngine;

namespace GrazerCore.GameElements
{
    public class BulletTypeDirection : Bullet
    {
        [SerializeField]
        private LayerMask m_TargetLayer = 0;
        [SerializeField]
        private Vector2 m_SeekRange = Vector2.one;
        private Transform seekTarget = null;

        public override void WakeUpBullet()
        {
            SearchTarget();
        }

        protected override void Update()
        {  
            if (seekTarget == null)
            {
                MoveDir = this.transform.right;
            } else
            {
                MoveDir = MoveDir.normalized;
            }
            this.transform.localPosition += (Vector3)(MoveDir * m_MoveSpeed) * Time.deltaTime;
        }

        protected override void BulletDead()
        {
            base.BulletDead();
            seekTarget = null;
        }

        private void SearchTarget()
        {
            var selfPos = this.transform.localPosition;
            var findTarget = Physics2D.OverlapBox(Vector2.zero, m_SeekRange, 0, m_TargetLayer);
            if (findTarget != null)
            {
                seekTarget = findTarget.transform;
                MoveDir = (seekTarget.localPosition - selfPos).normalized;
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1, 0, 0, 0.25f);
            Gizmos.DrawCube(Vector3.zero, m_SeekRange);
        }
    }
}
