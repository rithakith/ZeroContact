using UnityEngine;

public class ShieldController : MonoBehaviour
{
    public GameObject shieldVisual;
    public float shieldDuration = 3f;
    public float shieldCooldown = 5f;
    private bool isShieldActive = false;
    private float shieldTimer;
    private float cooldownTimer;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isShieldActive && cooldownTimer <= 0)
        {
            ActivateShield();
        }

        if (isShieldActive)
        {
            shieldTimer -= Time.deltaTime;
            if (shieldTimer <= 0)
            {
                DeactivateShield();
            }
        }

        if (cooldownTimer > 0)
            cooldownTimer -= Time.deltaTime;
    }

    void ActivateShield()
    {
        Debug.Log("Shield Activated!");
        isShieldActive = true;
        shieldTimer = shieldDuration;
        cooldownTimer = shieldCooldown;
        
        if (shieldVisual != null)
        {
            shieldVisual.SetActive(true);
            shieldVisual.transform.localScale = new Vector3(3f, 3f, 3f);
            shieldVisual.transform.localPosition = Vector3.zero;
            Debug.Log($"Shield Visual activated at position: {shieldVisual.transform.position}");
            Debug.Log($"Shield Visual scale: {shieldVisual.transform.localScale}");
        }
        else
        {
            Debug.LogError("Shield Visual is not assigned in the Inspector!");
        }
    }

    void DeactivateShield()
    {
        isShieldActive = false;
        shieldVisual.SetActive(false);
    }

    public bool IsShieldActive()
    {
        return isShieldActive;
    }
}
