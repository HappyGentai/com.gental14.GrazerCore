using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using SkateHero.GameElements;

namespace SkateHero.UIs
{
    public class GamePlayUI : BasicUI
    {
        private BasicPlayer _Player = null;
        [SerializeField]
        private Image m_PlayerHP = null;
        [SerializeField]
        private Image m_GrazeCounter = null;
        [SerializeField]
        private Image m_ExGuage = null;
        [SerializeField]
        private Image m_SkillImageA = null;
        [SerializeField]
        private Image m_SkillImageB = null;
        [SerializeField]
        private Image m_SkillImageC = null;
        [SerializeField]
        private Color m_SkillWhenCanUse = Color.white;
        [SerializeField]
        private Color m_SkillWhenCantUse = Color.gray;
        [SerializeField]
        private Color m_SkillWhenUsing = Color.red;

        [SerializeField]
        private UnityEvent OnSkillChargeDone = new UnityEvent();

        public void SetPlayer(BasicPlayer player)
        {
            _Player = player;
        }

        protected override void DoInitialize()
        {
            var maxHP = _Player.MaxHP;
            m_PlayerHP.fillAmount = _Player.HP / maxHP;
            _Player.OnHPChange.AddListener((float currentHp) => {
                m_PlayerHP.fillAmount = currentHp / maxHP;
            });
            var maxExGuage = _Player.MaxExGauge;
            m_ExGuage.fillAmount = _Player.ExGuage / maxExGuage;
            _Player.OnExGaugeChange.AddListener((float currentExGuage) => {
                m_ExGuage.fillAmount = currentExGuage / maxExGuage;
            });
            var skillTriggerA = _Player.SkillTriggers[0];
            var skillDataA = skillTriggerA.SkillData;
            var skillUsingA = false;
            skillDataA.AddOnSkillCastingChangeEvent((bool _skillUsing) =>
            {
                skillUsingA = _skillUsing;
            });
            var skillTriggerB = _Player.SkillTriggers[1];
            var skillDataB = skillTriggerB.SkillData;
            var skillUsingB = false;
            skillDataB.AddOnSkillCastingChangeEvent((bool _skillUsing) =>
            {
                skillUsingB = _skillUsing;
            });
            var skillTriggerC = _Player.SkillTriggers[2];
            var skillDataC = skillTriggerC.SkillData;
            var skillUsingC = false;
            skillDataC.AddOnSkillCastingChangeEvent((bool _skillUsing) =>
            {
                skillUsingC = _skillUsing;
            });
            var maxGrazeCounter = _Player.MaxGrazeCounter;
            m_GrazeCounter.fillAmount = _Player.GrazeCounter / _Player.MaxGrazeCounter;
            _Player.OnGrazeCounterChange.AddListener((float currentGrazeCounter) => {
                m_GrazeCounter.fillAmount = currentGrazeCounter / maxGrazeCounter;
                if (skillUsingA)
                {
                    m_SkillImageA.color = m_SkillWhenUsing;
                }
                else if (_Player.GrazeCounter >= skillDataA.GrazeEnergyCost)
                {
                    if (m_SkillImageA.color != m_SkillWhenCanUse)
                    {
                        OnSkillChargeDone?.Invoke();
                    }
                    m_SkillImageA.color = m_SkillWhenCanUse;
                }
                else
                {
                    m_SkillImageA.color = m_SkillWhenCantUse;
                }

                if (skillUsingB)
                {
                    m_SkillImageB.color = m_SkillWhenUsing;
                }
                else if (_Player.GrazeCounter >= skillDataB.GrazeEnergyCost)
                {
                    if (m_SkillImageB.color != m_SkillWhenCanUse)
                    {
                        OnSkillChargeDone?.Invoke();
                    }
                    m_SkillImageB.color = m_SkillWhenCanUse;
                }
                else
                {
                    m_SkillImageB.color = m_SkillWhenCantUse;
                }

                if (skillUsingC)
                {
                    m_SkillImageC.color = m_SkillWhenUsing;
                }
                else if (_Player.GrazeCounter >= skillDataC.GrazeEnergyCost)
                {
                    if (m_SkillImageC.color != m_SkillWhenCanUse)
                    {
                        OnSkillChargeDone?.Invoke();
                    }
                    m_SkillImageC.color = m_SkillWhenCanUse;
                }
                else
                {
                    m_SkillImageC.color = m_SkillWhenCantUse;
                }
            });
        }
    }
}
