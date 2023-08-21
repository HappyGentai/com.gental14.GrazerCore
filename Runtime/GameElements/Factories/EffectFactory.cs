using System.Collections.Generic;
using UnityEngine;
using GrazerCore.Effects;
using UnityEngine.Pool;

namespace GrazerCore.Factories
{
    public class EffectFactory
    {
        private static List<EffectPool> effectPools = new List<EffectPool>();

        public static SFXEffecter GetEffect(SFXEffecter _CoreEffect)
        {
            var poolsCount = effectPools.Count;
            for (int index = 0; index < poolsCount; ++index)
            {
                var effectPool = effectPools[index];
                var checkEffect = effectPool.CoreEffect;
                if (checkEffect == _CoreEffect)
                {
                    return effectPool.GetEffect();
                }
            }

            var newEffectPool = new EffectPool(_CoreEffect);
            effectPools.Add(newEffectPool);
            return newEffectPool.GetEffect();
        }

        public static void ReleaseAll()
        {
            var poolsCount = effectPools.Count;
            for (int index = 0; index < poolsCount; ++index)
            {
                var effectPool = effectPools[index];
                effectPool.ReleaseAll();
            }
        }

        public static void DisposeAll()
        {
            var poolsCount = effectPools.Count;
            for (int index = 0; index < poolsCount; ++index)
            {
                var effectPool = effectPools[index];
                effectPool.Dispose();
            }
        }
    }

    public class EffectPool
    {
        private SFXEffecter m_CoreEffect = null;
        public SFXEffecter CoreEffect
        {
            get { return m_CoreEffect; }
        }

        private ObjectPool<SFXEffecter> effectPool = null;
        private List<SFXEffecter> aliveObject = new List<SFXEffecter>();

        private GameObject storagePlace = null;

        public EffectPool(SFXEffecter _CoreEffect)
        {
            m_CoreEffect = _CoreEffect;
            //  Create pool
            effectPool = new ObjectPool<SFXEffecter>(CreatePoolItem,
                OnTakeFromPool, OnReturnToPool, OnDestroyPoolObject, false);
        }

        public void Dispose()
        {
            effectPool.Dispose();
        }

        public SFXEffecter GetEffect()
        {
            var effect = effectPool.Get();
            return effect;
        }

        public void ReleaseAll()
        {
            var aliveCount = aliveObject.Count;
            for (int index = 0; index < aliveCount; ++index)
            {
                effectPool.Release(aliveObject[0]);
            }
            aliveObject.Clear();
        }

        private SFXEffecter CreatePoolItem()
        {
            var newEffect = GameObject.Instantiate<SFXEffecter>(m_CoreEffect);
            newEffect.OnEffectDone.AddListener(() => {
                effectPool.Release(newEffect);
            });
            if (storagePlace == null)
            {
                storagePlace = new GameObject("EffectPoolItem_" + m_CoreEffect.name);
                storagePlace.transform.localPosition = Vector3.zero;
            }
            newEffect.transform.parent = storagePlace.transform;
            return newEffect;
        }

        private void OnReturnToPool(SFXEffecter effect)
        {
            effect.StopSFX();
            effect.gameObject.SetActive(false);
            aliveObject.Remove(effect);
        }

        private void OnTakeFromPool(SFXEffecter effect)
        {
            effect.gameObject.SetActive(true);
            aliveObject.Add(effect);
        }

        private void OnDestroyPoolObject(SFXEffecter effect)
        {
            effect.StopSFX();
            GameObject.Destroy(effect.gameObject);
        }
    }
}
