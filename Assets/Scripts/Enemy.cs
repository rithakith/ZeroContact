using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 2f;
    public Transform player;
    private ShieldController shield;

    void Start()
    {
        shield = player.GetComponent<ShieldController>();
    }

    void Update()
    {
        if (player != null)
            transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!shield.IsShieldActive())
            {
                Debug.Log("Player hit! Take damage.");
                // Handle player damage here
            }
            else
            {
                Debug.Log("Blocked by shield.");
            }
        }
    }
}
