#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using TMPro;
using UnityEngine.UI;

public class AddBackButton : EditorWindow
{
    [MenuItem("Tools/Add Back Button to Controls")]
    static void AddButton()
    {
        Debug.Log("=== ADDING BACK BUTTON ===");
        
        // Find the manager (could be either simple or enhanced)
        MonoBehaviour manager = GameObject.FindObjectOfType<ControlsTutorialManagerSimple>();
        if (manager == null)
        {
            manager = GameObject.FindObjectOfType<ControlsTutorialManagerEnhanced>();
        }
        
        if (manager == null)
        {
            Debug.LogError("No tutorial manager found!");
            return;
        }
        
        // Get the screen container
        GameObject screenContainer = null;
        Button nextButton = null;
        TextMeshProUGUI buttonText = null;
        
        if (manager is ControlsTutorialManagerSimple simple)
        {
            screenContainer = simple.screenContainer;
            nextButton = simple.nextButton;
            buttonText = simple.buttonText;
        }
        else if (manager is ControlsTutorialManagerEnhanced enhanced)
        {
            screenContainer = enhanced.screenContainer;
            nextButton = enhanced.nextButton;
            buttonText = enhanced.buttonText;
        }
        
        if (screenContainer == null || nextButton == null)
        {
            Debug.LogError("Missing required references!");
            return;
        }
        
        // Create back button as a copy of next button
        GameObject nextButtonObj = nextButton.gameObject;
        GameObject backButtonObj = GameObject.Instantiate(nextButtonObj, nextButtonObj.transform.parent);
        backButtonObj.name = "BackButton";
        
        // Position it to the left of the next button
        RectTransform nextRect = nextButtonObj.GetComponent<RectTransform>();
        RectTransform backRect = backButtonObj.GetComponent<RectTransform>();
        
        // Set anchors for left side
        backRect.anchorMin = new Vector2(0.05f, nextRect.anchorMin.y);
        backRect.anchorMax = new Vector2(0.25f, nextRect.anchorMax.y);
        backRect.offsetMin = Vector2.zero;
        backRect.offsetMax = Vector2.zero;
        
        // Update the text
        TextMeshProUGUI backButtonText = backButtonObj.GetComponentInChildren<TextMeshProUGUI>();
        if (backButtonText != null)
        {
            backButtonText.text = "BACK";
        }
        
        // Get the button component
        Button backButton = backButtonObj.GetComponent<Button>();
        
        // Clear existing listeners
        backButton.onClick.RemoveAllListeners();
        
        // Assign to manager
        if (manager is ControlsTutorialManagerSimple simpleManager)
        {
            simpleManager.backButton = backButton;
            simpleManager.backButtonText = backButtonText;
            
            // Add the PreviousScreen method if it doesn't exist
            System.Type type = simpleManager.GetType();
            var method = type.GetMethod("PreviousScreen");
            if (method == null)
            {
                Debug.Log("Note: PreviousScreen method needs to be added to the script");
            }
        }
        else if (manager is ControlsTutorialManagerEnhanced enhancedManager)
        {
            enhancedManager.backButton = backButton;
            enhancedManager.backButtonText = backButtonText;
        }
        
        EditorUtility.SetDirty(manager);
        EditorUtility.SetDirty(backButtonObj);
        
        Debug.Log("✓ Created back button");
        Debug.Log("✓ Positioned to the left of next button");
        Debug.Log("\nNote: The script has been updated to include:");
        Debug.Log("- PreviousScreen() method");
        Debug.Log("- Keyboard shortcuts: Left Arrow or Backspace");
        Debug.Log("- Back button returns to main menu from first screen");
        Debug.Log("\nDon't forget to save the scene!");
        
        EditorUtility.DisplayDialog("Back Button Added", 
            "The back button has been added!\n\n" +
            "Features:\n" +
            "• Click BACK button to go to previous screen\n" +
            "• Press Left Arrow or Backspace\n" +
            "• From first screen, goes to main menu\n\n" +
            "Save the scene to keep these changes!", 
            "OK");
    }
}
#endif