using UnityEngine;
using GrazerCore.GameElements;
using GrazerCore.GameElements.States;

namespace SkateHero.GameElements.States.EnemyStates
{
    public class EnemyStateMove : BasicState
    {
        private Enemy enemy = null;
        private Transform moveTarget = null;
        private float moveSpeed = 0;
        private Vector2 moveDir = Vector2.zero;

        public EnemyStateMove(StateController _stateController, Enemy _enemy, Vector2 _moveDir) : base(_stateController)
        {
            stateController = _stateController;
            enemy = _enemy;
            moveTarget = enemy.MoveTarget;
            moveSpeed = enemy.MoveSpeed;
            moveDir = _moveDir;
        }

        public override void OnEnter()
        {

        }

        public override void OnExit()
        {
  
        }

        public override void Track()
        {
            moveTarget.localPosition += (Vector3)(Time.deltaTime * moveDir * moveSpeed);
        }
    }
}
