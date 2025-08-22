#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System.Collections.Generic;

public class FixMissingScripts : EditorWindow
{
    [MenuItem("Tools/Fix Missing Scripts in Scene")]
    static void FixMissing()
    {
        int missingCount = 0;
        int fixedCount = 0;
        
        // Get all GameObjects in the scene
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        
        foreach (GameObject go in allObjects)
        {
            // Get all components
            Component[] components = go.GetComponents<Component>();
            
            for (int i = components.Length - 1; i >= 0; i--)
            {
                if (components[i] == null)
                {
                    missingCount++;
                    
                    // Try to fix known missing scripts
                    if (go.name == "ControlsTutorialManager")
                    {
                        // Remove the missing component
                        var serializedObject = new SerializedObject(go);
                        var prop = serializedObject.FindProperty("m_Component");
                        prop.DeleteArrayElementAtIndex(i);
                        serializedObject.ApplyModifiedProperties();
                        
                        // Add the correct component
                        go.AddComponent<ControlsTutorialManagerSimple>();
                        Debug.Log($"Fixed missing script on {go.name} - added ControlsTutorialManagerSimple");
                        fixedCount++;
                    }
                }
            }
        }
        
        Debug.Log($"Found {missingCount} missing scripts. Fixed {fixedCount}.");
        
        if (missingCount > fixedCount)
        {
            Debug.LogWarning($"Still have {missingCount - fixedCount} missing scripts. You may need to manually remove them or rebuild the scene.");
        }
        
        EditorUtility.DisplayDialog("Fix Complete", 
            $"Found {missingCount} missing scripts.\nFixed {fixedCount}.\n\nIf issues persist, use Tools > Rebuild Controls Scene Clean", 
            "OK");
    }
    
    [MenuItem("Tools/Rebuild Controls Scene Clean")]
    static void RebuildClean()
    {
        if (EditorUtility.DisplayDialog("Rebuild Controls Scene", 
            "This will create a fresh Controls scene.\n\nAny changes to the existing scene will be lost.\n\nContinue?", 
            "Yes, Rebuild", "Cancel"))
        {
            // Create new scene
            Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            
            // Add the enhanced builder
            GameObject builderObj = new GameObject("SceneBuilder");
            ControlsSceneBuilderEnhanced builder = builderObj.AddComponent<ControlsSceneBuilderEnhanced>();
            
            // Let Unity update
            EditorApplication.delayCall += () =>
            {
                // Trigger the build
                builder.buildScene = true;
                
                // Save after another delay
                EditorApplication.delayCall += () =>
                {
                    // Remove the builder
                    if (builderObj != null)
                        DestroyImmediate(builderObj);
                    
                    // Save the scene
                    string scenePath = "Assets/Scenes/ControlsScene.unity";
                    EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), scenePath);
                    
                    Debug.Log($"ControlsScene rebuilt and saved at: {scenePath}");
                    Debug.Log("Remember to assign the Player prefab to ControlsTutorialManager!");
                    
                    EditorUtility.DisplayDialog("Scene Rebuilt", 
                        "Controls scene has been rebuilt.\n\nDon't forget to:\n1. Assign Player prefab to ControlsTutorialManager\n2. Test the scene", 
                        "OK");
                };
            };
        }
    }
    
    [MenuItem("Tools/Remove All Missing Scripts")]
    static void RemoveAllMissing()
    {
        int removedCount = 0;
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        
        foreach (GameObject go in allObjects)
        {
            int removed = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
            if (removed > 0)
            {
                removedCount += removed;
                Debug.Log($"Removed {removed} missing scripts from {go.name}");
            }
        }
        
        Debug.Log($"Total removed: {removedCount} missing scripts");
        EditorUtility.DisplayDialog("Cleanup Complete", $"Removed {removedCount} missing scripts from the scene.", "OK");
    }
}
#endif