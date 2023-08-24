using UnityEngine;
using GrazerCore.GameElements;
using GrazerCore.Factories;

namespace SkateHero.Test
{
    public class TestEnemySpawner : MonoBehaviour
    {
        [SerializeField]
        private Enemy m_EnemyPrefab = null;

        // Start is called before the first frame update
        void Start()
        {
            EnemyFactory.GetEnemy(m_EnemyPrefab);
        }
    }
}
