using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
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

    public class EnemyPool
    {
        private Enemy m_CoreEnemy = null;
        public Enemy CoreEnemy
        {
            get { return m_CoreEnemy; }
        }

        private ObjectPool<Enemy> enemyPool = null;
        private List<Enemy> aliveObject = new List<Enemy>();
        public List<Enemy> AliveObject
        {
            get { return aliveObject; }
        }

        private GameObject storagePlace = null;

        public EnemyPool(Enemy _CoreEnemy)
        {
            m_CoreEnemy = _CoreEnemy;
            enemyPool = new ObjectPool<Enemy>(CreatePoolItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, false);
        }

        public void Dispose()
        {
            enemyPool.Dispose();
        }

        public Enemy GetEnemy()
        {
            var enemy = enemyPool.Get();
            return enemy;
        }

        public void ReleaseAll()
        {
            int aliveEnemyCount = aliveObject.Count;
            for (int index = 0; index < aliveEnemyCount; index++)
            {
                enemyPool.Release(aliveObject[0]);
            }
            aliveObject.Clear();
        }

        private Enemy CreatePoolItem()
        {
            var newEnemy = GameObject.Instantiate<Enemy>(m_CoreEnemy);
            newEnemy.Initialization();
            newEnemy.OnRecycle.AddListener((Enemy _enemy) =>
            {
                enemyPool.Release(_enemy);
            });
            if (storagePlace == null)
            {
                storagePlace = new GameObject("EnemyPoolItem_" + m_CoreEnemy.name);
                storagePlace.transform.localPosition = Vector3.zero;
            }
            newEnemy.transform.parent = storagePlace.transform;
            return newEnemy;
        }

        private void OnReturnedToPool(Enemy enemy)
        {
            enemy.SleepObject();
            enemy.gameObject.SetActive(false);
            aliveObject.Remove(enemy);
            EnemyFactory.OnEnemyReturnToPool.Invoke(enemy);
        }

        private void OnTakeFromPool(Enemy enemy)
        {
            enemy.gameObject.SetActive(true);
            aliveObject.Add(enemy);
        }

        private void OnDestroyPoolObject(Enemy enemy)
        {
            GameObject.Destroy(enemy.gameObject);
        }
    }
}
