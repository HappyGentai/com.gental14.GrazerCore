namespace GrazerCore.GameElements.States
{
    public class StateController
    {
        private BasicState currentState = null;

        public void SetState(BasicState newState)
        {
            if (currentState != null)
            {
                currentState.OnExit();
            }

            currentState = newState;
            if (currentState != null)
            {
                currentState.OnEnter();
            }
        }

        public void Track()
        {
            if (currentState != null)
            {
                currentState.Track();
            }
        }
    }
}
