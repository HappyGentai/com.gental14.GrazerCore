using System.Collections;
using UnityEngine;
using GrazerCore.GameElements;
using GrazerCore.GameElements.States;

namespace SkateHero.GameElements.States.EnemyStates
{
    public class BossStateNormalAttack : BasicState
    {
        private Enemy enemy = null;
        private Launcher[] launchers = null;
        private float delayTime = 0;
        private float fireTotalTime = 0;
        private Coroutine attactroutine = null;

        public BossStateNormalAttack(StateController _stateController, Enemy _enemy, Launcher[] _launchers, float _delayTime, float _fireTime): base(_stateController)
        {
            stateController = _stateController;
            enemy = _enemy;
            launchers = _launchers;
            delayTime = _delayTime;
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
            attactroutine = enemy.StartCoroutine(Attacking());
        }

        public override void OnExit()
        {
            var launcherCount = launchers.Length;
            for (int index = 0; index < launcherCount; ++index)
            {
                var launcher = launchers[index];
                launcher.StopLauncher();
            }
            enemy.StopCoroutine(attactroutine);
        }

        public override void Track()
        {

        }

        private IEnumerator Attacking()
        {
            yield return new WaitForSeconds(delayTime);

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
