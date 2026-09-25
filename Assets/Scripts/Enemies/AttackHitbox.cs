using UnityEngine;

namespace Enemies
{
    public class AttackHitbox : MonoBehaviour
    {
        public float Damage;
        
        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.CompareTag("Player")) return;
            other.GetComponent<Player.Player>().OnHit(Damage);
        }
    }
}
