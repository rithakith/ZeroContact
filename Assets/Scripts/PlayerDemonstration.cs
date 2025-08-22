using UnityEngine;
using System.Collections;

public class PlayerDemonstration : MonoBehaviour
{
    [Header("Visual References")]
    public GameObject shieldVisual;
    public ParticleSystem shieldActivateEffect;
    public Material physicalShieldMat;
    public Material fireShieldMat;
    public Material electricShieldMat;
    
    [Header("Animation Settings")]
    public float moveSpeed = 3f;
    public float jumpHeight = 2f;
    public float jumpForce = 10f;
    public float sprintMultiplier = 2f;
    public float dodgeDistance = 3f;
    public float dodgeSpeed = 10f;
    public float demoMoveDistance = 3f;
    
    [Header("Shield Colors")]
    public Color deflectColor = new Color(0.3f, 0.5f, 1f, 0.7f);
    public Color absorbColor = new Color(0.2f, 1f, 0.3f, 0.7f);
    public Color bypassColor = new Color(1f, 1f, 1f, 0.3f);
    
    [Header("Animation")]
    public Animator animator;
    public string jumpAnimParam = "isJumping";
    public string moveAnimParam = "isMoving";
    public string shieldAnimParam = "isShielding";
    
    private Vector3 startPosition;
    private Quaternion startRotation;
    private Coroutine currentDemo;
    private Renderer shieldRenderer;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private bool isGrounded = true;
    
