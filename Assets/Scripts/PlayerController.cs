using UnityEngine;
using UnityEngine.InputSystem;

// TODO: Attack and Animations (All code is commented)
public class Player : MonoBehaviour
{
    [Header("Player Stats")]
    public float HealthPoints = 10;

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

    private Vector3 _inputDirection;
    private Vector2 _moveInput;

    private Vector3 _finalVelocity;

    private bool _isDashing;
    private float _dashTimer;
    private float _dashCooldownTimer;

    //private bool isAttacking = false;
    //private float attackCooldown = 0.53f;

    private void Start()
    {
        _controller = GetComponent<CharacterController>();
        _mainCamera = Camera.main;
        //animator = GetComponent<Animator>();

        //PlayerInput playerInput = GetComponent<PlayerInput>();
        //sprintAction = playerInput.actions["Sprint"];
    }

    private void Update()
    {
        UpdateHorizontalVelocity();
        //UpdateAttackStatus();

        _controller.Move(_finalVelocity * Time.deltaTime);
    }

    // Movement Handlers
    public void OnMove(InputValue value)
    {
        _moveInput = value.Get<Vector2>();
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

    // Attack Handlers
    //public void OnAttack(InputValue inputValue)
    //{
    //    if (!isAttacking)
    //    {
    //        animator.SetTrigger("BasicAttack");
    //        isAttacking = true;
    //        basicSlash.Play();
    //    }
    //}

    //void UpdateAttackStatus()
    //{
    //    if (isAttacking)
    //    {
    //        attackCooldown -= Time.deltaTime;
    //        if (attackCooldown <= 0f)
    //        {
    //            isAttacking = false;
    //            attackCooldown = 0.53f;
    //        }
    //    }
    //}
}
