using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using SkateHero.GameElements;
using UnityEngine.InputSystem;

namespace SkateHero.Test {
    public class TestPlayerUI : MonoBehaviour
    {
        [SerializeField]
        private BasicPlayer m_Player = null;
        [SerializeField]
        private Image m_PlayerHP = null;
        [SerializeField]
        private Image m_GrazeCounter = null;
        [SerializeField]
        private Image m_ExGuage = null;
        [SerializeField]
        private Image m_SkillImage = null;
        [SerializeField]
        private Image m_SkillImage2 = null;
        [SerializeField]
        private Image m_SkillImage3 = null;
        [SerializeField]
        private Color m_SkillWhenCanUse= Color.white;
        [SerializeField]
        private Color m_SkillWhenCantUse = Color.gray;
        [SerializeField]
        private Color m_SkillWhenUsing = Color.red;
        public bool IsInitialization = false;

        [SerializeField]
        private UnityEvent OnSkillChargeDone = new UnityEvent();

        [Header("Game UI")]
        [SerializeField]
        private GameObject m_GameClearUI = null;
        public UnityEvent m_OnGameClear = new UnityEvent();
        [SerializeField]
        private GameObject m_GameOverUI = null;
        public UnityEvent m_OnGameOver = new UnityEvent();
        [SerializeField]
        protected InputActionReference m_ReStartActionRef = null;
        protected InputAction m_ReStartAction;
        public UnityEvent OnReStart = new UnityEvent();

        public void Initialization()
        {
            var maxHP = m_Player.MaxHP;
            m_PlayerHP.fillAmount = m_Player.HP / maxHP; ;
            m_Player.OnHPChange.AddListener((float currentHp) => {
                m_PlayerHP.fillAmount = currentHp / maxHP;
            });
            var maxExGuage = m_Player.MaxExGauge;
            m_ExGuage.fillAmount = m_Player.ExGuage / maxExGuage;
            m_Player.OnExGaugeChange.AddListener((float currentExGuage) => {
                m_ExGuage.fillAmount = currentExGuage / maxExGuage;
            });
            var skillTrigger = m_Player.SkillTriggers[0];
            var skillData = skillTrigger.SkillData;
            var skillUsing = false;
            skillData.AddOnSkillCastingChangeEvent((bool _skillUsing) =>
            {
                skillUsing = _skillUsing;
            });
            var skillTrigger2 = m_Player.SkillTriggers[1];
            var skillData2 = skillTrigger2.SkillData;
            var skillUsing2 = false;
            skillData2.AddOnSkillCastingChangeEvent((bool _skillUsing) =>
            {
                skillUsing2 = _skillUsing;
            });
            var skillTrigger3 = m_Player.SkillTriggers[2];
            var skillData3 = skillTrigger3.SkillData;
            var skillUsing3 = false;
            skillData3.AddOnSkillCastingChangeEvent((bool _skillUsing) =>
            {
                skillUsing3 = _skillUsing;
            });
            var maxGrazeCounter = m_Player.MaxGrazeCounter;
            m_GrazeCounter.fillAmount = m_Player.GrazeCounter / m_Player.MaxGrazeCounter;
            m_Player.OnGrazeCounterChange.AddListener((float currentGrazeCounter) => {
                m_GrazeCounter.fillAmount = currentGrazeCounter / maxGrazeCounter;
                if (skillUsing)
                {
                    m_SkillImage.color = m_SkillWhenUsing;
                }
                else if (m_Player.GrazeCounter >= skillData.GrazeEnergyCost)
                {
                    if (m_SkillImage.color != m_SkillWhenCanUse)
                    {
                        OnSkillChargeDone?.Invoke();
                    }
                    m_SkillImage.color = m_SkillWhenCanUse;
                }
                else
                {
                    m_SkillImage.color = m_SkillWhenCantUse;
                }
                if (skillUsing2)
                {
                    m_SkillImage2.color = m_SkillWhenUsing;
                }
                else if (m_Player.GrazeCounter >= skillData2.GrazeEnergyCost)
                {
                    if (m_SkillImage2.color != m_SkillWhenCanUse)
                    {
                        OnSkillChargeDone?.Invoke();
                    }
                    m_SkillImage2.color = m_SkillWhenCanUse;
                }
                else
                {
                    m_SkillImage2.color = m_SkillWhenCantUse;
                }
                if (skillUsing3)
                {
                    m_SkillImage3.color = m_SkillWhenUsing;
                }
                else if (m_Player.GrazeCounter >= skillData3.GrazeEnergyCost)
                {
                    if (m_SkillImage3.color != m_SkillWhenCanUse)
                    {
                        OnSkillChargeDone?.Invoke();
                    }
                    m_SkillImage3.color = m_SkillWhenCanUse;
                }
                else
                {
                    m_SkillImage3.color = m_SkillWhenCantUse;
                }
            });

            //  Set restart input
            m_ReStartAction = m_ReStartActionRef.action;
            m_ReStartAction.performed += (ctx) => {
                OnReStart?.Invoke();
            };

            IsInitialization = true;
        }

        public void StartUP()
        {
            m_GameClearUI.SetActive(false);
            m_GameOverUI.SetActive(false);
            m_ReStartAction.Disable();
        }

        public void GameClear()
        {
            m_GameClearUI.SetActive(true);
            m_ReStartAction.Enable();
            m_OnGameClear?.Invoke();
        }

        public void GameOver()
        {
            m_GameOverUI.SetActive(true);
            m_ReStartAction.Enable();
            m_OnGameOver?.Invoke();
        }
    }
}
