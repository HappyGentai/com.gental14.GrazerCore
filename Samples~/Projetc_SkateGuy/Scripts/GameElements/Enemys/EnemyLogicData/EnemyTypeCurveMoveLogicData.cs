using UnityEngine;

namespace SkateHero.GameElements.EnemyLogicData
{
    [System.Serializable]
    public class EnemyTypeCurveMoveLogicData
    {
        [SerializeField]
        private double m_CurveEndPosX = 0;
        [SerializeField]
        private double m_CurveEndPosY = 0;
        public Vector2 CurveEndPos
        {
            get
            {
                return new Vector2((float)m_CurveEndPosX, (float)m_CurveEndPosY);
            }
            private set { }
        }

        [SerializeField]
        private double m_CurveAidPosAX = 0;
        [SerializeField]
        private double m_CurveAidPosAY = 0;
        public Vector2 CurveAidPosA
        {
            get { return new Vector2((float)m_CurveAidPosAX, (float)m_CurveAidPosAY); }
            private set { }
        }

        [SerializeField]
        private double m_CurveAidPosBX = 0;
        [SerializeField]
        private double m_CurveAidPosBY = 0;
        public Vector2 CurveAidPosB
        {
            get { return new Vector2((float)m_CurveAidPosBX, (float)m_CurveAidPosBY); }
            private set { }
        }

        [SerializeField]
        private double m_SpeedScale = 0;
        public  float SpeedScale
        {
            get { return (float)m_SpeedScale; }
            private set { }
        }

        #region Search target and attack data
        [SerializeField]
        private LayerMask m_TargetMask= 0;
        public LayerMask TargetMask
        {
            get { return m_TargetMask; }
            private set { }
        }

        [SerializeField]
        private Vector2 m_SearchDirection = Vector2.zero;
        public Vector2 SearchDirection
        {
            get { return m_SearchDirection; }
            private set { }
        }

        [SerializeField]
        private float m_SearchLength = 2f;
        public float SearchLength
        {
            get { return m_SearchLength; }
            private set { }
        }

        [SerializeField]
        private float m_AttackTime = 1;
        public float AttackTime
        {
            get { return m_AttackTime; }
            private set { }
        }
        #endregion

        public EnemyTypeCurveMoveLogicData(Vector2 _CurveEndPos,
            Vector2 _CurveAidPosA, Vector2 _CurveAidPosB, float _SpeedScale,
            LayerMask targetMask, Vector2 searchDir, float searchLength,
            float  attackTime)
        {
            m_CurveEndPosX = _CurveEndPos.x;
            m_CurveEndPosY = _CurveEndPos.y;
            m_CurveAidPosAX = _CurveAidPosA.x;
            m_CurveAidPosAY = _CurveAidPosA.y;
            m_CurveAidPosBX = _CurveAidPosB.x;
            m_CurveAidPosBY = _CurveAidPosB.y;
            m_SpeedScale = _SpeedScale;
            m_TargetMask = targetMask;
            m_SearchDirection = searchDir;
            m_SearchLength = searchLength;
            m_AttackTime = attackTime;
        }
    }
}
