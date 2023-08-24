using UnityEngine;

namespace SkateHero.GameElements.EnemyLogicData
{
    [System.Serializable]
    public class EnemyTypeHitAndRunLogicData
    {
        [SerializeField]
        private double m_MoveTargetPointX = 0;
        [SerializeField]
        private double m_MoveTargetPointY = 0;
        public Vector2 MoveTargetPoint
        {
            get { return new Vector2((float)m_MoveTargetPointX, (float)m_MoveTargetPointY); }
            private set { }
        }
        [SerializeField]
        private double m_FireTime = 0;
        public float FireTime { get { return (float)m_FireTime; } private set { } }
        [SerializeField]
        private double m_FleeDirX = 0;
        [SerializeField]
        private double m_FleeDirY = 0;
        public Vector2 FleeDir
        {
            get { return new Vector2((float)m_FleeDirX, (float)m_FleeDirY); }
            private set { }
        }

        public EnemyTypeHitAndRunLogicData(Vector2 _MoveTargetPoint,
            float _FireTime, Vector2 _FleeDir)
        {
            m_MoveTargetPointX = _MoveTargetPoint.x;
            m_MoveTargetPointY = _MoveTargetPoint.y;
            m_FireTime = _FireTime;
            m_FleeDirX = _FleeDir.x;
            m_FleeDirY = _FleeDir.y;
        }
    }
}
