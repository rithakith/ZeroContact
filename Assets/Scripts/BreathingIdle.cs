using UnityEngine;

public class BreathingIdle : MonoBehaviour
{
    public float scaleAmount = 0.05f; // Breathing intensity
    public float speed = 2f;          // Breathing speed

    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale; // This remembers your desired base size
    }

    void Update()
    {
        float scaleFactor = 1 + Mathf.Sin(Time.time * speed) * scaleAmount;
        transform.localScale = originalScale * scaleFactor;
    }
}
