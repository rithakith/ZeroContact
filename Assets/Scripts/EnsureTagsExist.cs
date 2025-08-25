using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[InitializeOnLoad]
public static class EnsureTagsExist
{
    static EnsureTagsExist()
    {
        // Ensure required tags exist
        CreateTagIfNotExists("Enemy");
        CreateTagIfNotExists("Ground");
        CreateTagIfNotExists("Player");
    }

    static void CreateTagIfNotExists(string tagName)
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

        // Create tag if it doesn't exist
        if (!found)
        {
            tagsProp.InsertArrayElementAtIndex(0);
            SerializedProperty n = tagsProp.GetArrayElementAtIndex(0);
            n.stringValue = tagName;
            Debug.Log($"Created missing tag: {tagName}");
        }

        tagManager.ApplyModifiedProperties();
    }
}
#endif