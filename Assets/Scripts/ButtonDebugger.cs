using UnityEngine;
using UnityEngine.UI;

public class ButtonDebugger : MonoBehaviour
{
    void Start()
    {
        Button button = GetComponent<Button>();
        if (button != null)
        {
            Debug.Log($"ButtonDebugger on {gameObject.name}: Button component found");
            Debug.Log($"Button interactable: {button.interactable}");
            Debug.Log($"Button onClick listener count: {button.onClick.GetPersistentEventCount()}");
            
            // Add a debug listener
            button.onClick.AddListener(() => {
                Debug.Log($"BUTTON CLICKED: {gameObject.name}");
            });
        }
        else
        {
            Debug.LogError($"ButtonDebugger on {gameObject.name}: No Button component found!");
        }
    }
    
    void OnEnable()
    {
        Debug.Log($"Button {gameObject.name} enabled");
    }
    
    void OnDisable()
    {
        Debug.Log($"Button {gameObject.name} disabled");
    }
}