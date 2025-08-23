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
    private bool gameOver = false;

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


    void FixedUpdate()
    {
        if (gameOver || player == null) return;

        Vector2 directionToPlayer = (player.position - transform.position).normalized;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            // Move towards player both horizontally and vertically
            Vector2 targetVelocity = new Vector2(directionToPlayer.x, directionToPlayer.y) * speed;
            rb.linearVelocity = targetVelocity;

            // Face the correct direction
            if (directionToPlayer.x > 0 && !IsFacingRight)
            {
                IsFacingRight = true;
            }
            else if (directionToPlayer.x < 0 && IsFacingRight)
            {
                IsFacingRight = false;
            }
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
        }
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}