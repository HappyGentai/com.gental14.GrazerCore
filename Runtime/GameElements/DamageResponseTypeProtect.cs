using System.Collections;
using UnityEngine;

namespace GrazerCore.GameElements
{
    [System.Serializable]
    public class DamageResponseTypeProtect : DamageResponse
    {
        [SerializeField]
        private float m_ProtectDuration = 1.5f;
        private Coroutine protectRoutine = null;

        public override void Install(PlayableObject player)
        {
            base.Install(player);
            targetPlayer.OnPlayerSleep.AddListener(DisableEffect);
        }

        public override void UnInstall()
        {
            base.UnInstall();
            targetPlayer.OnPlayerSleep.RemoveListener(DisableEffect);
        }

        public override void OnDamaged(PlayableObject player, float dmg)
        {
            if (targetPlayer.HP <= 0)
            {
                return;
            }
            targetPlayer.Invincible = true;
            protectRoutine = targetPlayer.StartCoroutine(Protecting());
        }

        private IEnumerator Protecting()
        {
            yield return new WaitForSeconds(m_ProtectDuration);
            targetPlayer.Invincible = false;
        }

        private void DisableEffect()
        {
            if (protectRoutine != null)
            {
                targetPlayer.StopCoroutine(Protecting());
            }
        }
    }
}
