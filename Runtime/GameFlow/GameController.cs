using UnityEngine;
using GrazerCore.GameFlow.States;

namespace GrazerCore.GameFlow
{
    public class GameController : MonoBehaviour
    {
        protected static GameController _inti = null;

        protected GameState _CurrentState = null;

        private void Awake()
        {
            Initialize();
        }

        protected virtual void Initialize()
        {
            if (_inti == null)
            {
                _inti = this;
            }
            else if (_inti != this)
            {
                Destroy(this.gameObject);
            }
        }

        private void Update()
        {
            if (_CurrentState != null)
            {
                _CurrentState.Track(Time.deltaTime);
            }
        }

        public static void ChangeState(GameState changeState)
        {
            var currentState = _inti._CurrentState;
            if (currentState != null)
            {
                currentState.OnExit();
            }

            if (changeState != null)
            {
                _inti._CurrentState = changeState;
                changeState.OnEnter();
            }
        }
    }
}
