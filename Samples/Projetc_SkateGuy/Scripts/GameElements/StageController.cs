using UnityEngine;
using GrazerCore.GameFlow.Events;
using GrazerCore.Factories;
using SkateHero.GameElements;

namespace SkateHero.GameElements {
    public class StageController : MonoBehaviour
    {
        [SerializeField]
        private AudioClip m_StageBGM = null;
        public AudioClip StageBGM
        {
            get { return m_StageBGM; }
        }
        [SerializeField]
        private StageBackGround m_StageBackGround = null;
        [SerializeField]
        private GameEvent[] m_GameEvents = null;
        public GameEvent[] GameEvents
        {
            get { return m_GameEvents; }
        }

        public void StageStart()
        {
            this.gameObject.SetActive(true);
            m_StageBackGround.MoveBackGround();
        }

        public void StageClose()
        {
            m_StageBackGround.StopMoveBackGround();
            ClearOldGameElement();
            this.gameObject.SetActive(false);
        }

        public void ResetStage()
        {
            m_StageBackGround.ReSetBackGround();
            ClearOldGameElement();
        }

        private void ClearOldGameElement()
        {
            //  Recycle all old bullet 
            BulletFactory.ReleaseAll();
            //  Recycle all old enemy
            EnemyFactory.ReleaseAll();
            //  Recycle all old team
            EnemyTeamFactory.ReleaseAll();
            //  Recycle all old pick up object
            PickUpObjectFactory.ReleaseAll();
            //  Recycle all old effect
            EffectFactory.ReleaseAll();
        }
    }
}
