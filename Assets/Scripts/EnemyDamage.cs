using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    [Header("Damage Settings")]
    public int damage = 10;          // Amount of damage
    public bool instantKill = false; // If true, kills player instantly
    public bool continuousDamage = false; // If true, damage is applied while player stays inside trigger
    public float damageInterval = 1f;     // Time between damage ticks (if continuous)

    private float damageTimer = 0f;
    private Damage playerHealth;

    private void Update()
    {
        if (continuousDamage && damageTimer > 0f)
            damageTimer -= Time.deltaTime;
    }

    private void ApplyDamage(GameObject player)
    {
        Damage playerHealth = player.GetComponent<Damage>();
        if (playerHealth == null) return;

        if (instantKill)
        {
            playerHealth.TakeDamage(playerHealth.health); // Reduce all health
        }
        else
        {
            playerHealth.TakeDamage(damage);
        }
    }

    // Trigger damage once on collision
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        if (!continuousDamage)
        {
            ApplyDamage(collision.gameObject);
        }
    }

    // Trigger damage while player stays in trigger area
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player") || !continuousDamage) return;

        if (damageTimer <= 0f)
        {
            ApplyDamage(collision.gameObject);
            damageTimer = damageInterval; // reset timer
        }
    }

    // Optional: single-use trigger (like spikes)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        if (!continuousDamage)
        {
            ApplyDamage(collision.gameObject);
        }
    }
    // public int damage = 10; // Amount of damage to apply
    // private Damage playerHealth;
    // // Start is called once before the first execution of Update after the MonoBehaviour is created
    // void Start()
    // {

    // }

    // // Update is called once per frame
    // void Update()
    // {

    // }

    // private void OnCollisionEnter2D(Collision2D collision)
    // {
    //     if (collision.gameObject.CompareTag("Player"))
    //     {
    //         if (playerHealth == null)
    //         {
    //             playerHealth = collision.gameObject.GetComponent<Damage>();
    //         }
    //         playerHealth.TakeDamage(damage);
    //     }
    // }
}
