using UnityEngine;
using UnityEngine.UI;

public class Damage : MonoBehaviour
{
    public int maxHealth = 100;
    public int health;
    public Slider healthBar; // Reference to a UI Slider for health display

    void Awake()
    {

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
        health -= damage;
        healthBar.value = health;
        if (health <= 0)
        {
            // Die();
            Destroy(gameObject);
        }
    }
}
