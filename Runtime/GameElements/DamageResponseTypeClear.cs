using UnityEngine;
using GrazerCore.Interfaces;

namespace GrazerCore.GameElements
{
    [System.Serializable]
    public class DamageResponseTypeClear : DamageResponse
    {
        [SerializeField]
        private float m_ClearRadius = 3;
        [SerializeField]
        private LayerMask m_ClearTarget = 0;
#if UNITY_EDITOR
        [Header("DebugPart")]
        [SerializeField]
        private bool m_Debug = false;
        [SerializeField]
        private Color m_LineColo = Color.red;
        [SerializeField]
        private float m_Duration = 1;
#endif
        public override void OnDamaged(PlayableObject player, float dmg)
        {
            var centerPos = player.MoveTarget.localPosition;
            var hits = Physics2D.OverlapCircleAll(centerPos, m_ClearRadius, m_ClearTarget);
            int hitCount = hits.Length;
            for (int index = 0; index < hitCount; ++index)
            {
                var iRecycle = hits[index].GetComponent<IRecycleable>();
                iRecycle?.Recycle();
            }
#if UNITY_EDITOR
            if (m_Debug)
            {
                Debug.DrawLine(centerPos, centerPos + Vector3.up * m_ClearRadius, m_LineColo, m_Duration);
                Debug.DrawLine(centerPos, centerPos + Vector3.down * m_ClearRadius, m_LineColo, m_Duration);
                Debug.DrawLine(centerPos, centerPos + Vector3.right * m_ClearRadius, m_LineColo, m_Duration);
                Debug.DrawLine(centerPos, centerPos + Vector3.left * m_ClearRadius, m_LineColo, m_Duration);
            }
#endif
        }
    }
}
