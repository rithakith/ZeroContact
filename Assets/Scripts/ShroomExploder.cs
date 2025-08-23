using UnityEngine;

public class ShroomExploder : MonoBehaviour
{
    public float explosionDelay = 1.5f;
    public int damage = 10;

    private Animator animator;
    private bool exploding = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!exploding && collision.CompareTag("Player"))
        {
            exploding = true;
            animator.SetTrigger("PlayerNear");
            Invoke(nameof(Explode), explosionDelay);
        }
    }

    void Explode()
    {
        animator.SetTrigger("ExplodeNow");

        // Damage player if inside the shroom's trigger
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 2f); // optional radius for extra safety
        foreach (Collider2D hit in hits)
        {
            Damage playerHealth = hit.GetComponent<Damage>();
            PlayerController playerController = hit.GetComponent<PlayerController>();
            if (playerHealth != null && playerController != null)
            {
                if (!playerController.IsShieldActive())
                {
                    playerHealth.TakeDamage(damage);
                    Debug.Log("Player hit by shroom explosion!");
                }
            }
        }

        Destroy(gameObject, 0.5f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 2f);
    }
}
