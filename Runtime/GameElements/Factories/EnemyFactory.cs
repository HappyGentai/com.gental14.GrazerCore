using System.Collections.Generic;
using UnityEngine.Events;
using GrazerCore.GameElements;

namespace GrazerCore.Factories
{
    public class EnemyFactory
    {
        private static List<EnemyPool> enemyPools = new List<EnemyPool>();

        /// <summary>
        /// If want know enemy class get by some script, can subscribe this event to get enemy class. 
        /// That will trigger when some script call Function-EnemyFactory.GetEnemy.
        /// </summary>
        private static UnityEvent<Enemy> onEnemyGet = null;
        public static UnityEvent<Enemy> OnEnemyGet
        {
            get { return onEnemyGet; }
        }

        /// <summary>
        /// If want know enemy class when return to pool, can subscribe this event to get enemy class. 
        /// </summary>
        private static UnityEvent<Enemy> onEnemyReturnToPool = new UnityEvent<Enemy>();
        public static UnityEvent<Enemy> OnEnemyReturnToPool
        {
            get { return onEnemyReturnToPool; }
        }

        public static Enemy GetEnemy(Enemy _CoreEnemy)
        {
            //  Search in list
            var poolCount = enemyPools.Count;
            for (int index = 0; index < poolCount; ++index)
            {
                var enemyPool = enemyPools[index];
                var checkEnemy = enemyPool.CoreEnemy;
                if (checkEnemy == _CoreEnemy)
                {
                    return SetOnEnemyGetInvoke(enemyPool.GetEnemy());
                }
            }

            //  If no enemy pool in list, create one and take
            var newEnemyPool = new EnemyPool(_CoreEnemy);
            enemyPools.Add(newEnemyPool);
            return SetOnEnemyGetInvoke(newEnemyPool.GetEnemy());

            Enemy SetOnEnemyGetInvoke(Enemy enemy)
            {
                if (onEnemyGet != null)
                {
                    onEnemyGet.Invoke(enemy);
                }
                return enemy;
            }
        }

        public static List<Enemy> GetAliveEnemys()
        {
            var aliveEnemys = new List<Enemy>();
            var poolCount = enemyPools.Count;
            for (int index = 0; index < poolCount; ++index)
            {
                var enemys = enemyPools[index].AliveObject;
                aliveEnemys.AddRange(enemys);
            }
            return aliveEnemys;
        }

        public static void ReleaseAll()
        {
            var poolsCount = enemyPools.Count;
            for (int index = 0; index < poolsCount; ++index)
            {
                var enemyPool = enemyPools[index];
                enemyPool.ReleaseAll();
            }
        }

        public static void DisposeAll()
        {
            var poolsCount = enemyPools.Count;
            for (int index = 0; index < poolsCount; ++index)
            {
                var enemyPool = enemyPools[index];
                enemyPool.Dispose();
            }
        }
    }
}
