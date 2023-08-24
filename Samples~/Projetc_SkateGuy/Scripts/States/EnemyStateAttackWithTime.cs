using System.Collections;
using UnityEngine;
using GrazerCore.GameElements;
using GrazerCore.GameElements.States;

namespace SkateHero.GameElements.States.EnemyStates
{
    public class EnemyStateAttackWithTime : BasicState
    {
        private Enemy enemy = null;
        private Launcher[] launchers = null;
        private float fireTotalTime = 0;
        private Coroutine fireCoroutine = null;

        public EnemyStateAttackWithTime(StateController _stateController, Enemy _enemy, float _fireTime) : base(_stateController)
        {
            stateController = _stateController;
            enemy = _enemy;
            launchers = enemy.Launchers;
            fireTotalTime = _fireTime;
        }

        public override void OnEnter()
        {
            //  Awake all launcher
            var launcherCount = launchers.Length;
            for (int index = 0; index < launcherCount; ++index)
            {
                var launcher = launchers[index];
                launcher.AwakeLauncher();
            }
            fireCoroutine = enemy.StartCoroutine(Firing());
        }

        public override void OnExit()
        {
            var launcherCount = launchers.Length;
            for (int index = 0; index < launcherCount; ++index)
            {
                var launcher = launchers[index];
                launcher.StopLauncher();
            }
            if (fireCoroutine != null)
            {
                enemy.StopCoroutine(fireCoroutine);
            }
        }

        public override void Track()
        {
            
        }

        private IEnumerator Firing()
        {
            var timecounter = 0f;
            var launcherCount = launchers.Length;
            while (timecounter < fireTotalTime)
            {
                yield return null;
                timecounter += Time.deltaTime;
                for (int index = 0; index < launcherCount; ++index)
                {
                    var launcher = launchers[index];
                    launcher.HoldTrigger();
                }
            }
            this.SetToNextState();
        }
    }
}
