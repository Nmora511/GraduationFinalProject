using System.Collections;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    [Header("Combat Settings")]
    public float HealthPoints = 20f;

    private Renderer[] _entityRenderers;
    private bool _hitFlashIsExecuting = false;
    
    protected virtual void Start()
    {
        _entityRenderers = GetComponentsInChildren<Renderer>();
    }

    public virtual void OnHit(float damage)
    {
        if (!_hitFlashIsExecuting)
        {
            StartCoroutine(HitFlash());
        }
        
        HealthPoints -= damage;
        if (HealthPoints <= 0)
        {
            Death();
        }
    }

    private IEnumerator HitFlash()
    {
        _hitFlashIsExecuting = true;
        var originalColors = new Color[_entityRenderers.Length];
        for (var i = 0; i < _entityRenderers.Length; i++)
        {
            originalColors[i] = _entityRenderers[i].material.color;
        }

        ChangeEntityColor(Color.red);
        yield return new WaitForSeconds(0.1f);
        ChangeEntityColor(Color.white);
        yield return new WaitForSeconds(0.1f);
        ChangeEntityColor(Color.red);
        yield return new WaitForSeconds(0.1f);

        for (var i = 0; i < _entityRenderers.Length; i++)
        {
            _entityRenderers[i].material.color = originalColors[i];
        }
        _hitFlashIsExecuting = false;
    }

    private void ChangeEntityColor(Color newColor)
    {
        foreach (var renderer in _entityRenderers)
        {
            renderer.material.color = newColor;
        }
    }

    protected virtual void Death()
    {
        
    }

}
