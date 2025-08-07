using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 movement;
    public bool IsMoving { get; private set; }

    public void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Called before fist frame update
    void Start()
    {

    }

    // Called every frame
    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(movement.x * walkSpeed, rb.linearVelocity.y);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movement = context.ReadValue<Vector2>();

        IsMoving = movement != Vector2.zero;
    }
}
