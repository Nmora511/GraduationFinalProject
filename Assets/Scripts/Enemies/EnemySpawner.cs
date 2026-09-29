using UnityEngine;

namespace Enemies
{
    public class EnemySpawner : MonoBehaviour
    {
        public float SpawnInterval = 0.5f;
        public int maxEnemiesQuantity = 1;
        public GameObject EnemyPrefab;
        
        private Enemy _currentEnemyInstance;
        private float _spawnTimer;
        private int _enemiesQuantity = 0;

        private void Update()
        {
            if (_enemiesQuantity >= maxEnemiesQuantity) return;
            
            if (_spawnTimer <= 0)
            {
                var unitInstance = Instantiate(EnemyPrefab, transform.position, Quaternion.identity);
                _currentEnemyInstance = unitInstance.GetComponent<Enemy>();
                _spawnTimer = SpawnInterval;
                _enemiesQuantity++;
            }
            else
            {
                _spawnTimer -= Time.deltaTime;
            }
        }
    }
}
