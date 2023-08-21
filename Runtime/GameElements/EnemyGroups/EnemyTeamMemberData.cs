using UnityEngine;

namespace GrazerCore.GameElements.EnemyGroup
{
    [System.Serializable]
    public class EnemyTeamMemberData
    {
        [SerializeField]
        private Enemy m_EnemyPrefab = null;
        public Enemy EnemyPrefab
        {
            get { return m_EnemyPrefab ; }
        }
        [SerializeField]
        private Vector2 m_SetPosition = Vector2.zero;
        public Vector2 SetPosition
        {
            get { return m_SetPosition; }
        }
        [SerializeField]
        private float m_DelaySpawnTime = 0;
        public float DelaySpawnTime
        {
            get { return m_DelaySpawnTime; }
        }
        [SerializeField]
        private string m_LogicData = "";
        public string LogicData
        {
            get { return m_LogicData; }
        }

        public EnemyTeamMemberData(Enemy enemyPrefab, Vector2 setPos, float delayTime, string logicData)
        {
            m_EnemyPrefab = enemyPrefab;
            m_SetPosition = setPos;
            m_DelaySpawnTime = delayTime;
            m_LogicData = logicData;
        }
    }
}
