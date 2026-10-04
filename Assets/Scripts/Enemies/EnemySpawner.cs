using UnityEngine;
using PlayerClass = Player.Player;

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
                var spawnRotation = Quaternion.identity;
                
                if (PlayerClass.PlayerInstance is not null)
                {
                    var directionToPlayer = PlayerClass.PlayerInstance.transform.position - transform.position;
                    directionToPlayer.y = 0f;
                    
                    if (directionToPlayer.sqrMagnitude > 0.001f)
                    {
                        spawnRotation = Quaternion.LookRotation(directionToPlayer.normalized);
                    }
                }
                
                var unitInstance = Instantiate(EnemyPrefab, transform.position, spawnRotation);
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
