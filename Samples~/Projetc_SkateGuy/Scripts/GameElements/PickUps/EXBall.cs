using UnityEngine;
using GrazerCore.GameElements;

namespace SkateHero.GameElements
{
    public class EXBall : PickUpObject
    {
        [SerializeField]
        private float m_EXValue = 0;
        [SerializeField]
        private float m_CollectDistence = 0.1f;
        [SerializeField]
        private float m_FlySpeed = 3;
        [SerializeField]
        private Vector2 m_ChaseRange = Vector2.one;
        [SerializeField]
        private LayerMask m_SearchTargetMask = 0;
        private BasicPlayer targetPlayer = null;
        [SerializeField]
        private float m_ReleaseDelay = 0.6f; 
        [SerializeField]
        private SpriteRenderer m_SpriteRender = null;
        [SerializeField]
        private TrailRenderer m_TrailRenderer = null;
        private bool isPicked = false;
        private Coroutine workingRoutine = null;

        public override void Picked()
        {
            if (targetPlayer == null)
            {
                return;
            }
            targetPlayer.ExGuage += m_EXValue;
            m_TrailRenderer.Clear();
            m_SpriteRender.enabled = false;
            m_TrailRenderer.enabled = false;
            workingRoutine = StartCoroutine(DelayRelease());
        }

        private void Update()
        {
            if (targetPlayer != null && !isPicked)
            {
                var targetPos = targetPlayer.transform.position;
                var selfPos = this.transform.position;
                var distence = Vector2.Distance(targetPos ,selfPos);
                if (distence <= m_CollectDistence)
                {
                    OnPick();
                    isPicked = true;
                    return;
                }
                var moveDir = targetPos - selfPos;
                this.transform.position += moveDir.normalized * m_FlySpeed * Time.deltaTime;
            }
        }

        public override void ReSetState()
        {
            if (workingRoutine != null)
            {
                StopCoroutine(workingRoutine);
                workingRoutine = null;
            }
            isPicked = false;
            m_SpriteRender.enabled = true;
            m_TrailRenderer.enabled = true;
            targetPlayer = null;

            var selfPos = this.transform.localPosition;
            var findTarget = Physics2D.OverlapBox(Vector2.zero, m_ChaseRange, 0, m_SearchTargetMask);
            if (findTarget != null)
            {
                targetPlayer = findTarget.gameObject.GetComponent<BasicPlayer>();
            }
            else
            {
                Debug.LogError("Not Found, is player no collider?");
            }
        }

        private System.Collections.IEnumerator DelayRelease()
        {
            yield return new WaitForSeconds(m_ReleaseDelay);
            Recycle();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1, 0, 0, 0.25f);
            Gizmos.DrawCube(Vector3.zero, m_ChaseRange);
        }
    }
}
