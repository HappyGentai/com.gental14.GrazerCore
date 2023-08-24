using UnityEngine;
using GrazerCore.GameElements;
using GrazerCore.GameElements.States;

namespace SkateHero.GameElements.States.EnemyStates
{
    public class EnemyStateSinMove : BasicState
    {
        private Enemy enemy = null;
        private Transform moveTarget = null;
        private Launcher[] launchers = null;
        private float moveSpeed = 0;
        private Vector2 moveDir = Vector2.zero;
        private float sinHalfHeigh = 0;
        private bool attackWhenMove = false;
        private Vector2 straightMoveValue = Vector2.zero;
        private Vector2 perpendicularVector = Vector2.zero;

        public EnemyStateSinMove(StateController _stateController, Enemy _enemy, Vector2 _moveDir, float _sinHalfHeigh, bool _attackWhenMove) : base(_stateController)
        {
            stateController = _stateController;
            enemy = _enemy;
            launchers = enemy.Launchers;
            moveTarget = enemy.MoveTarget;
            moveSpeed = enemy.MoveSpeed;
            moveDir = _moveDir;
            sinHalfHeigh = _sinHalfHeigh;
            attackWhenMove = _attackWhenMove;

            straightMoveValue = moveTarget.localPosition;
            perpendicularVector = Vector2.Perpendicular(moveDir).normalized;
        }

        public override void OnEnter()
        {
            if (attackWhenMove)
            {
                var launcherCount = launchers.Length;
                for (int index = 0; index < launcherCount; ++index)
                {
                    var launcher = launchers[index];
                    launcher.AwakeLauncher();
                }
            }
        }

        public override void OnExit()
        {
            if (attackWhenMove)
            {
                var launcherCount = launchers.Length;
                for (int index = 0; index < launcherCount; ++index)
                {
                    var launcher = launchers[index];
                    launcher.StopLauncher();
                }
            }
        }

        public override void Track()
        {
            var dt = Time.deltaTime;
            straightMoveValue += (moveDir * moveSpeed * dt);
            var sinValue = Mathf.Sin(Time.time);
            var sinMoveValue = (Vector3)perpendicularVector * (sinValue * sinHalfHeigh);
            moveTarget.localPosition = (Vector3)straightMoveValue + sinMoveValue;

            if (attackWhenMove)
            {
                var launcherCount = launchers.Length;
                for (int index = 0; index < launcherCount; ++index)
                {
                    var launcher = launchers[index];
                    launcher.HoldTrigger();
                }
            }
        }
    }
}
