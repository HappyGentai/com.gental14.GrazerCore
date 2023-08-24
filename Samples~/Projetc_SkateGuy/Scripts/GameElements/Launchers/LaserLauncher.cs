using UnityEngine;
using UnityEngine.Events;
using GrazerCore.Factories;
using GrazerCore.GameElements;

namespace SkateHero.GameElements
{

    public class LaserLauncher : Launcher
    {
        [SerializeField]
        private UnityEvent m_OnStopFire = new UnityEvent();
        private Laser[] lasers = null;
        private bool fireing = false;

        public override void StartTrigger()
        {
            fireing = true;
            m_OnFiring?.Invoke();
            //  Start shoot laser
            int spotCount = m_FireSpots.Length;
            var selfPos = this.transform.position;
            lasers = new Laser[spotCount];
            for (int index = 0; index < spotCount; ++index)
            {
                var fireSpot = m_FireSpots[index];
                var firDir = Quaternion.AngleAxis(fireSpot.FireAngle, Vector3.forward) * m_BaseFireDirection;
                var laser = BulletFactory.GetBullet(fireSpot.FireBullet);
                laser.m_BulletBelong = m_LauncherBelong;
                laser.MoveDir = firDir;
                laser.transform.parent = this.transform;
                laser.transform.localPosition = Vector3.zero + (Vector3)fireSpot.FirePoint;
                laser.WakeUpBullet();
                lasers[index] = (Laser)laser;
            }
        }

        /// <summary>
        /// For some enemy logic only use hold trigger func.
        /// </summary>
        public override void HoldTrigger()
        {
            if (!fireing)
            {
                StartTrigger();
                fireing = true;
            }
        }

        public override void ReleaseTrigger()
        {
            if (lasers == null)
            {
                return;
            }
            m_OnStopFire?.Invoke();
            //  stop shoot laser
            var laserCount = lasers.Length;
            for (int index = 0; index < laserCount; ++index)
            {
                var laser = lasers[index];
                laser.transform.parent = null;
                laser.StopLaser();
            }
            fireing = false;
        }

        public override void StopLauncher()
        {
            base.StopLauncher();
            ReleaseTrigger();
        }
    }
}
