using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingSpikeController : MonoBehaviour
{
    private bool gameOver = false;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float rotationSpeed = 360f;
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float groundCheckDistance = 1.5f;
    [SerializeField] private float slopeCheckDistance = 0.5f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask playerLayer;

    private Rigidbody2D rb;
    private bool movingRight = true;
    private Transform playerTransform;
    private float groundAngle = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    void OnEnable()
    {
        Damage.OnPlayerDeath += StopMoving;
    }

    void OnDisable()
    {
        Damage.OnPlayerDeath -= StopMoving;
    }

    void StopMoving()
    {
        gameOver = true;
        rb.linearVelocity = Vector2.zero;
    }

    private void Update()
    {
        if (gameOver) return;
        // Check ground angle for slope
        UpdateGroundAngle();

        // Check for ground edges
        if (!CheckGround())
        {
            Flip();
        }

        // Check for walls
        if (CheckWall())
        {
            Flip();
        }

        // Follow player if within range (only care about X direction)
        if (playerTransform != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
            if (distanceToPlayer < detectionRange)
            {
                // Only consider horizontal direction to player
                float playerDirX = playerTransform.position.x - transform.position.x;

                if (playerDirX < -0.1f && movingRight)
                {
                    Flip();
                }
                else if (playerDirX > 0.1f && !movingRight)
                {
                    Flip();
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (gameOver) return;
        // Move along slope
        float moveDirection = movingRight ? 1f : -1f;

        // Calculate movement vector based on ground angle
        Vector2 moveVector = new Vector2(Mathf.Cos(groundAngle * Mathf.Deg2Rad), Mathf.Sin(groundAngle * Mathf.Deg2Rad));
        moveVector *= moveDirection * moveSpeed;

        // Apply movement
        rb.linearVelocity = new Vector2(moveVector.x, moveVector.y);

        // Stick to ground
        StickToGround();

        // Rotate the spike
        transform.Rotate(0, 0, -moveDirection * rotationSpeed * Time.fixedDeltaTime);
    }

    private bool CheckGround()
    {
        float rayLength = 1f;
        Vector2 rayOrigin = transform.position + (movingRight ? Vector3.right : Vector3.left) * 0.5f;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, rayLength, groundLayer);

        Debug.DrawRay(rayOrigin, Vector2.down * rayLength, Color.red);

        return hit.collider != null;
    }

    private bool CheckWall()
    {
        float rayLength = 0.5f;
        Vector2 direction = movingRight ? Vector2.right : Vector2.left;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, rayLength, groundLayer);

        Debug.DrawRay(transform.position, direction * rayLength, Color.blue);

        return hit.collider != null;
    }

    private void Flip()
    {
        movingRight = !movingRight;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Deal damage to player
            // Damage damage = collision.gameObject.GetComponent<Damage>();

            // if (damage != null)
            // {
            //     damage.Health -= 1; // Deal 1 damage
            // }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

    private void UpdateGroundAngle()
    {
        // Check ground ahead in movement direction
        Vector2 checkPosition = transform.position;
        float lookAheadDistance = 0.5f;
        float moveDirection = movingRight ? 1f : -1f;
        checkPosition.x += moveDirection * lookAheadDistance;

        RaycastHit2D hit = Physics2D.Raycast(checkPosition, Vector2.down, groundCheckDistance, groundLayer);

        if (hit.collider != null)
        {
            // Calculate angle of ground ahead
            Vector2 groundNormal = hit.normal;
            groundAngle = Vector2.SignedAngle(Vector2.up, groundNormal);

            // If moving left, we need to flip the angle
            if (!movingRight)
            {
                groundAngle = -groundAngle;
            }
        }
        else
        {
            groundAngle = 0f;
        }
    }

    private void StickToGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);

        if (hit.collider != null)
        {
            // Add downward force to keep spike grounded
            float groundDistance = hit.distance;
            CircleCollider2D col = GetComponent<CircleCollider2D>();
            float targetDistance = col ? col.radius : 0.5f;

            if (groundDistance > targetDistance)
            {
                // Apply downward force if too far from ground
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y - 10f * Time.fixedDeltaTime);
            }
        }
    }
}