using UnityEngine;
using GrazerCore.GameFlow.Events;
using UnityEngine.Events;

namespace GrazerCore.GameFlow
{
    [System.Serializable]
    public class GameEventController
    {
        [SerializeField]
        private GameEvent[] m_GameEvents = null;
        public GameEvent[] GameEvents
        {
            get { return m_GameEvents; }
            set
            {
                m_GameEvents = value;
            }
        }

        private GameEvent currentGameEvent = null;
        private int currentEventIndex = 0;

        protected UnityEvent<GameventResult> _OnEventDone =
            new UnityEvent<GameventResult>();
        public UnityEvent<GameventResult> OnEventDone
        {
            get { return _OnEventDone; }
        }

        protected UnityEvent _OnEventAllDone = new UnityEvent();
        public UnityEvent OnEventAllDone
        {
            get { return _OnEventAllDone; }
        }

        private bool isInitialize = false;
        public bool IsInitialize
        {
            get { return isInitialize; }
        }

        public void Initialize()
        {
            if (isInitialize)
            {
                return;
            }

            var eventCount = m_GameEvents.Length;
            for (int index = 0; index < eventCount; ++index)
            {
                var gameEvent = m_GameEvents[index];
                gameEvent.OnEventDone.AddListener(OnGameEventDone);
            }

            isInitialize = true;
        }

        public void Reset()
        {
            currentEventIndex = 0;
            currentGameEvent = null;
            var eventCount = m_GameEvents.Length;
            for (int index = 0; index < eventCount; ++index)
            {
                var gameEvent = m_GameEvents[index];
                gameEvent.Reset();
            }
        }

        public void StartFlow()
        {
            var eventCount = m_GameEvents.Length;
            if (currentEventIndex >= eventCount)
            {
                Debug.LogError("EventIndex out of range.");
                return;
            }

            currentGameEvent = m_GameEvents[currentEventIndex];
            currentGameEvent.Launch();
            currentEventIndex++;
        }

        public void StopGameEvent()
        {
            if (currentGameEvent != null)
            {
                currentGameEvent.Stop();
            }
        }

        public void ContinueGameEvent()
        {
            if (currentGameEvent != null)
            {
                currentGameEvent.Continue();
            }
        }

        public void CloseGameEvent()
        {
            if (currentGameEvent != null)
            {
                currentGameEvent.Close();
            }
        }

        private void OnGameEventDone(GameventResult result)
        {
            OnEventDone?.Invoke(result);

            //  Check all game event done yet.
            var eventCount = m_GameEvents.Length;
            if (currentEventIndex == eventCount)
            {
                OnEventAllDone?.Invoke();
            } else
            {
                currentGameEvent = m_GameEvents[currentEventIndex];
                currentGameEvent.Launch();
                currentEventIndex++;
            }
        }
    }
}
