namespace GrazerCore.GameFlow.States
{
    public abstract class GameState
    {
        public GameState NextState = null;

        /// <summary>
        /// Work when enter this state.
        /// </summary>
        public abstract void OnEnter();

        /// <summary>
        /// Work when set to current game state, call by class-GameController.
        /// </summary>
        /// <param name="dt"></param>
        public abstract void Track(float dt);

        /// <summary>
        /// Work when exit from this state.
        /// </summary>
        public abstract void OnExit();

        public void GoToNextState()
        {
            GameController.ChangeState(NextState);
        }
    }
}
