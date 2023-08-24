using UnityEngine;
using GrazerCore.GameElements;
using GrazerCore.Skills.Datas;
using SkateHero.GameElements;

namespace SkateHero.Skills.Datas
{
    [CreateAssetMenu(fileName = "SkillDataGrazeCounter", menuName = "SkateGuy/SkillDatas/GrazeCounter")]
    public class SkillDataGrazeCounter : SkillData<BasicPlayer>
    {
        [SerializeField]
        private Launcher m_FireLauncher = null;
        [SerializeField]
        private float m_Duration = 3f;
        [SerializeField]
        private int m_FireNeedGrazeTime = 2;

        protected override void SkillInitialization()
        {
            m_Skill = new Skills.SkillGrazeCounter(CasterData, m_FireLauncher, m_Duration, m_FireNeedGrazeTime);
        }

        public override bool TryCastSkill()
        {
            if (!UseConditionCheck())
            {
                return false;
            } else if (m_Skill.IsCasting)
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
