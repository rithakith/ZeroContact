using UnityEngine;

[RequireComponent(typeof(ShieldController))]
public class Shield2DSetup : MonoBehaviour
{
    void Start()
    {
        ShieldController shieldController = GetComponent<ShieldController>();
        
        if (shieldController.shieldVisual == null)
        {
            Debug.Log("Creating 2D shield visual...");
            
            GameObject shield = new GameObject("Shield2D");
            shield.transform.SetParent(transform);
            shield.transform.localPosition = Vector3.zero;
            
            SpriteRenderer spriteRenderer = shield.AddComponent<SpriteRenderer>();
            
            Texture2D texture = new Texture2D(128, 128);
            Color[] pixels = new Color[128 * 128];
            
            Vector2 center = new Vector2(64, 64);
            float radius = 60f;
            
            for (int y = 0; y < 128; y++)
            {
                for (int x = 0; x < 128; x++)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), center);
                    
                    if (distance < radius)
                    {
                        float alpha = 1f - (distance / radius);
                        pixels[y * 128 + x] = new Color(0f, 0.5f, 1f, alpha * 0.5f);
                    }
                    else
                    {
                        pixels[y * 128 + x] = Color.clear;
                    }
                }
            }
            
            texture.SetPixels(pixels);
            texture.Apply();
            
            Sprite shieldSprite = Sprite.Create(texture, new Rect(0, 0, 128, 128), new Vector2(0.5f, 0.5f), 100f);
            spriteRenderer.sprite = shieldSprite;
            spriteRenderer.sortingOrder = 100;
            
            shield.transform.localScale = new Vector3(2f, 2f, 1f);
            
            shield.SetActive(false);
            
            shieldController.shieldVisual = shield;
            
            Debug.Log("2D Shield visual created!");
        }
    }
}