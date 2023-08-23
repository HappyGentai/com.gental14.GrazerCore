using System.Collections.Generic;
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
}
