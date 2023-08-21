using UnityEngine;
using UnityEngine.Events;

namespace GrazerCore.Skills.Datas
{
    public abstract class SkillData<T> : ScriptableObject
    {
        [SerializeField]
        protected float m_GrazeEnergyCost = 0;
        public virtual float GrazeEnergyCost
        {
            get { return m_GrazeEnergyCost; }
            protected set { }
        }
        protected Skill m_Skill = null;
        protected T CasterData = default;

        public virtual void CreateSKillEntity(T casterData)
        {
            CasterData = casterData;
            SkillInitialization();
        }

        public virtual bool TryCastSkill()
        {
            if (!UseConditionCheck())
            {
                return false;
            }
            m_Skill.SkillAwake();
            return true;
        }

        public virtual void AddOnSkillCastingChangeEvent(UnityAction<bool> action)
        {
            m_Skill.OnIsCastingChange.AddListener(action);
        }

        public virtual void RemoveOnSkillCastingChangeEvent(UnityAction<bool> action)
        {
            m_Skill.OnIsCastingChange.RemoveListener(action);
        }

        protected abstract bool UseConditionCheck();

        protected abstract void SkillInitialization();
    }
}
