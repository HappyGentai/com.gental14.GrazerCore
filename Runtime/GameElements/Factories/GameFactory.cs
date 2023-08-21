using UnityEngine;
using GrazerCore.GameElements;
using GrazerCore.Effects;

namespace GrazerCore.Factories
{
    public class GameFactory : MonoBehaviour
    {
        public static FactoryProduct GetProduct(GameObject corePrefab)
        {
            var type = TypeCheck(corePrefab);
            if (type == FactoryObjectType.UNDEFINED)
            {
                return new FactoryProduct(null, FactoryObjectType.UNDEFINED);
            }

            return TakeProduct(corePrefab, type);
        }

        private static FactoryProduct TakeProduct(GameObject _object, FactoryObjectType type)
        {
            switch (type)
            {
                case FactoryObjectType.ENEMY:
                    var enemyPrefab = _object.GetComponent<Enemy>();
                    var enemy = EnemyFactory.GetEnemy(enemyPrefab);
                    return new FactoryProduct(enemy.gameObject, FactoryObjectType.ENEMY);
                case FactoryObjectType.BULLET:
                    var bulletPrefab = _object.GetComponent<Bullet>();
                    var bullet = BulletFactory.GetBullet(bulletPrefab);
                    return new FactoryProduct(bullet.gameObject, FactoryObjectType.BULLET);
                case FactoryObjectType.EFFECT:
                    var effectPrefab = _object.GetComponent<SFXEffecter>();
                    var effect = EffectFactory.GetEffect(effectPrefab);
                    return new FactoryProduct(effect.gameObject, FactoryObjectType.EFFECT);
                case FactoryObjectType.PICKUPOBJECT:
                    var pickUpObjectPrefab = _object.GetComponent<PickUpObject>();
                    var pickUpObject = PickUpObjectFactory.GetPickUpObject(pickUpObjectPrefab);
                    return new FactoryProduct(pickUpObject.gameObject, FactoryObjectType.PICKUPOBJECT);
                default:
                    return new FactoryProduct(null, FactoryObjectType.UNDEFINED);
            }
        }

        private static FactoryObjectType TypeCheck(GameObject _object)
        {
            if (_object.GetComponent<Enemy>() != null)
            {
                return FactoryObjectType.ENEMY;
            }
            else if (_object.GetComponent<Bullet>() != null)
            {
                return FactoryObjectType.BULLET;
            }
            else if (_object.GetComponent<SFXEffecter>() != null)
            {
                return FactoryObjectType.EFFECT;
            }
            else if (_object.GetComponent<SFXEffecter>() != null)
            {
                return FactoryObjectType.EFFECT;
            }
            else
            {
                return FactoryObjectType.UNDEFINED;
            }
        }
    }
}
