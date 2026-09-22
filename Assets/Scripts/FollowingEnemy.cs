using System;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class FollowingEnemy : MonoBehaviour
{
    private static readonly int IsChasing = Animator.StringToHash("IsChasing");
    private static readonly int HasAttacked = Animator.StringToHash("hasAttacked");


    // The Finite State Machine (FSM)
    public enum EnemyState { Idle, Chasing, Investigating, Attacking }
    
    [Header("AI State")]
    public EnemyState CurrentState = EnemyState.Idle;

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
    public float AttackCooldown = 2.7f;
    public float AttackDamage = 5f;
    private float _attackTimer; 

    private NavMeshAgent _navMeshAgent;
    private float _originalStoppingDistance;

    private Vector3 _lastKnownPosition;
    private Vector3 _previousPlayerPosition;
    private float _distanceToPlayer;

    private Animator _animator;

    private void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _originalStoppingDistance = _navMeshAgent.stoppingDistance;

        _animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (Target is null) return;

        _distanceToPlayer = (Target.position - transform.position).magnitude;
        
        if (_attackTimer > 0f)
        {
            _attackTimer -= Time.deltaTime;
        }
        
        switch (CurrentState)
        {
            case EnemyState.Idle:
                _navMeshAgent.isStopped = true;
                
                
                if (CanSeePlayer())
                {
                    _navMeshAgent.stoppingDistance = _originalStoppingDistance;
                    CurrentState = EnemyState.Chasing;
                } 
                else if (_distanceToPlayer <= ProximityRadius * 1.5)
                {
                    RotateTowards(Target.position);
                }
                else {
                    RotateTowards(_lastKnownPosition);
                }
                break;

            case EnemyState.Chasing:
                _navMeshAgent.isStopped = false;
                _navMeshAgent.SetDestination(Target.position);
                
                CalculateLastKnownPosition();
                _previousPlayerPosition = Target.position;

                _animator.SetBool(IsChasing, true);
                
                var directionToPlayer = (Target.position - transform.position).normalized;
                var angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
                
                if (LostLineOfSight())
                {
                    _navMeshAgent.stoppingDistance = 0f;
                    CurrentState = EnemyState.Investigating;
                }
                else if (_distanceToPlayer <= ProximityRadius && _attackTimer <= 0f && angleToPlayer <= 45f)
                {
                    CurrentState = EnemyState.Attacking;
                    _attackTimer = AttackCooldown;
                    _animator.SetBool(IsChasing, false);
                    _animator.SetTrigger(HasAttacked);
                } 
                
                break;

            case EnemyState.Investigating:
                _navMeshAgent.isStopped = false;
                _navMeshAgent.SetDestination(_lastKnownPosition);
                
                if (CanSeePlayer()) 
                {
                    _navMeshAgent.stoppingDistance = _originalStoppingDistance;
                    CurrentState = EnemyState.Chasing;
                }
                else if (!_navMeshAgent.pathPending && _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
                {
                    _lastKnownPosition = _navMeshAgent.steeringTarget;
                    CurrentState = EnemyState.Idle; 
                    _animator.SetBool(IsChasing, false);
                }
                break;
            
            case EnemyState.Attacking:
                if (_attackTimer > 0f)
                {
                    _navMeshAgent.isStopped = true;
                    CalculateLastKnownPosition();
                }
                else
                {
                    CurrentState = EnemyState.Idle;
                }
                break;
            
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private bool CanSeePlayer()
    {
        var directionToPlayer = Target.position - transform.position;
        var distanceToPlayer = directionToPlayer.magnitude;

        var isTooClose = distanceToPlayer <= ProximityRadius;
        
        var isInSight = (distanceToPlayer <= ViewRadius) && (Vector3.Angle(transform.forward, directionToPlayer) <= ViewAngle / 2f);

        if (!isTooClose && !isInSight) return false;
        return !Physics.Raycast(transform.position, directionToPlayer.normalized, distanceToPlayer, ObstacleLayer);
    }

    private bool LostLineOfSight()
    {
        var rayStart = transform.position + EyeOffset;
        var rayTarget = Target.position + EyeOffset;

        var directionToPlayer = rayTarget - rayStart;
        var distanceToPlayer = directionToPlayer.magnitude;

        return distanceToPlayer > ViewRadius * 1.5f || Physics.Raycast(transform.position, directionToPlayer.normalized, distanceToPlayer, ObstacleLayer);
    }
    
    private void RotateTowards(Vector3 targetPosition)
    {
        var direction = targetPosition - transform.position;
        direction.y = 0f; 

        // Using 0.001f instead of checking for Vector3.zero prevents micro-jitters 
        if (direction.sqrMagnitude < 0.001f) return;
        var lookRotation = Quaternion.LookRotation(direction.normalized);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

    }

    private void CalculateLastKnownPosition()
    {
        var playerMoveDir = (Target.position - _previousPlayerPosition).normalized;

        // If the target is moving, extrapolate the last known position forward
        if ((Target.position - _previousPlayerPosition).magnitude > 0.01f)
        {
            _lastKnownPosition = Target.position + (playerMoveDir * InvestigationOvershoot);
        }
        else
        {
            _lastKnownPosition = Target.position;
        }
    }
}