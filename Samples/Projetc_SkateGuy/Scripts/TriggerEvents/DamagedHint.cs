using System.Collections;
using UnityEngine;
using GrazerCore.GameElements;

namespace SkateHero.TriggerEvents
{
    public class DamagedHint : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer m_BlackEffect = null;
        [SerializeField]
        private string m_SetSortingLayerName;
        [SerializeField]
        private float m_StopTime = 1f;
        private Coroutine ShowingRoutine = null;

        private void Start()
        {
            m_BlackEffect.enabled = false;
        }

        public void StartDamagedHint(PlayableObject player, GameObject collisionObject)
        {
            if (player.HP <= 0)
            {
                return;
            }

            var bullet = collisionObject.GetComponent<Bullet>();
            if (bullet != null)
            {
                //  if are players then return
                if (bullet.m_BulletBelong == player.tag)
                {
                    return;
                }
                ShowDamagedHint(collisionObject);
                return;
            }

            var enemy = collisionObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                ShowDamagedHint(collisionObject);
                return;
            }
        }

        private void ShowDamagedHint(GameObject target)
        {
            if (ShowingRoutine != null)
            {
                return;
            }
            var spriteRender = target.GetComponentInChildren<SpriteRenderer>();
            ShowingRoutine = StartCoroutine(DamagedHintShowing(spriteRender));
        }

        private void StopDamagedHint()
        {
            if (ShowingRoutine != null)
            {
                StopCoroutine(ShowingRoutine);
            }
        }

        private IEnumerator DamagedHintShowing(SpriteRenderer spriteRenderer)
        {
            var originalSortingLayer = spriteRenderer.sortingLayerName;
            var originalSortingOrder = spriteRenderer.sortingOrder;

            m_BlackEffect.enabled = true;
            spriteRenderer.sortingLayerName = m_SetSortingLayerName;
            spriteRenderer.sortingOrder = m_BlackEffect.sortingOrder + 1;

            yield return new WaitForSecondsRealtime(m_StopTime);
            spriteRenderer.sortingLayerName = originalSortingLayer;
            spriteRenderer.sortingOrder = originalSortingOrder;
            m_BlackEffect.enabled = false;
            ShowingRoutine = null;
        }
    }
}
