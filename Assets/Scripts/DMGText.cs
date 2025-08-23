using UnityEngine;

public class DMGText : MonoBehaviour
{
    public float lifetime = 1.5f; // Lifetime of the damage text
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, lifetime); // Destroy the damage text after the specified lifetime
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
