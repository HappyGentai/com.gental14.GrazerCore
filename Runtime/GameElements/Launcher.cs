using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using GrazerCore.Factories;

namespace GrazerCore.GameElements
{
    public class Launcher : MonoBehaviour
    {
        [SerializeField]
        protected string m_LauncherBelong = "";
        [SerializeField]
        protected Vector3 m_BaseFireDirection = Vector3.right;
        [SerializeField]
        protected FireSpot[] m_FireSpots = null;
        [SerializeField]
        protected float m_FireRecall = 0.1f;
        [SerializeField]
        protected UnityEvent m_OnFiring = null;

        protected float fireCounter = 0;
        private Coroutine workingRoutine = null;

        private bool isWorking = false;
        public bool IsWorking
        {
            get { return isWorking; }
        }

        [SerializeField]
        protected bool launcherLock = false;
        public bool LauncherLock
        {
            set
            {
                launcherLock = value;
            }
        }

        public virtual void AwakeLauncher()
        {
            StopLauncher();
            fireCounter = m_FireRecall;
            workingRoutine = StartCoroutine(LauncherWorking());
            isWorking = true;
            LauncherLock = false;
        }

        public virtual void StopLauncher()
        {
            if (workingRoutine != null)
            {
                StopCoroutine(workingRoutine);
                workingRoutine = null;
            }
            isWorking = false;
        }

        /// <summary>
        /// If want set first trigger event, write here
        /// </summary>
        public virtual void StartTrigger()
        {

        }

        public virtual void HoldTrigger()
        {
            if (fireCounter >= m_FireRecall && !launcherLock)
            {
                m_OnFiring?.Invoke();
                fireCounter = 0;
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
            }
        }

        /// <summary>
        /// If want set stop trigger event, write here
        /// </summary>
        public virtual void ReleaseTrigger()
        {

        }

        private IEnumerator LauncherWorking()
        {
            while (true)
            {
                yield return null;

                if (fireCounter < m_FireRecall && !launcherLock)
                {
                    fireCounter += Time.deltaTime;
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1, 0, 0, 0.25f);
            Gizmos.DrawSphere(this.transform.position, 0.25f);

            //  Draw fire spot(If have)
            var selfPos = this.transform.position;
            Gizmos.color = Color.green;
            int spotCount = m_FireSpots.Length;
            for (int index = 0; index < spotCount; ++index)
            {
                var fireSpot = m_FireSpots[index];
                var firePoint = selfPos + (Vector3)fireSpot.FirePoint;
                var firDir = Quaternion.AngleAxis(fireSpot.FireAngle, Vector3.forward) * m_BaseFireDirection;
                Gizmos.DrawLine(firePoint, firePoint + firDir * 5);
            }
        }
    }

    [System.Serializable]
    public class FireSpot
    {
        [SerializeField]
        private Bullet m_FireBullet = null;
        public Bullet FireBullet
        {
            get { return m_FireBullet; }
        }
        [SerializeField]
        private Vector2 m_FirePoint = Vector2.zero;
        public Vector2 FirePoint
        {
            get { return m_FirePoint; }
        }
        [SerializeField]
        private float m_FireAngle = 0;
        public float FireAngle
        {
            get { return m_FireAngle; }
        }
    }
}
