using GrazerCore.GameElements;
using GrazerCore.GameElements.States;

namespace SkateHero.GameElements.States.EnemyStates
{
    public class EnemyStateSelfDestruction : BasicState
    {
        private Enemy enemy = null;

        public EnemyStateSelfDestruction(StateController _stateController, Enemy _enemy) : base(_stateController)
        {
            enemy = _enemy;
        }

        public override void OnEnter()
        {
            enemy.Recycle();
        }

        public override void OnExit()
        {

        }

        public override void Track()
        {
            
        }
    }
}