using UnityEngine;

namespace SkateHero.GameElements.EnemyLogicData
{
    [System.Serializable]
    public class EnemyTypeSinMoveLogicData
    {
        [SerializeField]
        private double m_MoveDirectionX = 0;
        [SerializeField]
        private double m_MoveDirectionY = 0;
        public Vector2 MoveDirection
        {
            get { return new Vector2((float)m_MoveDirectionX, (float)m_MoveDirectionX); }
            private set { }
        }

        [SerializeField]
        private double m_SinHalfHeigh = 0;
        public float SinHalfHeigh
        {
            get { return (float)m_SinHalfHeigh; }
            private set { }
        }

        [SerializeField]
        private bool m_AttackWhenMove = false;
        public bool AttackWhenMove
        {
            get { return m_AttackWhenMove; }
            private set { }
        }

        public EnemyTypeSinMoveLogicData(Vector2 _MoveDirection, float _SinHalfHeigh,
            bool _AttackWhenMove)
        {
            m_MoveDirectionX = _MoveDirection.x;
            m_MoveDirectionY = _MoveDirection.y;
            m_SinHalfHeigh = _SinHalfHeigh;
            m_AttackWhenMove = _AttackWhenMove;
        }
    }
}