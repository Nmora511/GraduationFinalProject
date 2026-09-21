using UnityEngine;
using UnityEngine.InputSystem;

// TODO: Attack and Animations (All code is commented)
public class Player : MonoBehaviour
{
    public float healthPoints;
    public float moveSpeed;
    public float rotationSpeed;

    private CharacterController controller;
    //private Animator animator;

    private Vector3 inputDirection;
    private Vector2 moveInput;

    private Vector3 FinalVelocity;

    //private bool isAttacking = false;
    //private float attackCooldown = 0.53f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        //animator = GetComponent<Animator>();

        //PlayerInput playerInput = GetComponent<PlayerInput>();
        //sprintAction = playerInput.actions["Sprint"];
    }

    void Update()
    {
        UpdateHorizontalVelocity();
        //UpdateAttackStatus();

        controller.Move(FinalVelocity * Time.deltaTime);
    }

    // Movement Handlers
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void UpdateHorizontalVelocity()
    {
        // Fixes movimentation based on camera angle
        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;

        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        inputDirection = (camForward * moveInput.y + camRight * moveInput.x).normalized;
        
        float currentSpeed = 0f;

        if (inputDirection != Vector3.zero) //&& (!isAttacking ))
        {
            Quaternion targetRotation = Quaternion.LookRotation(inputDirection);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            currentSpeed = moveSpeed;
        }
        //else if (inputDirection == Vector3.zero)
        //{
        //    animator.SetBool("isSprinting", false);
        //}

        FinalVelocity = inputDirection * currentSpeed;
        //animator.SetFloat("Speed", currentSpeed);
    }

    // Atack Handlers
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
