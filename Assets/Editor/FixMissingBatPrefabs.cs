using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class FixMissingBatPrefabs : EditorWindow
{
    private GameObject replacementPrefab;
    private List<GameObject> missingPrefabInstances = new List<GameObject>();
    
    [MenuItem("Tools/Fix Missing Bat Prefabs")]
    public static void ShowWindow()
    {
        GetWindow<FixMissingBatPrefabs>("Fix Missing Bat Prefabs");
    }
    
    void OnGUI()
    {
        GUILayout.Label("Fix Missing Bat Prefab References", EditorStyles.boldLabel);
        GUILayout.Space(10);
        
        // Prefab selection
        GUILayout.Label("Step 1: Select the Bat Prefab to use as replacement:");
        replacementPrefab = (GameObject)EditorGUILayout.ObjectField("Bat Prefab", replacementPrefab, typeof(GameObject), false);
        
        GUILayout.Space(10);
        
        // Find missing prefabs button
        if (GUILayout.Button("Step 2: Find All Missing Bat Prefabs in Scene"))
        {
            FindMissingBatPrefabs();
        }
        
        // Display found instances
        if (missingPrefabInstances.Count > 0)
        {
            GUILayout.Space(10);
            GUILayout.Label($"Found {missingPrefabInstances.Count} missing Bat prefab instances", EditorStyles.helpBox);
            
            GUILayout.Space(10);
            
            // Replace button
            GUI.enabled = replacementPrefab != null;
            if (GUILayout.Button("Step 3: Replace All Missing Prefabs", GUILayout.Height(30)))
            {
                ReplaceMissingPrefabs();
            }
            GUI.enabled = true;
            
            if (replacementPrefab == null)
            {
                EditorGUILayout.HelpBox("Please select a Bat prefab before replacing", MessageType.Warning);
            }
        }
    }
    
    void FindMissingBatPrefabs()
    {
        missingPrefabInstances.Clear();
        
        // Find all GameObjects in the scene
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        
        foreach (GameObject obj in allObjects)
        {
            // Check if it's a missing prefab instance with "Bat" in the name
            if (PrefabUtility.GetPrefabInstanceStatus(obj) == PrefabInstanceStatus.MissingAsset &&
                obj.name.Contains("Bat"))
            {
                missingPrefabInstances.Add(obj);
            }
        }
        
        Debug.Log($"Found {missingPrefabInstances.Count} missing Bat prefab instances");
    }
    
    void ReplaceMissingPrefabs()
    {
        if (replacementPrefab == null)
        {
            Debug.LogError("No replacement prefab selected!");
            return;
        }
        
        Undo.RecordObjects(missingPrefabInstances.ToArray(), "Replace Missing Bat Prefabs");
        
        int replacedCount = 0;
        List<GameObject> newInstances = new List<GameObject>();
        
        foreach (GameObject missingInstance in missingPrefabInstances)
        {
            if (missingInstance == null) continue;
            
            // Store the transform data
            Vector3 position = missingInstance.transform.position;
            Quaternion rotation = missingInstance.transform.rotation;
            Vector3 scale = missingInstance.transform.localScale;
            Transform parent = missingInstance.transform.parent;
            int siblingIndex = missingInstance.transform.GetSiblingIndex();
            string name = missingInstance.name;
            
            // Create new instance
            GameObject newInstance = (GameObject)PrefabUtility.InstantiatePrefab(replacementPrefab);
            newInstance.transform.position = position;
            newInstance.transform.rotation = rotation;
            newInstance.transform.localScale = scale;
            newInstance.transform.SetParent(parent);
            newInstance.transform.SetSiblingIndex(siblingIndex);
            newInstance.name = name;
            
            // Register the new instance for undo
            Undo.RegisterCreatedObjectUndo(newInstance, "Replace Missing Bat Prefab");
            
            newInstances.Add(newInstance);
            
            // Destroy the missing instance
            Undo.DestroyObjectImmediate(missingInstance);
            
            replacedCount++;
        }
        
        Debug.Log($"Successfully replaced {replacedCount} missing Bat prefab instances");
        
        // Clear the list and refresh
        missingPrefabInstances = newInstances;
        EditorUtility.SetDirty(this);
    }
}