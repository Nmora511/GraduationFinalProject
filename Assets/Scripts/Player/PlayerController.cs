using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

// TODO: Attack and Animations (All code is commented)
namespace Player
{
    public class Player : Entity
    {
        private static readonly int Speed = Animator.StringToHash("speed");
        private static readonly int HasAttacked = Animator.StringToHash("hasAttacked");

        [Header("Combat Settings")] 
        public float AttackDamage;
        public ScytheHitbox ScytheHitbox;
        public float AttackDuration = 0.7f;
        public float AttackSlideFriction = 4f;
        
        [Header("Movement Settings")]
        public float MoveSpeed = 5f;
        public float RotationSpeed = 15f;

        [Header("Dash Settings")]
        public float DashSpeed = 15f;
        public float DashDuration = 0.2f;
        public float DashCooldown = 1f;

        private Camera _mainCamera;
    
        private CharacterController _controller;
        //private Animator animator;

        private Renderer _playerRenderer;
        
        private Vector3 _inputDirection;
        private Vector2 _moveInput;

        private Vector3 _finalVelocity;

        private bool _isDashing;
        private float _dashTimer;
        private float _dashCooldownTimer;
        
        private Animator _animator;

        private bool _isAttacking = false;
        private float _attackCooldown = 1.2f;
        private Collider _scytheCollider;

        private new void Start()
        {
            base.Start();
            _controller = GetComponent<CharacterController>();
            _mainCamera = Camera.main;
            _animator = GetComponentInChildren<Animator>();
            
            ScytheHitbox.Damage = AttackDamage;
            _scytheCollider = ScytheHitbox.GetComponent<Collider>();
            _scytheCollider.enabled = false;

            //PlayerInput playerInput = GetComponent<PlayerInput>();
            //sprintAction = playerInput.actions["Sprint"];
        }

        private void Update()
        {
            UpdateHorizontalVelocity();
            UpdateAttackStatus();

            _controller.Move(_finalVelocity * Time.deltaTime);
        }

        // Movement Handlers
        public void OnMove(InputValue value)
        {
            _moveInput = value.Get<Vector2>();
            _animator.SetFloat(Speed, _moveInput.magnitude);
        }

        public void OnDash(InputValue value)
        {
            if (value.isPressed && !_isDashing && _dashCooldownTimer <= 0f)
            {
                _isDashing = true;
                _dashTimer = DashDuration;
                _dashCooldownTimer = DashCooldown;
            }
        }

        private void UpdateHorizontalVelocity()
        {
            // Dash Handler
            if (_dashCooldownTimer > 0f)
            {
                _dashCooldownTimer -= Time.deltaTime;
            }

            if (_isDashing)
            {
                _dashTimer -= Time.deltaTime;
                if (_dashTimer <= 0f)
                {
                    _isDashing = false;
                }
                else
                {
                    _finalVelocity = transform.forward * DashSpeed;
                    return;
                }
            }
            
            if (_isAttacking)
            {
                // Reduz a velocidade gradualmente (Lerp) de AttackSlideSpeed até 0
                _finalVelocity = Vector3.Lerp(_finalVelocity, Vector3.zero, Time.deltaTime * AttackSlideFriction);
                return; // Encerra a função para ignorar os inputs do analógico/teclado
            }

            // Walking Handler
            // Fixes movimentation based on camera angle
            var camForward = _mainCamera.transform.forward;
            var camRight = _mainCamera.transform.right;

            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();

            _inputDirection = (camForward * _moveInput.y + camRight * _moveInput.x).normalized;
        
            var currentSpeed = 0f;

            if (_inputDirection != Vector3.zero) //&& (!isAttacking ))
            {
                var targetRotation = Quaternion.LookRotation(_inputDirection);

                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, RotationSpeed * Time.deltaTime);

                currentSpeed = MoveSpeed;
            }
            //else if (inputDirection == Vector3.zero)
            //{
            //    animator.SetBool("isSprinting", false);
            //}

            _finalVelocity = _inputDirection * currentSpeed;
            //animator.SetFloat("Speed", currentSpeed);
        }

        // Combat Handlers
        
        public void OnAttack(InputValue inputValue)
        {
            if (_isAttacking) return;
            
            _scytheCollider.enabled = true;
            _animator.SetTrigger(HasAttacked);
            _isAttacking = true;
        }

        private void UpdateAttackStatus()
        {
            if (!_isAttacking) return;
            
            _attackCooldown -= Time.deltaTime;
            
            if (_attackCooldown > 0f) return;
            
            _scytheCollider.enabled = false;
            _isAttacking = false;
            _attackCooldown = AttackDuration;
        }
    }
}
