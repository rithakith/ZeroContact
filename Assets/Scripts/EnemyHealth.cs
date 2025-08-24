using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int ehealth = 30;
    private Animator animator;
    private AudioSource audioSource;

    void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    public void EnemyTakeDamage(int dmg)
    {
        ehealth -= dmg;
        Debug.Log(gameObject.name + " took " + dmg + " damage!");

        if (ehealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Play death animation
        animator.SetTrigger(AnimationStrings.EnemyDeath);

        // Disable collisions
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // Hide sprite
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.enabled = false;

        // Play death sound
        if (audioSource != null) audioSource.Play();

        // Optional: destroy after audio finishes
        Destroy(gameObject, audioSource.clip.length);
    }

}