    void Start()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;
        
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 2f;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
        
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (shieldVisual != null)
        {
            shieldRenderer = shieldVisual.GetComponent<Renderer>();
            if (shieldRenderer == null)
            {
                shieldRenderer = shieldVisual.GetComponent<SpriteRenderer>();
            }
            CreateShieldMaterials();
            shieldVisual.SetActive(false);
        }
        
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<BoxCollider2D>();
        }
    }
    
    void CreateShieldMaterials()
    {
        // Create materials if not assigned
        if (physicalShieldMat == null)
        {
            physicalShieldMat = new Material(Shader.Find("Standard"));
            physicalShieldMat.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            SetMaterialTransparent(physicalShieldMat);
        }
        
        if (fireShieldMat == null)
        {
            fireShieldMat = new Material(Shader.Find("Standard"));
            fireShieldMat.color = new Color(1f, 0.3f, 0f, 0.5f);
            SetMaterialTransparent(fireShieldMat);
        }
        
        if (electricShieldMat == null)
        {
            electricShieldMat = new Material(Shader.Find("Standard"));
            electricShieldMat.color = new Color(0f, 0.5f, 1f, 0.5f);
            SetMaterialTransparent(electricShieldMat);
        }
    }
    
    void SetMaterialTransparent(Material mat)
    {
        mat.SetFloat("_Mode", 3);
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 3000;
    }
    
    public void DemonstrateAction(ControlsTutorial.PlayerAction action, float duration)
    {
        if (currentDemo != null)
            StopCoroutine(currentDemo);
            
        currentDemo = StartCoroutine(DemoCoroutine(action, duration));
    }
    
    IEnumerator DemoCoroutine(ControlsTutorial.PlayerAction action, float duration)
    {
        // Reset position
        transform.position = startPosition;
        transform.rotation = startRotation;
        
        switch (action)
        {
            case ControlsTutorial.PlayerAction.MoveForward:
                yield return DemoMovement(Vector3.forward, duration);
                break;
                
            case ControlsTutorial.PlayerAction.MoveLeft:
                yield return DemoMovement(Vector3.left, duration);
                break;
                
            case ControlsTutorial.PlayerAction.MoveRight:
                yield return DemoMovement(Vector3.right, duration);
                break;
                
            case ControlsTutorial.PlayerAction.MoveBackward:
                yield return DemoMovement(Vector3.back, duration);
                break;
                
            case ControlsTutorial.PlayerAction.Jump:
                yield return DemoJump(duration);
                break;
                
            case ControlsTutorial.PlayerAction.Sprint:
                yield return DemoSprint(duration);
                break;
                
            case ControlsTutorial.PlayerAction.ActivateShield:
                yield return DemoShieldToggle(duration);
                break;
                
            case ControlsTutorial.PlayerAction.ShieldModePhysical:
                yield return DemoShieldMode(physicalShieldMat, duration);
                break;
                
            case ControlsTutorial.PlayerAction.ShieldModeFire:
                yield return DemoShieldMode(fireShieldMat, duration);
                break;
                
            case ControlsTutorial.PlayerAction.ShieldModeElectric:
                yield return DemoShieldMode(electricShieldMat, duration);
                break;
                
            case ControlsTutorial.PlayerAction.Deflect:
                yield return DemoDeflect(duration);
                break;
                
            case ControlsTutorial.PlayerAction.Bypass:
                yield return DemoBypass(duration);
                break;
                
            case ControlsTutorial.PlayerAction.DodgeRoll:
                yield return DemoDodgeRoll(duration);
                break;
                
            case ControlsTutorial.PlayerAction.ComboDeflect:
                yield return DemoCombatSequence(duration);
                break;
                
            default:
                yield return new WaitForSeconds(duration);
                break;
        }
    }
    
    IEnumerator DemoMovement(Vector3 direction, float duration)
    {
        float elapsed = 0;
        while (elapsed < duration)
        {
            transform.position += direction * moveSpeed * Time.deltaTime;
            
            // Loop back if too far
            if (Vector3.Distance(transform.position, startPosition) > 5f)
            {
                transform.position = startPosition;
            }
            
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
    
    IEnumerator DemoJump(float duration)
    {
        int jumps = Mathf.FloorToInt(duration / 1.5f);
        
        for (int i = 0; i < jumps; i++)
        {
            // Jump up
            float jumpTime = 0.5f;
            float elapsed = 0;
            Vector3 startPos = transform.position;
            
            while (elapsed < jumpTime)
            {
                float t = elapsed / jumpTime;
                float height = Mathf.Sin(t * Mathf.PI) * jumpHeight;
                transform.position = new Vector3(startPos.x, startPos.y + height, startPos.z);
                
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            transform.position = startPos;
            yield return new WaitForSeconds(0.5f);
        }
    }
    
    IEnumerator DemoSprint(float duration)
    {
        // Move forward quickly
        float elapsed = 0;
        while (elapsed < duration)
        {
            transform.position += Vector3.forward * moveSpeed * sprintMultiplier * Time.deltaTime;
            
            // Loop back
            if (transform.position.z > startPosition.z + 5f)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, startPosition.z - 2f);
            }
            
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
    
    IEnumerator DemoShieldToggle(float duration)
    {
        float toggleInterval = 1f;
        float elapsed = 0;
        
        while (elapsed < duration)
        {
            // Toggle shield
            if (shieldVisual != null)
            {
                shieldVisual.SetActive(!shieldVisual.activeSelf);
                
                // Play effect
                if (shieldActivateEffect != null && shieldVisual.activeSelf)
                {
                    shieldActivateEffect.Play();
                }
            }
            
            yield return new WaitForSeconds(toggleInterval);
            elapsed += toggleInterval;
        }
        
        // Ensure shield is off at end
        if (shieldVisual != null)
            shieldVisual.SetActive(false);
    }
    
    IEnumerator DemoShieldMode(Material shieldMat, float duration)
    {
        // Activate shield with specific material
        if (shieldVisual != null)
        {
            shieldVisual.SetActive(true);
            
            if (shieldRenderer != null && shieldMat != null)
            {
                shieldRenderer.material = shieldMat;
            }
            
            // Pulse effect
            float pulseSpeed = 2f;
            float elapsed = 0;
            
            while (elapsed < duration)
            {
                float alpha = Mathf.PingPong(elapsed * pulseSpeed, 1f) * 0.3f + 0.2f;
                Color color = shieldRenderer.material.color;
                color.a = alpha;
                shieldRenderer.material.color = color;
                
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            shieldVisual.SetActive(false);
        }
        else
        {
            yield return new WaitForSeconds(duration);
        }
    }
    
    IEnumerator DemoDeflect(float duration)
    {
        if (shieldVisual != null)
        {
            shieldVisual.SetActive(true);
        }
        
        float deflectInterval = 1.5f;
        float elapsed = 0;
        
        while (elapsed < duration)
        {
            // Quick scale animation
            yield return ScaleShield(1f, 1.5f, 0.2f);
            yield return ScaleShield(1.5f, 1f, 0.3f);
            
            yield return new WaitForSeconds(deflectInterval - 0.5f);
            elapsed += deflectInterval;
        }
        
        if (shieldVisual != null)
            shieldVisual.SetActive(false);
    }
    
    IEnumerator DemoBypass(float duration)
    {
        if (shieldVisual != null)
        {
            shieldVisual.SetActive(true);
            
            // Make shield very transparent
            if (shieldRenderer != null)
            {
                Color color = shieldRenderer.material.color;
                color.a = 0.1f;
                shieldRenderer.material.color = color;
            }
            
            yield return new WaitForSeconds(duration);
            
            shieldVisual.SetActive(false);
        }
        else
        {
            yield return new WaitForSeconds(duration);
        }
    }
    
    IEnumerator DemoDodgeRoll(float duration)
    {
        int rolls = Mathf.FloorToInt(duration / 1f);
        
        for (int i = 0; i < rolls; i++)
        {
            // Roll to the right
            Vector3 startPos = transform.position;
            Vector3 endPos = startPos + Vector3.right * dodgeDistance;
            
            float rollTime = 0.5f;
            float elapsed = 0;
            
            while (elapsed < rollTime)
            {
                float t = elapsed / rollTime;
                transform.position = Vector3.Lerp(startPos, endPos, t);
                
                // Rotate during roll
                transform.rotation = Quaternion.Euler(0, 0, t * 360f);
                
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            transform.rotation = startRotation;
            yield return new WaitForSeconds(0.5f);
            
            // Return to center
            transform.position = startPosition;
        }
    }
    
    IEnumerator DemoCombatSequence(float duration)
    {
        // Demonstrate a combat sequence
        float elapsed = 0;
        
        while (elapsed < duration)
        {
            // Move and activate shield
            yield return DemoMovement(Vector3.right, 1f);
            yield return DemoShieldMode(fireShieldMat, 1f);
            yield return DemoDeflect(1f);
            yield return DemoDodgeRoll(1f);
            
            elapsed += 4f;
        }
    }
    
    IEnumerator ScaleShield(float fromScale, float toScale, float time)
    {
        if (shieldVisual == null) yield break;
        
        float elapsed = 0;
        while (elapsed < time)
        {
            float t = elapsed / time;
            float scale = Mathf.Lerp(fromScale, toScale, t);
            shieldVisual.transform.localScale = Vector3.one * scale * 2f;
            
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
    
    // New simplified demo methods for tutorial
    public void DemoJump()
    {
        if (isGrounded && rb != null)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            if (animator != null)
            {
                animator.SetBool(jumpAnimParam, true);
            }
            StartCoroutine(ResetJumpAnimation());
        }
    }
    
    IEnumerator ResetJumpAnimation()
    {
        yield return new WaitForSeconds(0.5f);
        if (animator != null)
        {
            animator.SetBool(jumpAnimParam, false);
        }
    }
    
    public void DemoMoveLeftRight()
    {
        if (currentDemo != null)
            StopCoroutine(currentDemo);
        currentDemo = StartCoroutine(MoveLeftRightSequence());
    }
    
    IEnumerator MoveLeftRightSequence()
    {
        if (animator != null)
        {
            animator.SetBool(moveAnimParam, true);
        }
        
        yield return MoveToPosition(startPosition + Vector3.left * demoMoveDistance, 1f);
        yield return new WaitForSeconds(0.2f);
        
        yield return MoveToPosition(startPosition + Vector3.right * demoMoveDistance, 2f);
        yield return new WaitForSeconds(0.2f);
        
        yield return MoveToPosition(startPosition, 1f);
        
        if (animator != null)
        {
            animator.SetBool(moveAnimParam, false);
        }
    }
    
    IEnumerator MoveToPosition(Vector3 targetPos, float duration)
    {
        Vector3 startPos = transform.position;
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, elapsedTime / duration);
            
            if (targetPos.x > transform.position.x)
                transform.localScale = new Vector3(1, 1, 1);
            else if (targetPos.x < transform.position.x)
                transform.localScale = new Vector3(-1, 1, 1);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        transform.position = targetPos;
    }
    
    public void DemoShieldDeflect()
    {
        if (currentDemo != null)
            StopCoroutine(currentDemo);
        currentDemo = StartCoroutine(ShieldDeflectSequence());
    }
    
    IEnumerator ShieldDeflectSequence()
    {
        ShowShield(deflectColor);
        
        if (animator != null)
        {
            animator.SetTrigger("Deflect");
        }
        
        yield return ScaleShield(1f, 1.5f, 0.2f);
        yield return ScaleShield(1.5f, 1f, 0.3f);
        
        yield return new WaitForSeconds(0.5f);
        
        HideShield();
    }
    
    public void DemoShieldAbsorb()
    {
        if (currentDemo != null)
            StopCoroutine(currentDemo);
        currentDemo = StartCoroutine(ShieldAbsorbSequence());
    }
    
    IEnumerator ShieldAbsorbSequence()
    {
        ShowShield(absorbColor);
        
        if (animator != null)
        {
            animator.SetBool(shieldAnimParam, true);
        }
        
        // Pulse effect
        float pulseTime = 2f;
        float elapsed = 0f;
        
        while (elapsed < pulseTime)
        {
            if (shieldRenderer != null)
            {
                float alpha = Mathf.PingPong(elapsed * 2f, 1f) * 0.3f + 0.4f;
                Color color = absorbColor;
                color.a = alpha;
                SetShieldColor(color);
            }
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        if (animator != null)
        {
            animator.SetBool(shieldAnimParam, false);
        }
        
        HideShield();
    }
    
    public void DemoShieldBypass()
    {
        if (currentDemo != null)
            StopCoroutine(currentDemo);
        currentDemo = StartCoroutine(ShieldBypassSequence());
    }
    
    IEnumerator ShieldBypassSequence()
    {
        ShowShield(bypassColor);
        
        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            Color fadeColor = originalColor;
            fadeColor.a = 0.5f;
            spriteRenderer.color = fadeColor;
            
            yield return new WaitForSeconds(1f);
            
            spriteRenderer.color = originalColor;
        }
        
        HideShield();
    }
    
    void ShowShield(Color color)
    {
        if (shieldVisual != null)
        {
            shieldVisual.SetActive(true);
            SetShieldColor(color);
            
            if (shieldActivateEffect != null)
            {
                shieldActivateEffect.Play();
            }
        }
    }
    
    void HideShield()
    {
        if (shieldVisual != null)
        {
            shieldVisual.SetActive(false);
        }
    }
    
    void SetShieldColor(Color color)
    {
        if (shieldRenderer != null)
        {
            if (shieldRenderer is SpriteRenderer spriteRend)
            {
                spriteRend.color = color;
            }
            else if (shieldRenderer.material != null)
            {
                shieldRenderer.material.color = color;
            }
        }
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check for ground by multiple methods
        bool isGroundLayer = false;
        int groundLayer = LayerMask.NameToLayer("Ground");
        if (groundLayer != -1)
        {
            isGroundLayer = collision.gameObject.layer == groundLayer;
        }
        
        bool isGroundTag = false;
        try
        {
            isGroundTag = collision.gameObject.CompareTag("Ground");
        }
        catch (UnityException)
        {
            // Tag doesn't exist, ignore
        }
        bool isGroundByNormal = collision.contacts.Length > 0 && collision.contacts[0].normal.y > 0.5f;
        
        if (isGroundLayer || isGroundTag || isGroundByNormal)
        {
            isGrounded = true;
        }
    }
    
    void OnCollisionExit2D(Collision2D collision)
    {
        // Check for ground by multiple methods
        bool isGroundLayer = false;
        int groundLayer = LayerMask.NameToLayer("Ground");
        if (groundLayer != -1)
        {
            isGroundLayer = collision.gameObject.layer == groundLayer;
        }
        
        bool isGroundTag = false;
        try
        {
            isGroundTag = collision.gameObject.CompareTag("Ground");
        }
        catch (UnityException)
        {
            // Tag doesn't exist, ignore
        }
        
        if (isGroundLayer || isGroundTag)
        {
            isGrounded = false;
        }
    }
}