using System.Collections;
using UnityEngine;
using GrazerCore.GameElements;
using GrazerCore.Tool;
using GrazerCore.Skills;
using SkateHero.GameElements;

namespace SkateHero.Skills
{
    /// <summary>
    /// When cast will cancel enemy bullet in range and heal player,
    /// then upgrade player power a time.
    /// </summary>
    public class ExSkillHyperMode : Skill
    {
        private BasicPlayer _player = null;
        private float _duraction = 0;
        private float _healValue = 0;
        private LayerMask _bulletTarget = 0;
        private Vector2 _cancelRange = Vector2.zero;
        private string _belong = "";
        private Launcher _launcherPrefab = null;

        private Launcher _launcher = null;
        private Coroutine timeCoroutine = null;

        public ExSkillHyperMode(BasicPlayer player, float duraction,
            float healValue, LayerMask bulletTarget, string belong, Vector2 cancelRange
            , Launcher launcher)
        {
            _player = player;
            _duraction = duraction;
            _healValue = healValue;
            _bulletTarget = bulletTarget;
            _belong = belong;
            _cancelRange = cancelRange;
            _launcherPrefab = launcher;
            if (_launcher == null)
            {
                _launcher = GameObject.Instantiate<Launcher>(_launcherPrefab);
                _launcher.gameObject.SetActive(false);
            }
        }

        public override void SkillAwake()
        {
            if (IsCasting)
            {
                return;
            }
            else { IsCasting = !IsCasting; }

            //  Heal
            _player.HP += _healValue;
            //  Cancel bullet
            CancelBullet();
            //  Set launcher and start work
            _launcher.gameObject.SetActive(true);
            _launcher.transform.SetParent(_player.transform);
            _launcher.transform.localPosition = Vector3.zero;
            _launcher.AwakeLauncher();
            _launcher.StartTrigger();
            //  Set time counter
            timeCoroutine = CoroutineAgent.StartEntrustCoroutine(SkillCasting());
            _player.OnPlayerSleep.AddListener(StopSkillFromOutSide);
        }

        public override void SkillDone()
        {
            _player.OnPlayerSleep.RemoveListener(StopSkillFromOutSide);
            IsCasting = false;
            _launcher.StopLauncher();
            _launcher.gameObject.SetActive(false);
            _launcher.transform.SetParent(null);
        }

        public override void SkillEffect()
        {

        }

        private void CancelBullet()
        {
            var finds = Physics2D.OverlapBoxAll(Vector2.zero, _cancelRange, 0, _bulletTarget);
            var findCount = finds.Length;
            for (int index = 0; index < findCount; ++index)
            {
                var bullet = finds[index].GetComponent<Bullet>();
                if (bullet != null)
                {
                    if (bullet.m_BulletBelong != _belong)
                    {
                        bullet.Recycle();
                    }
                }
            }
        }

        private void StopSkillFromOutSide()
        {
            CoroutineAgent.StopEntrustCoroutine(timeCoroutine);
            SkillDone();
        }

        private IEnumerator SkillCasting()
        {
            var skillCOunter = _duraction;
            while(skillCOunter > 0)
            {
                yield return null;
                skillCOunter -= Time.deltaTime;
                _launcher.HoldTrigger();
            }

            SkillDone();
        }
    }
}
