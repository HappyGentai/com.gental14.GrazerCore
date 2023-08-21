using UnityEngine;
using UnityEngine.Events;

namespace GrazerCore.GameFlow.Events
{
    public abstract class GameEvent : ScriptableObject
    {
        protected UnityEvent<GameventResult> _OnEventDone = 
            new UnityEvent<GameventResult>();
        public UnityEvent<GameventResult> OnEventDone
        {
            get { return _OnEventDone; }
        }

        /// <summary>
        /// Launch game event, call F-GameEventDone when event done.
        /// </summary>
        public abstract void Launch();

        /// <summary>
        /// [Optional]  Call game event stop.
        /// </summary>
        public virtual void Stop()
        {

        }

        /// <summary>
        /// [Optional]  Call game event continue.
        /// </summary>
        public virtual void Continue()
        {

        }

        /// <summary>
        /// [Optional]  Call game event close.
        /// </summary>
        public virtual void Close()
        {

        }

        /// <summary>
        /// [Optional]  Reset game event data.
        /// </summary>
        public virtual void Reset()
        {

        }

        protected virtual void GameEventDone()
        {
            OnEventDone?.Invoke(GameventResult.Empty);
        }
    }

    public abstract class GameventResult
    {
        public static GameventResult Empty
        {
            get { return null; }
        }
    }
}
