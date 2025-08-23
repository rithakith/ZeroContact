#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class TagAndLayerSetup : EditorWindow
{
    [MenuItem("Tools/Setup Tags and Layers")]
    static void SetupTagsAndLayers()
    {
        // Add Ground tag
        AddTag("Ground");
        
        // Add Ground layer
        AddLayer("Ground");
        
        Debug.Log("Tags and Layers setup complete!");
        EditorUtility.DisplayDialog("Setup Complete", "Ground tag and layer have been added to the project.", "OK");
    }
    
    static void AddTag(string tagName)
    {
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty tagsProp = tagManager.FindProperty("tags");
        
        // Check if tag already exists
        bool found = false;
        for (int i = 0; i < tagsProp.arraySize; i++)
        {
            SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
            if (t.stringValue.Equals(tagName))
            {
                found = true;
                break;
            }
        }
        
        // Add tag if not found
        if (!found)
        {
            tagsProp.InsertArrayElementAtIndex(0);
            SerializedProperty newTag = tagsProp.GetArrayElementAtIndex(0);
            newTag.stringValue = tagName;
            Debug.Log($"Added tag: {tagName}");
        }
        else
        {
            Debug.Log($"Tag already exists: {tagName}");
        }
        
        tagManager.ApplyModifiedProperties();
    }
    
    static void AddLayer(string layerName)
    {
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty layers = tagManager.FindProperty("layers");
        
        // Find first empty layer slot (starting from 8, as 0-7 are Unity's built-in layers)
        for (int i = 8; i < 32; i++)
        {
            SerializedProperty layerProperty = layers.GetArrayElementAtIndex(i);
            if (string.IsNullOrEmpty(layerProperty.stringValue))
            {
                layerProperty.stringValue = layerName;
                Debug.Log($"Added layer: {layerName} at index {i}");
                tagManager.ApplyModifiedProperties();
                return;
            }
            else if (layerProperty.stringValue == layerName)
            {
                Debug.Log($"Layer already exists: {layerName} at index {i}");
                return;
            }
        }
        
        Debug.LogWarning("No empty layer slots available!");
    }
}
#endif