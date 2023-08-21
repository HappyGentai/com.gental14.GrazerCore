namespace GrazerCore.GameElements.States
{
    public abstract class BasicState
    {
        public BasicState nextState = null;

        protected StateController stateController = null;

        public BasicState(StateController  _stateController)
        {
            stateController = _stateController;
        }

        public abstract void OnEnter();

        public abstract void OnExit();

        public abstract void Track();

        public virtual void SetToNextState()
        {
            stateController.SetState(nextState);
        }
    }
}
