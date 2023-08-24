using UnityEngine;
using GrazerCore.GameElements;
using GrazerCore.Factories;
using GrazerCore.Skills;

namespace SkateHero.Skills
{
    public class SkillAbsortBullet : Skill
    {
        private PlayableObject player = null;
        private Bullet bullet = null;
        private string belong = "";
        private Vector2 fireAdjustPos = Vector2.zero;

        public SkillAbsortBullet(PlayableObject _player, Bullet _bullet, string _belong, Vector2 _fireAdjustPos)
        {
            player = _player;
            bullet = _bullet;
            belong = _belong;
            fireAdjustPos = _fireAdjustPos;
        }

        public override void SkillAwake()
        {
            var firePos = player.MoveTarget.localPosition + (Vector3)fireAdjustPos;
            var fireBullet = BulletFactory.GetBullet(bullet);
            fireBullet.m_BulletBelong = belong;
            fireBullet.MoveDir = player.MoveTarget.right;
            fireBullet.transform.position = firePos;
            fireBullet.WakeUpBullet();
        }

        public override void SkillDone()
        {

        }

        public override void SkillEffect()
        {

        }
    }
}
