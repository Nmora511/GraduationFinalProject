using UnityEngine;

namespace Traps
{
    public class DamageHitbox : MonoBehaviour
    {
        public float Damage;
        
        private void OnTriggerStay(Collider other)
        {
            var entityCollided = other.GetComponent<Entity>();
            if (entityCollided == null) return;
            entityCollided.OnHit(Damage, transform.position);
        }
    }
}