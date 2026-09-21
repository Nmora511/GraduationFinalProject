using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class FollowingEnemy : MonoBehaviour
{
    // The Finite State Machine (FSM)
    public enum EnemyState { Idle, Chasing, Investigating, Attacking }
    
    [Header("AI State")]
    public EnemyState currentState = EnemyState.Idle;

    [Header("Target Reference")]
    public Transform target;

    [Header("Vision & Senses Settings")]
    public float viewRadius = 20f; 
    [Range(0, 360)]
    public float viewAngle = 150f; 
    public float proximityRadius = 2f; 
    public LayerMask obstacleLayer;

    [Header("Advanced AI Feel")]
    public Vector3 eyeOffset = new Vector3(0, 1f, 0);
    public float investigationOvershoot = 1.5f;

    private NavMeshAgent agent;
    private float originalStoppingDistance;

    private Vector3 lastKnownPosition;
    private Vector3 previousPlayerPosition;

    private Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        originalStoppingDistance = agent.stoppingDistance;

        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (target == null) return;

        switch (currentState)
        {
            case EnemyState.Idle:
                agent.isStopped = true;
                
                if (CanSeePlayer())
                {
                    agent.stoppingDistance = originalStoppingDistance;
                    currentState = EnemyState.Chasing;
                }
                break;

            case EnemyState.Chasing:
                agent.isStopped = false;
                agent.SetDestination(target.position);

                CalculateLastKnownPosition();
                previousPlayerPosition = target.position;

                animator.SetBool("IsChasing", true);
                
                if (LostLineOfSight())
                {
                    agent.stoppingDistance = 0f;
                    currentState = EnemyState.Investigating;
                }
                else if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    FaceTarget();
                }
                break;

            case EnemyState.Investigating:
                agent.isStopped = false;
                agent.SetDestination(lastKnownPosition);

                if (CanSeePlayer()) 
                {
                    agent.stoppingDistance = originalStoppingDistance;
                    currentState = EnemyState.Chasing;
                }
                else if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
                {
                    FaceTarget();
                    currentState = EnemyState.Idle;
                    animator.SetBool("IsChasing", false);
                }
                break;
        }
    }

    bool CanSeePlayer()
    {
        Vector3 directionToPlayer = target.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        bool isTooClose = distanceToPlayer <= proximityRadius;
        
        bool isInSight = (distanceToPlayer <= viewRadius) && (Vector3.Angle(transform.forward, directionToPlayer) <= viewAngle / 2f);

        if (isTooClose || isInSight)
        {
            if (!Physics.Raycast(transform.position, directionToPlayer.normalized, distanceToPlayer, obstacleLayer))
            {
                return true; 
            }
        }
        return false;
    }

    bool LostLineOfSight()
    {
        Vector3 rayStart = transform.position + eyeOffset;
        Vector3 rayTarget = target.position + eyeOffset;

        Vector3 directionToPlayer = rayTarget - rayStart;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer > viewRadius * 1.5f) return true;

        return Physics.Raycast(transform.position, directionToPlayer.normalized, distanceToPlayer, obstacleLayer);
    }

    void FaceTarget()
    {
        var turnTowardNavSteeringTarget = agent.steeringTarget;

        Vector3 direction = (turnTowardNavSteeringTarget - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

    }

    void CalculateLastKnownPosition()
    {
        Vector3 playerMoveDir = (target.position - previousPlayerPosition).normalized;

        // If the target is moving, extrapolate the last known position forward
        if ((target.position - previousPlayerPosition).magnitude > 0.01f)
        {
            lastKnownPosition = target.position + (playerMoveDir * investigationOvershoot);
        }
        else
        {
            lastKnownPosition = target.position;
        }
    }
}