using System.Collections.Generic;
using GrazerCore.Effects;

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
}
