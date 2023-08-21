using UnityEngine;

namespace GrazerCore.Factories
{
    public struct FactoryProduct
    {
        public GameObject Product;

        public FactoryObjectType Type;

        public FactoryProduct(GameObject product, FactoryObjectType type)
        {
            Product = product;
            Type = type;
        }
    }
}
