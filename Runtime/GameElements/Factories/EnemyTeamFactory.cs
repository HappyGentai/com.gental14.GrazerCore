using System.Collections.Generic;
using GrazerCore.GameElements.EnemyGroup;

namespace GrazerCore.Factories
{
    public class EnemyTeamFactory
    {
        private static List<EnemyTeamPool> enemyTeamPools = new List<EnemyTeamPool>();

        public static EnemyTeam GetEnemyTeam(EnemyTeamData teamDataPrefab)
        {
            //  Search in list
            var poolsCount = enemyTeamPools.Count;
            for (int index = 0; index < poolsCount; ++index)
            {
                var enemyTeamPool = enemyTeamPools[index];
                var checkTeamData = enemyTeamPool.CoreEnemyTeamData;
                if (checkTeamData == teamDataPrefab)
                {
                    return enemyTeamPool.GetEnemyTeam();
                }
            }

            // If no enemyTeam pool in list, create one and take
            var newPool = new EnemyTeamPool(teamDataPrefab);
            enemyTeamPools.Add(newPool);
            return newPool.GetEnemyTeam();
        }

        public static void ReleaseAll()
        {
            var poolsCount = enemyTeamPools.Count;
            for (int index = 0; index < poolsCount; ++index)
            {
                var enemyTeamPool = enemyTeamPools[index];
                enemyTeamPool.ReleaseAll();
            }
        }

        public static void DisposeAll()
        {
            var poolsCount = enemyTeamPools.Count;
            for (int index = 0; index < poolsCount; ++index)
            {
                var enemyTeamPool = enemyTeamPools[index];
                enemyTeamPool.Dispose();
            }
        }
    }
}
