using System.Collections;
using UnityEngine;
using GrazerCore.GameElements;

namespace SkateHero.GameElements
{
    public class SupportBox : PickUpObject
    {
        [SerializeField]
        private LayerMask m_TargetLayer = 0;
        [SerializeField]
        private SpriteRenderer m_Render = null;
        [SerializeField]
        private float m_HealValue = 30;
        [SerializeField]
        private float m_GrazeCounterFillValue = 30;
        [SerializeField]
        private float m_ExGaugeFillValue = 30;
        [SerializeField]
        private Vector2 m_FlyDir = Vector2.left;
        [SerializeField]
        private float m_FlySpeed = 3;
        [SerializeField]
        private float m_ReleaseDelay = 0.6f;
        private bool isPicked = false;
        private Coroutine workingRoutine = null;
        private BasicPlayer targetPlayer = null;

        private void Update()
        {
            if (!isPicked)
            {
                this.transform.localPosition += (Vector3)m_FlyDir * m_FlySpeed * Time.deltaTime;
            }
        }

        public override void Picked()
        {
            targetPlayer.HP += m_HealValue;
            targetPlayer.GrazeCounter += m_GrazeCounterFillValue;
            targetPlayer.ExGuage+= m_ExGaugeFillValue;
            m_Render.enabled = false;
            workingRoutine = StartCoroutine(DelayRelease());
        }

        public override void ReSetState()
        {
            if (workingRoutine != null)
            {
                StopCoroutine(workingRoutine);
                workingRoutine = null;
            }
            m_Render.enabled = true;
            isPicked = false;
            targetPlayer = null;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (((1 << collision.gameObject.layer) & m_TargetLayer) == 0)
            {
                return;
            }
            targetPlayer = collision.GetComponent<BasicPlayer>();
            if (targetPlayer != null)
            {
                isPicked = true;
                OnPick();
            }
        }

        private IEnumerator DelayRelease()
        {
            yield return new WaitForSeconds(m_ReleaseDelay);
            Recycle();
        }
    }
}
