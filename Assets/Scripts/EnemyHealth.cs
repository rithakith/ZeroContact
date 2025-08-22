using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int ehealth = 30;  

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
        Destroy(gameObject);
    }
}
