using UnityEngine;

namespace Enemies
{
    public class EnemySpawner : MonoBehaviour
    {
        public float SpawnInterval = 0.5f;
        public GameObject EnemyPrefab;

        private Enemy _currentEnemyInstance;
        private float _spawnTimer;

        private void Update()
        {
            if (_spawnTimer <= 0)
            {
                var unitInstance = Instantiate(EnemyPrefab, transform.position, Quaternion.identity);
                _currentEnemyInstance = unitInstance.GetComponent<Enemy>();
                _spawnTimer = SpawnInterval;
            }
            else
            {
                _spawnTimer -= Time.deltaTime;
            }
        }
    }
}
