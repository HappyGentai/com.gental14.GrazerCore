using System.Collections;
using UnityEngine;

namespace SkateHero.GameElements
{
    public class StageBackGround : MonoBehaviour
    {
        [SerializeField]
        private BackGroundUnit[] m_BackGroundUnits = null;
        [SerializeField]
        private bool m_PlayOnStart = false;
        private Coroutine backgroundRoutine = null;

        void Start()
        {
            if (m_PlayOnStart)
            {
                MoveBackGround();
            }
        }

        public void MoveBackGround()
        {
            StopMoveBackGround();
            backgroundRoutine = StartCoroutine(BackGroundMoving());
        }

        public void StopMoveBackGround()
        {
            if (backgroundRoutine != null)
            {
                StopCoroutine(backgroundRoutine);
            }
        }

        public void ReSetBackGround()
        {
            var unitCount = m_BackGroundUnits.Length;
            for (int index = 0; index < unitCount; ++index)
            {
                var unit = m_BackGroundUnits[index];
                unit.SetToZero();
            }
        }

        private IEnumerator BackGroundMoving()
        {
            while(true)
            {
                yield return null;
                var unitCount = m_BackGroundUnits.Length;
                for (int index = 0; index < unitCount; ++index)
                {
                    var unit = m_BackGroundUnits[index];
                    unit.MoveBackGrounmd();
                }
            }
        }
    }

    [System.Serializable]
    public class BackGroundUnit
    {
        [SerializeField]
        private MeshRenderer m_BackGroundRender = null;
        [SerializeField]
        private Vector2 m_FlowValue = Vector2.zero;
        private Vector2 currentOffset = Vector2.zero;
        
        public void MoveBackGrounmd()
        {
            currentOffset += Time.deltaTime * m_FlowValue;
            SetOffset(currentOffset);
        }

        public void SetBackGroundPosition(Vector2 setPos)
        {
            currentOffset = setPos;
            SetOffset(currentOffset);
        }

        public void SetToZero()
        {
            currentOffset = Vector2.zero;
            SetOffset(currentOffset);
        }

        private void SetOffset(Vector2 value)
        {
            m_BackGroundRender.material.SetTextureOffset("_MainTex", value);
        }
    }
}
