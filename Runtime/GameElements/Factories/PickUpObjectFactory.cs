using System.Collections.Generic;
using UnityEngine;
using GrazerCore.GameElements;
using UnityEngine.Pool;

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

    public class PickUpObjectPool
    {
        private PickUpObject m_CorePickUpObject = null;
        public PickUpObject CorePickUpObject
        {
            get { return m_CorePickUpObject; }
        }

        private ObjectPool<PickUpObject> pickUpObjectPool = null;
        private List<PickUpObject> aliveObject = new List<PickUpObject>();

        private GameObject storagePlace = null;

        public PickUpObjectPool(PickUpObject _CorePickUpObject)
        {
            m_CorePickUpObject = _CorePickUpObject;
            //  Create pool
            pickUpObjectPool = new ObjectPool<PickUpObject>(CreatePoolItem,
                OnTakeFromPool, OnReturnToPool, OnDestroyPoolObject, false);
        }

        public void Dispose()
        {
            pickUpObjectPool.Dispose();
        }

        public PickUpObject GetPickUpObject()
        {
            var pickUpObject = pickUpObjectPool.Get();
            return pickUpObject;
        }

        public void ReleaseAll()
        {
            var aliveCount = aliveObject.Count;
            for (int index = 0; index < aliveCount; ++index)
            {
                pickUpObjectPool.Release(aliveObject[0]);
            }
            aliveObject.Clear();
        }

        private PickUpObject CreatePoolItem()
        {
            var newPickUpObject = GameObject.Instantiate<PickUpObject>(m_CorePickUpObject);
            newPickUpObject.OnPickDoen.AddListener(() =>
            {
                pickUpObjectPool.Release(newPickUpObject);
            });
            if (storagePlace == null)
            {
                storagePlace = new GameObject("PickUpPoolItem_"+ m_CorePickUpObject.name);
                storagePlace.transform.localPosition = Vector3.zero;
            }
            newPickUpObject.transform.parent = storagePlace.transform;
            return newPickUpObject;
        }

        private void OnReturnToPool(PickUpObject pickUpObject)
        {
            pickUpObject.gameObject.SetActive(false);
            aliveObject.Remove(pickUpObject);
        }

        private void OnTakeFromPool(PickUpObject pickUpObject)
        {
            pickUpObject.gameObject.SetActive(true);
            pickUpObject.ReSetState();
            aliveObject.Add(pickUpObject);
        }

        private void OnDestroyPoolObject(PickUpObject pickUpObject)
        {
            GameObject.Destroy(pickUpObject.gameObject);
        }
    }
}
