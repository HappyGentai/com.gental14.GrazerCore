using UnityEngine;
using GrazerCore.GameElements;
using GrazerCore.Skills.Datas;
using SkateHero.GameElements;

namespace SkateHero.Skills.Datas
{
    [CreateAssetMenu(fileName = "ExSkillDataHyperMode", menuName = "SkateGuy/ExSkillDatas/HyperMode")]
    public class ExSkillDataHyperMode : SkillData<BasicPlayer>
    {
        [SerializeField]
        private float m_ExGuageCost = 0;
        [SerializeField]
        private float m_SkillDuraction = 0;
        [SerializeField]
        private float m_HealValue = 0;
        [SerializeField]
        private LayerMask m_BulletTarget = 0;
        [SerializeField]
        private string m_Belong = "";
        [SerializeField]
        private Vector2 m_CancelRange = Vector2.zero;
        [SerializeField]
        private Launcher m_PowerUpLauncher = null;

        protected override void SkillInitialization()
        {
            m_Skill = new Skills.ExSkillHyperMode(CasterData, m_SkillDuraction, m_HealValue, m_BulletTarget,
                m_Belong, m_CancelRange, m_PowerUpLauncher);
        }

        public override bool TryCastSkill()
        {
            if (!UseConditionCheck())
            {
                return false;
            }
            else if (m_Skill.IsCasting)
            {
                return false;
            }
            m_Skill.SkillAwake();
            CasterData.GrazeCounter -= GrazeEnergyCost;
            CasterData.ExGuage -= m_ExGuageCost;
            return true;
        }

        protected override bool UseConditionCheck()
        {
            if (CasterData.GrazeCounter >= GrazeEnergyCost &&
                CasterData.ExGuage >= m_ExGuageCost)
            {
                return true;
            }
            return false;
        }
    }
}
