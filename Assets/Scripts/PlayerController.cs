using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using NUnit.Framework;
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    private Vector2 movement;

    public float CurrentMoveSpeed
    {
        get
        {
            if (IsMoving)
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
}
