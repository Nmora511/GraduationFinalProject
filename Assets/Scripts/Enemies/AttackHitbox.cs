using UnityEngine;

namespace Enemies
{
    public class AttackHitbox : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                other.GetComponent<Player.Player>().OnHit();
            }
        }
    }
}
