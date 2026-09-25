using UnityEngine;

public class Trap : MonoBehaviour
{
    private static readonly int WasActivated = Animator.StringToHash("wasActivated");
    private Animator _animator;
    
    private void Start()
    {
        _animator = GetComponent<Animator>();    
    }
    
    public void OnCollisionStay(Collision collision)
    {
        var contact = collision.GetContact(0);
        
        // Check if normal points up
        if (!(Vector3.Dot(contact.normal, Vector3.up) > 0.5f)) return;
        Debug.Log("Colidiu");
        var entityCollided =  collision.gameObject.GetComponent<Entity>();
        if (entityCollided == null) return;
        Debug.Log("Entity collided");
        
        _animator.SetTrigger(WasActivated);
        

    }
}
