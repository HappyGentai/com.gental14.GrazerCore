using System.Collections;
using UnityEngine;
using GrazerCore.GameElements;
using GrazerCore.Tool;
using GrazerCore.Skills;

namespace SkateHero.Skills
{
    public class SkillGrazeCounter : Skill
    {
        private PlayableObject player = null;
        private float skilDuration = 0;
        private int fireCost = 0;
        private int fireCounter = 0;
        private Launcher LauncherPrefab = null;
        private Launcher Launcher = null;
        private Coroutine timeCoroutine = null;

        public SkillGrazeCounter(PlayableObject _player, Launcher _Launcher, float _SkillDuration, int _fireCost)
        {
            player = _player;
            skilDuration = _SkillDuration;
            fireCost = _fireCost;
            LauncherPrefab = _Launcher;
            if (Launcher == null)
            {
                Launcher = GameObject.Instantiate<Launcher>(LauncherPrefab);
            }
        }

        public override void SkillAwake()
        {
            if (IsCasting)
            {
                return;
            }
            else { IsCasting = !IsCasting; }
            player.OnGraze.AddListener(OnGraze);
            timeCoroutine = CoroutineAgent.StartEntrustCoroutine(SkillCasting());
            player.OnPlayerSleep.AddListener(StopSkillFromOutSide);
            Launcher.gameObject.SetActive(true);
            Launcher.AwakeLauncher();
            Launcher.transform.SetParent(player.transform);
            Launcher.transform.localPosition = Vector3.zero;
        }

        public override void SkillDone()
        {
            player.OnGraze.RemoveListener(OnGraze);
            player.OnPlayerSleep.RemoveListener(StopSkillFromOutSide);
            IsCasting = false;
            Launcher.StopLauncher();
            Launcher.gameObject.SetActive(false);
            Launcher.transform.SetParent(null);
        }

        public override void SkillEffect()
        {

        }

        private void OnGraze(Collider2D[] grazeTargets)
        {
            if (grazeTargets.Length != 0)
            {
                fireCounter++;
            }
            
            if (fireCounter >= fireCost)
            {
                //  Shoot bullet
                Launcher.HoldTrigger();
                fireCounter = 0;
            }
        }

        private void StopSkillFromOutSide()
        {
            CoroutineAgent.StopEntrustCoroutine(timeCoroutine);
            SkillDone();
        }

        IEnumerator SkillCasting()
        {
            yield return new WaitForSeconds(skilDuration);
            SkillDone();
        }
    }
}
