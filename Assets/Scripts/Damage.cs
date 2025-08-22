using UnityEngine;
using UnityEngine.UI;

public class Damage : MonoBehaviour
{
    public int maxHealth = 100;
    public int health;
    public Slider healthBar; // Reference to a UI Slider for health display

    private Animator animator;
    private EntityVFX entityVFX;
    private PlayerController playerController;
    private bool isDead = false;
    void Awake()
    {
        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
        entityVFX = GetComponent<EntityVFX>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = maxHealth;
        healthBar.maxValue = maxHealth;
        healthBar.value = health;
    }



    // Update is called once per frame
    void Update()
    {

    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        health -= damage;
        entityVFX.TriggerOnDamageVFX();
        healthBar.value = health;
        if (health <= 0)
        {
            Die();

        }
    }

    private void Die()
    {
        isDead = true;
        animator.SetTrigger(AnimationStrings.Death);
        playerController.enabled = false; // Disable player controls
        if (playerController != null)
        {
            playerController.enabled = false;
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = Vector2.zero;
        }


        // // Destroy after animation finishes
        // float deathAnimLength = animator.GetCurrentAnimatorStateInfo(0).length;
        // Destroy(gameObject, deathAnimLength);
    }
}
