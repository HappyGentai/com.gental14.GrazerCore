using UnityEngine;
using GrazerCore.GameElements;
using GrazerCore.Skills.Datas;
using SkateHero.GameElements;

namespace SkateHero.Skills.Datas
{
    [CreateAssetMenu(fileName = "SkillDataAbsortBullet", menuName = "SkateGuy/SkillDatas/AbsortBullet")]
    public class SkillDataAbsortBullet : SkillData<BasicPlayer>
    {
        [SerializeField]
        private Bullet m_Bullet = null;
        [SerializeField]
        private string m_Belong = "";
        [SerializeField]
        private Vector2 m_FireAdjustPos = Vector2.zero;

        protected override void SkillInitialization()
        {
            m_Skill = new Skills.SkillAbsortBullet(CasterData, m_Bullet, m_Belong, m_FireAdjustPos);
        }

        public override bool TryCastSkill()
        {
            if (!UseConditionCheck())
            {
                return false;
            }
            m_Skill.SkillAwake();
            CasterData.GrazeCounter -= GrazeEnergyCost;
            return true;
        }

        protected override bool UseConditionCheck()
        {
            if (CasterData.GrazeCounter >= GrazeEnergyCost)
            {
                return true;
            }
            return false;
        }
    }
}
