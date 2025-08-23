using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Damage : MonoBehaviour
{
    public static Damage Instance;
    public static event Action OnPlayerDeath;
    public int maxHealth = 100;
    public int health;
    public Slider healthBar; // Reference to a UI Slider for health display

    private Animator animator;
    public AudioSource backgroundMusic;
    public AudioSource audioSource;
    public AudioClip deathClip, damageClip;
    public int crystalCount = 0;
    public TMP_Text crystalCountText;
    private EntityVFX entityVFX;
    private PlayerController playerController;
    private bool isDead = false;
    void Awake()
    {
        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
        entityVFX = GetComponent<EntityVFX>();
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject); // prevent duplicates
        }
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
        audioSource.PlayOneShot(damageClip);
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
        OnPlayerDeath?.Invoke();

        // Stop background music
        if (backgroundMusic != null)
            backgroundMusic.Stop();

        audioSource.PlayOneShot(deathClip);

        if (playerController != null)
        {
            playerController.enabled = false;
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = Vector2.zero;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        //collectibles
        if (collision.CompareTag("crystal") && collision.gameObject.activeSelf)
        {

            crystalCount += 1;
            crystalCountText.text = crystalCount + "/40";
        }
        if (collision.CompareTag("HPCrystal"))
        {

            int healAmount = 20;
            health += healAmount;

            if (health > maxHealth)
                health = maxHealth;

            healthBar.value = health;
        }
    }
}