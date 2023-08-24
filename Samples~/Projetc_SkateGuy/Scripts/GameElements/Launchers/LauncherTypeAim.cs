using UnityEngine;
using GrazerCore.GameElements;

namespace SkateHero.GameElements
{
    public class LauncherTypeAim : Launcher
    {
        [SerializeField]
        private Transform m_RotatePart = null;
        [SerializeField]
        private float m_AimRadius = 200;
        [SerializeField]
        private LayerMask m_AimMask = 0;
        private Transform aimTarget = null;

        private void Aim()
        {
            var selfPos = this.transform.position;
            var findTarget = Physics2D.OverlapCircle(selfPos, m_AimRadius, m_AimMask);
            aimTarget = findTarget?.transform;
            if (aimTarget != null)
            {
                var subtraction = aimTarget.transform.position - this.transform.position;
                var myAngle = Mathf.Atan2(subtraction.y, subtraction.x) * Mathf.Rad2Deg;
                m_RotatePart.localEulerAngles = Vector3.forward * myAngle;
                m_BaseFireDirection = this.transform.right;
            }
        }

        public override void HoldTrigger()
        {
            Aim();
            base.HoldTrigger();
        }
    }
}
