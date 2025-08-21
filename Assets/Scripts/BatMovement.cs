using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BatMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 2f;
    public float detectionRange = 10f;
    public float hoverHeight = 3f;
    public float verticalFloatAmount = 0.5f;
    public float verticalFloatSpeed = 2f;
    
    [Header("Ground Detection")]
    public LayerMask groundLayer;
    public float groundCheckDistance = 10f;
    
    [Header("References")]
    public Transform player;
    
    private Rigidbody2D rb;
    private float floatTimer = 0f;
    
    private bool _isFacingRight = true;
    public bool IsFacingRight
    {
        get { return _isFacingRight; }
        private set
        {
            if (_isFacingRight != value)
            {
                transform.localScale *= new Vector2(-1, 1);
            }
            _isFacingRight = value;
        }
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
    }

    void Start()
    {
        if (player == null)
        {
            GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
            if (playerGO != null)
            {
                player = playerGO.transform;
            }
        }
    }

    void FixedUpdate()
    {
        if (player == null) return;
        
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
        // Horizontal movement towards player
        if (distanceToPlayer <= detectionRange)
        {
            // Only track player's X position, ignore Y
            float horizontalDistance = player.position.x - transform.position.x;
            float direction = Mathf.Sign(horizontalDistance);
            
            // Move horizontally
            float horizontalMovement = direction * speed;
            rb.linearVelocity = new Vector2(horizontalMovement, 0);
            
            // Face the correct direction
            if (direction > 0 && !IsFacingRight)
            {
                IsFacingRight = true;
            }
            else if (direction < 0 && IsFacingRight)
            {
                IsFacingRight = false;
            }
        }
        else
        {
            rb.linearVelocity = new Vector2(0, 0);
        }
        
        // Check ground ahead of bat in movement direction
        Vector2 checkPosition = transform.position;
        if (distanceToPlayer <= detectionRange && player != null)
        {
            // Look ahead in the direction we're moving toward the player
            float lookAheadDistance = 1f;
            float directionToPlayer = Mathf.Sign(player.position.x - transform.position.x);
            checkPosition.x += directionToPlayer * lookAheadDistance;
        }
        
        RaycastHit2D groundHit = Physics2D.Raycast(checkPosition, Vector2.down, groundCheckDistance, groundLayer);
        
        if (groundHit.collider != null)
        {
            // Calculate desired height above ground
            floatTimer += Time.fixedDeltaTime;
            float floatOffset = Mathf.Sin(floatTimer * verticalFloatSpeed) * verticalFloatAmount;
            float desiredY = groundHit.point.y + hoverHeight + floatOffset;
            
            // Apply vertical movement
            float verticalVelocity = (desiredY - transform.position.y) * 8f;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, verticalVelocity);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}