using UnityEngine;

[RequireComponent(typeof(ShieldController))]
public class ShieldSetupHelper : MonoBehaviour
{
    void Start()
    {
        ShieldController shieldController = GetComponent<ShieldController>();
        
        if (shieldController.shieldVisual == null)
        {
            Debug.Log("Creating shield visual object...");
            
            GameObject shield = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            shield.name = "ShieldVisual";
            shield.transform.SetParent(transform);
            shield.transform.localPosition = Vector3.zero;
            shield.transform.localScale = new Vector3(3f, 3f, 3f);
            
            Renderer renderer = shield.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                mat.SetFloat("_Surface", 1);
                mat.SetFloat("_Blend", 0);
                mat.SetFloat("_AlphaClip", 0);
                mat.SetFloat("_SrcBlend", 5);
                mat.SetFloat("_DstBlend", 10);
                mat.SetFloat("_ZWrite", 0);
                mat.SetFloat("_Cull", 0);
                mat.color = new Color(0f, 0.8f, 1f, 0.3f);
                mat.SetColor("_EmissionColor", new Color(0f, 0.5f, 1f) * 2f);
                mat.EnableKeyword("_EMISSION");
                mat.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                mat.renderQueue = 3000;
                renderer.material = mat;
            }
            
            shield.SetActive(false);
            
            shieldController.shieldVisual = shield;
            
            Debug.Log("Shield visual created and assigned!");
        }
        else
        {
            Debug.Log($"Shield visual already assigned: {shieldController.shieldVisual.name}");
        }
    }
}