namespace GrazerCore.GameElements
{
    /// <summary>
    /// Will trigger when Class-PlayableObject damaged.
    /// Need set function to Class-PlayableObject unity event-OnGetDamage to subscribe
    /// </summary>
    [System.Serializable]
    public abstract class DamageResponse
    {
        protected PlayableObject targetPlayer = null;

        /// <summary>
        /// Call this function when attach to PlayableObject.
        /// </summary>
        public virtual void Install(PlayableObject player)
        {
            targetPlayer = player;
            targetPlayer.OnGetDamage.AddListener(OnDamaged);
        }
        /// <summary>
        /// Call this function when non-attach to PlayableObject.
        /// </summary>
        public virtual void UnInstall()
        {
            targetPlayer.OnGetDamage.RemoveListener(OnDamaged);
        }

        public abstract void OnDamaged(PlayableObject player, float dmg);
    }
}
