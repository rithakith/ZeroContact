using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using NUnit.Framework;
[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections))]
public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    public float airwalkSpeed = 4f;
    public float jumpImpulse = 10f;
    private Vector2 movement;

    TouchingDirections touchingDirections;

    [Header("Shield")]
    public Animator shieldAnimator;   // reference to shield child Animator
    private bool shieldActive = false;

    public float CurrentMoveSpeed
    {
        get
        {
            if (IsMoving && !touchingDirections.IsOnWall)
            {
                if (touchingDirections.IsGround)
                {
                    if (IsRunning)
                    {
                        return runSpeed;
                    }
                    else
                    {
                        return walkSpeed;
                    }
                }
                else
                {
                    // Air movement
                    return airwalkSpeed;
                }

            }
            else
            {
                return 0;
            }
        }
    }
    private Rigidbody2D rb;
    Animator animator;


    [SerializeField]
    private bool _isMoving = false;

    public bool IsMoving
    {
        get
        {
            return _isMoving;
        }
        private set
        {
            _isMoving = value;
            animator.SetBool(AnimationStrings.IsMoving, value);
        }
    }

    [SerializeField]
    private bool _isRunning = false;

    public bool IsRunning
    {
        get
        {
            return _isRunning;
        }
        private set
        {
            _isRunning = value;
            animator.SetBool(AnimationStrings.IsRunning, value);
        }
    }

    public bool _isFacingRight = true;


    public bool IsFacingRight
    {
        get
        {
            return _isFacingRight;
        }
        private set
        {
            if (_isFacingRight != value)
            {
                Vector3 localScale = transform.localScale;
                localScale.x *= -1; // Just flip X sign
                transform.localScale = localScale;
            }
            _isFacingRight = value;
        }

    }

    public void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirections = GetComponent<TouchingDirections>();
    }

    // Called before fist frame update
    void Start()
    {

    }

    // Called every frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(movement.x * CurrentMoveSpeed, rb.linearVelocity.y);
        animator.SetFloat(AnimationStrings.yVelocity, rb.linearVelocity.y);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movement = context.ReadValue<Vector2>();

        IsMoving = movement != Vector2.zero;

        SetFacingDirection(movement);
    }

    private void SetFacingDirection(Vector2 movement)
    {
        if (movement.x > 0 && !IsFacingRight)
        {
            IsFacingRight = true;
        }
        else if (movement.x < 0 && IsFacingRight)
        {
            IsFacingRight = false;
        }
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        Debug.Log("Run context: " + context.phase);
        if (context.started)
        {
            IsRunning = true;
        }
        else if (context.canceled)
        {
            IsRunning = false;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        //Todo: check if alive as well
        if (context.started && touchingDirections.IsGround)
        {
            animator.SetTrigger(AnimationStrings.jump);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpImpulse);

        }

    }

    public void OnShield(InputAction.CallbackContext context)
    {
        if (context.started && !shieldActive)
        {
            shieldAnimator.SetTrigger(AnimationStrings.powerup);
            shieldActive = true;
        }
        else if (context.canceled && shieldActive)
        {
            shieldAnimator.SetTrigger(AnimationStrings.powerdown);
            shieldActive = false;
        }
    }
}

    
