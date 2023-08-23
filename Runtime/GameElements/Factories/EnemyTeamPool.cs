using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using GrazerCore.GameElements.EnemyGroup;

namespace GrazerCore.Factories
{
    public class EnemyTeamPool
    {
        private EnemyTeamData m_CoreEnemyTeamData = null;
        public EnemyTeamData CoreEnemyTeamData
        {
            get { return m_CoreEnemyTeamData; }
        }

        private ObjectPool<EnemyTeam> enemyTeamPool = null;

        private List<EnemyTeam> aliveObject = new List<EnemyTeam>();

        public EnemyTeamPool(EnemyTeamData _CoreEnemyTeamData)
        {
            m_CoreEnemyTeamData = _CoreEnemyTeamData;
            enemyTeamPool = new ObjectPool<EnemyTeam>(CreatePoolItem, OnTakeFormPool,
                    OnReturnToPool, OnDestroyPoolObject, false);
        }

        public void Dispose()
        {
            enemyTeamPool.Dispose();
        }

        public EnemyTeam GetEnemyTeam()
        {
            var enemyTeam = enemyTeamPool.Get();
            return enemyTeam;
        }

        public void ReleaseAll()
        {
            int aliveEnemyCount = aliveObject.Count;
            for (int index = 0; index < aliveEnemyCount; index++)
            {
                enemyTeamPool.Release(aliveObject[0]);
            }
            aliveObject.Clear();
        }

        #region About pool event
        EnemyTeam CreatePoolItem()
        {
            var newEnemyTeam = GameObject.Instantiate<EnemyTeam>(m_CoreEnemyTeamData.EnemyTeam);
            newEnemyTeam.OnSummonDone.AddListener(() => {
                enemyTeamPool.Release(newEnemyTeam);
            });
            return newEnemyTeam;
        }

        void OnTakeFormPool(EnemyTeam enemyTeam)
        {
            enemyTeam.gameObject.SetActive(true);
            aliveObject.Add(enemyTeam);
        }

        void OnReturnToPool(EnemyTeam enemyTeam)
        {
            enemyTeam.Close();
            enemyTeam.OnAllMemberGone.RemoveAllListeners();
            enemyTeam.gameObject.SetActive(false);
            aliveObject.Remove(enemyTeam);
        }

        void OnDestroyPoolObject(EnemyTeam enemyTeam)
        {
            GameObject.Destroy(enemyTeam.gameObject);
        }
        #endregion
    }
}
