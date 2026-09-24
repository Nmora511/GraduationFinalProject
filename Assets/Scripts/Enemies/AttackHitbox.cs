using UnityEngine;

namespace Enemies
{
    public class AttackHitbox : MonoBehaviour
    {
        public float Damage;
        public float InvulnerabilityDuration = 0.2f;

        private float _hitTimer;
        private bool _isInvulnerable;

        private void Update()
        {
            if (_hitTimer > 0f)
            {
                _hitTimer -= Time.deltaTime;
            }
            else
            {
                _isInvulnerable = false;
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.CompareTag("Player")) return;
            if (_isInvulnerable) return;
            other.GetComponent<Player.Player>().OnHit(Damage);
            _isInvulnerable = true;
            _hitTimer = InvulnerabilityDuration;
        }
    }
}
