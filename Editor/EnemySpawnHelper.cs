using UnityEngine;
using GrazerCore.GameElements;
using GrazerCore.Interfaces;

namespace GrazerCore.Editor
{
    public class EnemySpawnHelper : MonoBehaviour
    {
        public Enemy m_TargetObject = null;
        public float m_DelayTime = 0;
        [HideInInspector]
        public string m_LogicData = "";

        public EnemySpawnHelper(Enemy _enemy, float delayTime, string logicData)
        {
            m_TargetObject = _enemy;
            m_DelayTime = delayTime;
            m_LogicData = logicData;
        }

        public string GetLogicData()
        {
            if (m_TargetObject is ILogicDataSetable iDataSetable)
            {
                return iDataSetable.GetLogicData();
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogWarning("TargetObject doesn;t have interface-IDataSetable");
#endif
                return "";
            }
        }

        public void SetLogicData(string rawData)
        {
            if (m_TargetObject is ILogicDataSetable iDataSetable)
            {
                iDataSetable.SetLogicData(rawData);
            }
            else
            {
#if UNITY_EDITOR
                Debug.LogWarning("TargetObject doesn;t have interface-IDataSetable");
#endif
            }
        }
    }
}
