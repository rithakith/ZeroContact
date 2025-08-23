using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public class DisableButtonKeyboardNavigation : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("Tools/Disable Space Key Button Navigation")]
    public static void DisableSpaceNavigation()
    {
        // Find all buttons in the scene
        Button[] allButtons = GameObject.FindObjectsOfType<Button>(true);
        int fixedCount = 0;
        
        foreach (Button button in allButtons)
        {
            // Disable keyboard navigation on the button
            Navigation nav = button.navigation;
            nav.mode = Navigation.Mode.None;
            button.navigation = nav;
            
            fixedCount++;
            Debug.Log($"Disabled keyboard navigation on button: {button.name}");
        }
        
        // Also find and update the EventSystem to ensure Space doesn't trigger submit
        UnityEngine.EventSystems.EventSystem eventSystem = GameObject.FindObjectOfType<UnityEngine.EventSystems.EventSystem>();
        if (eventSystem != null)
        {
            // Add a component to intercept submit events
            ButtonSubmitBlocker blocker = eventSystem.GetComponent<ButtonSubmitBlocker>();
            if (blocker == null)
            {
                blocker = eventSystem.gameObject.AddComponent<ButtonSubmitBlocker>();
                Debug.Log("Added ButtonSubmitBlocker to prevent Space key from triggering buttons");
            }
        }
        
        Debug.Log($"Fixed {fixedCount} buttons. Space key will no longer trigger button clicks.");
        Debug.Log("Buttons can still be clicked with mouse, but not activated with keyboard.");
        
        // Mark scene as dirty
        EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
    }
#endif
}

// Component to block Space key from triggering button submit
public class ButtonSubmitBlocker : MonoBehaviour
{
    void Update()
    {
        // Intercept Space key to prevent it from triggering UI submit
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Consume the Space key event for UI
            UnityEngine.EventSystems.EventSystem current = UnityEngine.EventSystems.EventSystem.current;
            if (current != null && current.currentSelectedGameObject != null)
            {
                // Deselect any selected UI element when Space is pressed
                current.SetSelectedGameObject(null);
            }
        }
    }
    
    void Start()
    {
        // Ensure no button is selected by default
        UnityEngine.EventSystems.EventSystem current = UnityEngine.EventSystems.EventSystem.current;
        if (current != null)
        {
            current.SetSelectedGameObject(null);
        }
    }
}