using UnityEngine;

namespace GrazerCore.GameElements.EnemyGroup
{
    [System.Serializable]
    public class EnemyTeamData
    {
        [SerializeField]
        private EnemyTeam m_EnemyTeam = null;
        public EnemyTeam EnemyTeam
        {
            get { return m_EnemyTeam; }
        }

        [SerializeField]
        private float m_WaveWaitTime = 0;
        public float WaveWaitTime
        {
            get { return m_WaveWaitTime; }
        }
    }
}
