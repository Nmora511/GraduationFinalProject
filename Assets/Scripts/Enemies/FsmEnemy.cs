using System;
using UnityEngine;
using UnityEngine.AI;

namespace Enemies
{
    public class FsmEnemy : Enemy
    {
        private static readonly int IsChasing = Animator.StringToHash("IsChasing");
        private static readonly int HasAttacked = Animator.StringToHash("hasAttacked");
        
        // The Finite State Machine (FSM)
        public enum EnemyState { Idle, Chasing, Investigating, Attacking }
    
        [Header("AI State")]
        public EnemyState CurrentState = EnemyState.Idle;
    
        [Header("Combat Settings")]
        public float AttackCooldown = 2.7f;
        public float AttackDamage = 5f;
        public Collider DamageCollider;
        private float _attackTimer; 
        
        private float _originalStoppingDistance;

        private Vector3 _lastKnownPosition;
        private Vector3 _previousPlayerPosition;
        private float _distanceToPlayer;
        
        private void Start()
        {
            base.Start();
            _originalStoppingDistance = navMeshAgent.stoppingDistance;
            DamageCollider.enabled = false;
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
                    navMeshAgent.isStopped = true;
                
                
                    if (CanSeePlayer())
                    {
                        navMeshAgent.stoppingDistance = _originalStoppingDistance;
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
                    navMeshAgent.isStopped = false;
                    navMeshAgent.SetDestination(Target.position);
                
                    CalculateLastKnownPosition();
                    _previousPlayerPosition = Target.position;

                    animator.SetBool(IsChasing, true);
                
                    var directionToPlayer = (Target.position - transform.position).normalized;
                    var angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
                
                    if (LostLineOfSight())
                    {
                        navMeshAgent.stoppingDistance = 0f;
                        CurrentState = EnemyState.Investigating;
                    }
                    else if (_distanceToPlayer <= ProximityRadius && _attackTimer <= 0f && angleToPlayer <= 45f)
                    {
                        CurrentState = EnemyState.Attacking;
                        _attackTimer = AttackCooldown;
                        animator.SetBool(IsChasing, false);
                        animator.SetTrigger(HasAttacked);
                        DamageCollider.enabled = true;
                    } 
                
                    break;

                case EnemyState.Investigating:
                    navMeshAgent.isStopped = false;
                    navMeshAgent.SetDestination(_lastKnownPosition);
                
                    if (CanSeePlayer()) 
                    {
                        navMeshAgent.stoppingDistance = _originalStoppingDistance;
                        CurrentState = EnemyState.Chasing;
                    }
                    else if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
                    {
                        _lastKnownPosition = navMeshAgent.steeringTarget;
                        CurrentState = EnemyState.Idle; 
                        animator.SetBool(IsChasing, false);
                    }
                    break;
            
                case EnemyState.Attacking:
                    if (_attackTimer > 0f)
                    {
                        navMeshAgent.isStopped = true;
                        CalculateLastKnownPosition();
                    }
                    else
                    {
                        CurrentState = EnemyState.Idle;
                        DamageCollider.enabled  = false;
                    }
                    break;
            
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void OnHit()
        {
        
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
}