using Enemies;
using UnityEngine;

namespace Player
{
    public class ScytheHitbox : MonoBehaviour
    {
        public float Damage;
        
        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.CompareTag("Enemy")) return;
            other.GetComponent<Enemy>().OnHit(Damage);
        }
    }
}