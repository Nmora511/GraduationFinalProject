using UnityEngine;
using UnityEngine.AI;

namespace Enemies
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Enemy : MonoBehaviour
    {
        [Header("Target Reference")]
        public Transform Target;
    
        [Header("Vision & Senses Settings")]
        public float ViewRadius = 20f; 
        [Range(0, 360)]
        public float ViewAngle = 150f; 
        public float ProximityRadius = 3f; 
        public LayerMask ObstacleLayer;

        [Header("Advanced AI Feel")]
        public Vector3 EyeOffset = new Vector3(0, 1f, 0);
        public float InvestigationOvershoot = 1.5f;
    
        [Header("Combat Settings")]
        public float HealthPoints = 20f;

        protected NavMeshAgent navMeshAgent;
        protected Animator animator;

        protected virtual void Start()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            animator = GetComponentInChildren<Animator>();
        }
        
    }
}
