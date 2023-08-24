using UnityEngine;
using GrazerCore.GameElements;
using GrazerCore.GameElements.States;

namespace SkateHero.GameElements.States.EnemyStates
{
    public class EnemyStateAdiotMove: BasicState
    {
        private Enemy enemy = null;
        private Transform enemyMoveTarget = null;
        private Vector2 moveDir = Vector2.zero;
        private Launcher[] launchers = null;

        public EnemyStateAdiotMove(StateController _stateController, Enemy _enemy, Vector2 _moveDir) : base(_stateController)
        {
            stateController = _stateController;
            enemy = _enemy;
            enemyMoveTarget = enemy.MoveTarget;
            moveDir = _moveDir;
            launchers = enemy.Launchers;
            //  Awake all launcher
            var launcherCount = launchers.Length;
            for (int index = 0; index < launcherCount; ++index)
            {
                var launcher = launchers[index];
                launcher.AwakeLauncher();
            }
        }

        public override void OnEnter()
        {
            
        }

        public override void OnExit()
        {
            var launcherCount = launchers.Length;
            for (int index = 0; index < launcherCount; ++index)
            {
                var launcher = launchers[index];
                launcher.StopLauncher();
            }
        }

        public override void Track()
        {
            var enemySpeed = enemy.MoveSpeed;
            enemyMoveTarget.localPosition += (Vector3)(Time.deltaTime * moveDir * enemySpeed);
            Fire();
        }

        private void Fire()
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