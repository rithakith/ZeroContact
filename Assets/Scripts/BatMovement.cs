using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BatMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 2f;
    public float detectionRange = 10f;
    public float verticalFloatAmount = 0.5f;
    public float verticalFloatSpeed = 2f;
    
    [Header("References")]
    public Transform player;
    
    private Rigidbody2D rb;
    private float startY;
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
        startY = transform.position.y;
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
        
        if (distanceToPlayer <= detectionRange)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            
            float horizontalMovement = direction.x * speed;
            rb.linearVelocity = new Vector2(horizontalMovement, rb.linearVelocity.y);
            
            if (direction.x > 0 && !IsFacingRight)
            {
                IsFacingRight = true;
            }
            else if (direction.x < 0 && IsFacingRight)
            {
                IsFacingRight = false;
            }
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
        
        floatTimer += Time.fixedDeltaTime;
        float newY = startY + Mathf.Sin(floatTimer * verticalFloatSpeed) * verticalFloatAmount;
        rb.position = new Vector2(rb.position.x, newY);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}