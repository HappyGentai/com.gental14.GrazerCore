using System.Collections.Generic;
using GrazerCore.GameElements;

namespace GrazerCore.Factories
{
    public class PickUpObjectFactory
    {
        private static List<PickUpObjectPool> pickUpObjectPools = new List<PickUpObjectPool>();

        public static PickUpObject GetPickUpObject(PickUpObject _CorePickUpObject)
        {
            //  Search in list
            var poolCount = pickUpObjectPools.Count;
            for (int index = 0; index < poolCount; ++index)
            {
                var pickUpObjectPool = pickUpObjectPools[index];
                var checkPickUpObject = pickUpObjectPool.CorePickUpObject;
                if (checkPickUpObject == _CorePickUpObject)
                {
                    return pickUpObjectPool.GetPickUpObject();
                }
            }

            //  If no pickUpObjectPool in list, create one and take
            var newPickUpObjectPool = new PickUpObjectPool(_CorePickUpObject);
            pickUpObjectPools.Add(newPickUpObjectPool);
            return newPickUpObjectPool.GetPickUpObject();
        }

        public static void ReleaseAll()
        {
            var poolsCount = pickUpObjectPools.Count;
            for (int index = 0; index < poolsCount; ++index)
            {
                var pickUpObjectPool = pickUpObjectPools[index];
                pickUpObjectPool.ReleaseAll();
            }
        }

        public static void DisposeAll()
        {
            int poolsCount = pickUpObjectPools.Count;
            for (int index = 0; index < poolsCount; ++index)
            {
                var pickUpObjectPool = pickUpObjectPools[index];
                pickUpObjectPool.Dispose();
            }
        }
    }
}
