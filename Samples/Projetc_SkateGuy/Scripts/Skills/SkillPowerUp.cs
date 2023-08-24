using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GrazerCore.GameElements;
using GrazerCore.Skills;

namespace SkateHero.Skills
{
    public class SkillPowerUp : Skill
    {
        private PlayableObject player = null;
        private List<Launcher> tempLaunchers = new List<Launcher>();
        private Launcher powerUpLauncher = null;
        private float skilDuration = 0;
        private Coroutine timeCoroutine = null;

        public SkillPowerUp(PlayableObject _player, Launcher _launcher, float _SkillDuration)
        {
            player = _player;
            skilDuration = _SkillDuration;
            if (powerUpLauncher == null)
            {
                powerUpLauncher = GameObject.Instantiate<Launcher>(_launcher);
            }
        }

        public override void SkillAwake()
        {
            if (IsCasting)
            {
                return;
            }
            else { IsCasting = !IsCasting; }

            player.OnPlayerSleep.AddListener(CancelSkill);
            // Save player launcher
            var playerLaunchers = player.Launchers;
            var playerLauncherCount = playerLaunchers.Length;
            for (int index = 0; index < playerLauncherCount; ++index)
            {
                tempLaunchers.Add(playerLaunchers[index]);
            }
            player.Launchers = new Launcher[1] { powerUpLauncher };
            powerUpLauncher.gameObject.SetActive(true);
            powerUpLauncher.transform.SetParent(player.transform);
            powerUpLauncher.transform.localPosition = Vector3.zero;
            powerUpLauncher.AwakeLauncher();
            //  Set timer
            timeCoroutine = player.StartCoroutine(SkillCasting());
        }

        public override void SkillDone()
        {
            player.OnPlayerSleep.RemoveListener(CancelSkill);
            player.Launchers = tempLaunchers.ToArray();
            IsCasting = false;
            powerUpLauncher.StopLauncher();
            powerUpLauncher.gameObject.SetActive(false);
            powerUpLauncher.transform.SetParent(null);
        }

        public override void SkillEffect()
        {

        }

        IEnumerator SkillCasting()
        {
            yield return new WaitForSeconds(skilDuration);
            SkillDone();
        }

        private void CancelSkill()
        {
            player.StopCoroutine(timeCoroutine);
            SkillDone();
        }
    }
}
