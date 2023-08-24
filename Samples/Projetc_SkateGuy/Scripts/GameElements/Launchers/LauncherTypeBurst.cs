using System.Collections;
using UnityEngine;
using GrazerCore.Factories;
using GrazerCore.GameElements;

namespace SkateHero.GameElements
{
    public class LauncherTypeBurst : Launcher
    {
        [SerializeField]
        private int m_BurstCount = 3;
        [SerializeField]
        private float m_FireDelay = 0.1f;
        private bool bursting = false;
        private Coroutine burstRoutine = null;

        public override void HoldTrigger()
        {
            if (fireCounter >= m_FireRecall && !launcherLock && !bursting)
            {
                fireCounter = 0;
                StartBurst();
            }
        }

        public override void StopLauncher()
        {
            base.StopLauncher();
            //  Stop burst
            StopBurst();
        }

        private void StartBurst()
        {
            StopBurst();
            bursting = true;
            burstRoutine = StartCoroutine(Bursting());
        }

        private void StopBurst()
        {
            if (burstRoutine != null)
            {
                StopCoroutine(burstRoutine);
            }
            bursting = false;
        }

        private IEnumerator Bursting()
        {
            var burstCount = m_BurstCount;
            var fireCounter = m_FireDelay;
            while (burstCount > 0 )
            {
                yield return null;
                fireCounter += Time.deltaTime;
                if (fireCounter >= m_FireDelay)
                {
                    if (m_OnFiring != null)
                    {
                        m_OnFiring.Invoke();
                    }
                    int spotCount = m_FireSpots.Length;
                    var selfPos = this.transform.position;
                    for (int index = 0; index < spotCount; ++index)
                    {
                        var fireSpot = m_FireSpots[index];
                        var firDir = Quaternion.AngleAxis(fireSpot.FireAngle, Vector3.forward) * m_BaseFireDirection;
                        var bullet = BulletFactory.GetBullet(fireSpot.FireBullet);
                        bullet.m_BulletBelong = m_LauncherBelong;
                        bullet.MoveDir = firDir;
                        bullet.transform.position = this.transform.position + (Vector3)fireSpot.FirePoint;
                        bullet.WakeUpBullet();
                    }
                    fireCounter = 0;
                    burstCount--;
                }
            }
            bursting = false;
            burstRoutine = null;
        }
    }
}
