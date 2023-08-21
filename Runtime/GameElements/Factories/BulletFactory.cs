using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using GrazerCore.GameElements;

namespace GrazerCore.Factories
{
    public class BulletFactory
    {
        private static List<BulletPool> bulletPools = new List<BulletPool>();

        public static Bullet GetBullet(Bullet _CoreBullet)
        {
            //  Search in list
            var poolsCount = bulletPools.Count;
            for (int index = 0; index < poolsCount; ++index)
            {
                var bulletPool = bulletPools[index];
                var checkBullet = bulletPool.CoreBullet;
                if (checkBullet == _CoreBullet)
                {
                    return bulletPool.GetBullet();
                }
            }

            //  If no bullet pool in list, create one and take
            var newBulletPool = new BulletPool(_CoreBullet);
            bulletPools.Add(newBulletPool);
            return newBulletPool.GetBullet();
        }

        public static void ReleaseAll()
        {
            var poolsCount = bulletPools.Count;
            for (int index = 0; index < poolsCount; ++index)
            {
                var bulletPool = bulletPools[index];
                bulletPool.ReleaseAll();
            }
        }

        public static void DisposeAll()
        {
            int poolsCount = bulletPools.Count;
            for (int index = 0; index < poolsCount; ++index)
            {
                var bulletPool = bulletPools[index];
                bulletPool.Dispose();
            }
        }
    }

    public class BulletPool
    {
        /// <summary>
        /// New bullet Instantiate target
        /// </summary>
        private Bullet m_CoreBullet = null;
        public Bullet CoreBullet
        {
            get { return m_CoreBullet; }
        }

        private ObjectPool<Bullet> bulletPool = null;
        private List<Bullet> aliveObject = new List<Bullet>();

        private GameObject storagePlace = null;

        public BulletPool(Bullet _CoreBullet)
        {
            m_CoreBullet = _CoreBullet;
            /*
             *  Some time bullet will hit multiple target also call same time release to pool function
             *  In herem i prefer set ObjectPool's collectionCheck to false to avoid pool thrown exception 
             */
            bulletPool = new ObjectPool<Bullet>(CreatePoolItem, OnTakeFromPool, OnReturnedToPool, OnDestroyPoolObject, false);
        }

        public void Dispose()
        {
            bulletPool.Dispose();
        }

        public Bullet GetBullet()
        {
            var bullet = bulletPool.Get();
            return bullet;
        }

        public void ReleaseAll()
        {
            var aliveBulletCount = aliveObject.Count;
            for (int index = 0; index < aliveBulletCount; index++)
            {
                bulletPool.Release(aliveObject[0]);
            }
            aliveObject.Clear();
        }

        private Bullet CreatePoolItem()
        {
            var newBullet = GameObject.Instantiate<Bullet>(m_CoreBullet);
            newBullet.eventWhenBulletDead += () =>
            {
                bulletPool.Release(newBullet);
            };
            if (storagePlace == null)
            {
                storagePlace = new GameObject("PickUpPoolItem_" + m_CoreBullet.name);
                storagePlace.transform.localPosition = Vector3.zero;
            }
            newBullet.transform.parent = storagePlace.transform;
            return newBullet;
        }

        private void OnReturnedToPool(Bullet bullet)
        {
            bullet.gameObject.SetActive(false);
            aliveObject.Remove(bullet);
        }

        private void OnTakeFromPool(Bullet bullet)
        {
            bullet.gameObject.SetActive(true);
            bullet.MoveDir = Vector2.zero;
            aliveObject.Add(bullet);
        }

        private void OnDestroyPoolObject(Bullet bullet)
        {
            GameObject.Destroy(bullet.gameObject);
        }
    }
}
