using UnityEngine.Events;

namespace GrazerCore.Skills
{
    public abstract class Skill
    {
        protected UnityEvent<bool> onIsCastingChange = new UnityEvent<bool>();
        public UnityEvent<bool> OnIsCastingChange
        {
            get { return onIsCastingChange; }
            protected set { }
        }

        private bool isCasting = false;
        public virtual bool IsCasting
        {
            get { return isCasting; }
            protected set
            {
                isCasting = value;
                OnIsCastingChange.Invoke(isCasting);
            }
        }

        public abstract void SkillAwake();

        public abstract void SkillEffect();

        public abstract void SkillDone();
    }
}
