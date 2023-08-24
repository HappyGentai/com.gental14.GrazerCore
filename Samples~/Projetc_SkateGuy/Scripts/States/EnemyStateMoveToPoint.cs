using System.Collections;
using UnityEngine;
using GrazerCore.GameElements;
using GrazerCore.GameElements.States;

namespace SkateHero.GameElements.States.EnemyStates
{
    public class EnemyStateMoveToPoint : BasicState
    {
        private Enemy enemy = null;
        private Transform moveTarget = null;
        private float moveSpeed = 0;
        private Vector2 targetPoint = Vector2.zero;
        private Coroutine moveingCoroutine = null;

        public EnemyStateMoveToPoint(StateController _stateController, Enemy _enemy, Vector2 _targetPoin) : base(_stateController)
        {
            stateController = _stateController;
            enemy = _enemy;
            moveTarget = enemy.MoveTarget;
            moveSpeed = enemy.MoveSpeed;
            targetPoint = _targetPoin;
        }

        public override void OnEnter()
        {
            moveingCoroutine = enemy.StartCoroutine(MoveingTo());
        }

        public override void OnExit()
        {
            enemy.StopCoroutine(moveingCoroutine);
        }

        public override void Track()
        {
            
        }

        private IEnumerator MoveingTo()
        {
            var dir = (targetPoint - (Vector2)moveTarget.localPosition).normalized;
            var dirToDis = Vector2.Distance(dir, Vector2.zero); ;
            var totalDistence = Vector2.Distance(targetPoint, (Vector2)moveTarget.localPosition);
            var totalMove = 0f;
            while(totalMove < totalDistence)
            {
                yield return null;
                var dt = Time.deltaTime * moveSpeed;
                moveTarget.localPosition += (Vector3)(dir * dt);
                totalMove += dirToDis * dt;
            }
            moveTarget.localPosition = targetPoint;
            this.SetToNextState();
        }
    }
}
