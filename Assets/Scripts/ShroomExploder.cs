using UnityEngine;

public class ShroomExploder : MonoBehaviour
{
    public float explosionDelay = 1.5f;
    public float explosionRadius = 2f;
    public int damage = 2;

    private Animator animator;
    private bool exploding = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !exploding)
        {
            exploding = true;
            animator.SetTrigger("PlayerNear");
            Invoke(nameof(Explode), explosionDelay);
        }
    }

    void Explode()
    {
        animator.SetTrigger("ExplodeNow");

        // Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        // foreach (var hit in colliders)
        // {
        //     if (hit.CompareTag("Player"))
        //     {
        //         // hit.GetComponent<Damage>().Health -= damage;
        //     }
        // }
        Destroy(gameObject, 0.5f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
