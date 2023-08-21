using UnityEngine;
using GrazerCore.Interfaces;

namespace GrazerCore.Fields
{
    public class ShootLockArea : MonoBehaviour
    {
        [SerializeField]
        private LayerMask m_TargetMask = 0;
        [SerializeField]
        private bool m_Reverse = false;
        [SerializeField]
        private bool m_LockEnter = false;
        [SerializeField]
        private bool m_LockExit = false;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (((1 << collision.gameObject.layer) & m_TargetMask) == 0 || m_LockEnter)
            {
                return;
            }
            var invincibleObject = collision.gameObject.GetComponent<IShootable>();
            if (invincibleObject != null)
            {
                var lockWeapon = m_Reverse;
                invincibleObject.CanShoot(lockWeapon);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (((1 << collision.gameObject.layer) & m_TargetMask) == 0 || m_LockExit)
            {
                return;
            }
            var invincibleObject = collision.gameObject.GetComponent<IShootable>();
            if (invincibleObject != null)
            {
                var lockWeapon = !m_Reverse;
                invincibleObject.CanShoot(lockWeapon);
            }
        }
    }
}
