using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int ehealth = 30;
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
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
        animator.SetTrigger(AnimationStrings.EnemyDeath);
        Destroy(gameObject, 0.5f); // Delay to allow death animation to play
    }
}
