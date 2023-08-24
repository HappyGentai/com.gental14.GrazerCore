using UnityEngine;
using GrazerCore.GameFlow;

namespace SkateHero.GameFlow
{
    public class SimpleGameFlower : MonoBehaviour
    {
        [SerializeField]
        private GameEventController m_GameEventController = null;

        private void Start()
        {
            m_GameEventController.Initialize();
            m_GameEventController.OnEventAllDone.AddListener(EventAllDone);
        }

        private void EventAllDone()
        {
            Debug.Log("All event done");
        }

        private void OnGUI()
        {
            if (GUILayout.Button("StartFlow") && m_GameEventController.IsInitialize)
            {
                m_GameEventController.StartFlow();
            }
            if (GUILayout.Button("CloseGameEvent") & m_GameEventController.IsInitialize)
            {
                m_GameEventController.CloseGameEvent();
            }
            if (GUILayout.Button("ResetGameEvent") & m_GameEventController.IsInitialize)
            {
                m_GameEventController.Reset();
            }
        }
    }
}
