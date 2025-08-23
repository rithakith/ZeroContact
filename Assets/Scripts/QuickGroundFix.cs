using UnityEngine;

// This script automatically fixes ground collision when attached to DemoGround
[ExecuteInEditMode]
public class QuickGroundFix : MonoBehaviour
{
    void Start()
    {
        if (Application.isPlaying)
        {
            FixGround();
        }
    }

    void OnValidate()
    {
        if (!Application.isPlaying)
        {
            FixGround();
        }
    }

    void FixGround()
    {
        // Remove 3D collider if present
        Collider col3D = GetComponent<Collider>();
        if (col3D != null)
        {
            Debug.Log("QuickGroundFix: Removing 3D collider");
            if (Application.isPlaying)
                Destroy(col3D);
            else
                DestroyImmediate(col3D);
        }

        // Add 2D collider if missing
        Collider2D col2D = GetComponent<Collider2D>();
        if (col2D == null)
        {
            Debug.Log("QuickGroundFix: Adding BoxCollider2D");
            gameObject.AddComponent<BoxCollider2D>();
        }

        // Ensure proper position
        if (transform.position.y < -1.5f)
        {
            transform.position = new Vector3(transform.position.x, -2f, 0);
        }
    }
}