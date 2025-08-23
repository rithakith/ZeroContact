using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

[RequireComponent(typeof(Button))]
public class ControlsButtonEnhancer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Hover Effects")]
    public bool enableHoverScale = true;
    public float hoverScale = 1.1f;
    public float clickScale = 0.95f;
    public float scaleSpeed = 10f;
    
    [Header("Position Effects")]
    public bool enableHoverMove = true;
    public float hoverOffsetY = 5f;
    public float clickOffsetY = -2f;
    public float moveSpeed = 10f;
    
    [Header("Color Effects")]
    public bool enableTextColorChange = true;
    public Color normalTextColor = Color.white;
    public Color hoverTextColor = new Color(1f, 1f, 0.8f);
    public float colorSpeed = 8f;
    
    [Header("Audio")]
    public AudioClip hoverSound;
    public AudioClip clickSound;
    public float soundVolume = 0.5f;
    
    private Button button;
    private RectTransform rectTransform;
    private TextMeshProUGUI buttonText;
    private AudioSource audioSource;
    
    private Vector3 originalScale;
    private Vector3 originalPosition;
    private Vector3 targetScale;
    private Vector3 targetPosition;
    private Color targetTextColor;
    
    private bool isHovering = false;
    private bool isPressed = false;
    
    void Start()
    {
        button = GetComponent<Button>();
        rectTransform = GetComponent<RectTransform>();
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
        
        originalScale = transform.localScale;
        originalPosition = rectTransform.anchoredPosition;
        targetScale = originalScale;
        targetPosition = originalPosition;
        
        if (buttonText != null)
        {
            normalTextColor = buttonText.color;
            targetTextColor = normalTextColor;
        }
        
        // Create audio source if needed
        if (hoverSound != null || clickSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.volume = soundVolume;
        }
        
        // Start animation coroutine
        StartCoroutine(AnimateButton());
    }
    
    IEnumerator AnimateButton()
    {
        while (true)
        {
            // Animate scale
            if (enableHoverScale)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * scaleSpeed);
            }
            
            // Animate position
            if (enableHoverMove)
            {
                rectTransform.anchoredPosition = Vector3.Lerp(rectTransform.anchoredPosition, targetPosition, Time.deltaTime * moveSpeed);
            }
            
            // Animate text color
            if (enableTextColorChange && buttonText != null)
            {
                buttonText.color = Color.Lerp(buttonText.color, targetTextColor, Time.deltaTime * colorSpeed);
            }
            
            yield return null;
        }
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!button.interactable) return;
        
        isHovering = true;
        UpdateButtonState();
        
        if (hoverSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hoverSound);
        }
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        isPressed = false;
        UpdateButtonState();
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!button.interactable) return;
        
        isPressed = true;
        UpdateButtonState();
        
        if (clickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }
    
    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        UpdateButtonState();
    }
    
    void UpdateButtonState()
    {
        if (isPressed)
        {
            targetScale = originalScale * clickScale;
            targetPosition = originalPosition + Vector3.up * clickOffsetY;
            targetTextColor = hoverTextColor;
        }
        else if (isHovering)
        {
            targetScale = originalScale * hoverScale;
            targetPosition = originalPosition + Vector3.up * hoverOffsetY;
            targetTextColor = hoverTextColor;
        }
        else
        {
            targetScale = originalScale;
            targetPosition = originalPosition;
            targetTextColor = normalTextColor;
        }
    }
    
    void OnDisable()
    {
        // Reset to original state
        transform.localScale = originalScale;
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = originalPosition;
        }
        if (buttonText != null)
        {
            buttonText.color = normalTextColor;
        }
    }
}