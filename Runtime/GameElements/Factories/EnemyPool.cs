using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using GrazerCore.GameElements;

namespace GrazerCore.Factories
{
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
