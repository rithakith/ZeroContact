using UnityEngine;
using UnityEditor;

public static class BatPrefabFixer
{
    [MenuItem("GameObject/Fix Missing Bat Prefabs", false, 0)]
    public static void FixMissingBatPrefabsMenu()
    {
        FixMissingBatPrefabs();
    }
    
    [MenuItem("Assets/Fix Missing Bat Prefabs in Scene")]
    public static void FixMissingBatPrefabsFromAssets()
    {
        FixMissingBatPrefabs();
    }
    
    private static void FixMissingBatPrefabs()
    {
        // Try to find the Bat prefab automatically
        string[] guids = AssetDatabase.FindAssets("t:Prefab Bat");
        GameObject batPrefab = null;
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (path.EndsWith("Bat.prefab"))
            {
                batPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                break;
            }
        }
        
        if (batPrefab == null)
        {
            Debug.LogError("Could not find Bat.prefab in the project!");
            EditorUtility.DisplayDialog("Error", "Could not find Bat.prefab in the project!\nPlease make sure Bat.prefab exists in Assets/Prefabs/", "OK");
            return;
        }
        
        // Find all missing prefab instances
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        int fixedCount = 0;
        
        foreach (GameObject obj in allObjects)
        {
            if (obj.name.Contains("Bat") && obj.name.Contains("Missing"))
            {
                // Store transform data
                Vector3 position = obj.transform.position;
                Quaternion rotation = obj.transform.rotation;
                Vector3 scale = obj.transform.localScale;
                Transform parent = obj.transform.parent;
                int siblingIndex = obj.transform.GetSiblingIndex();
                string name = obj.name.Replace(" (Missing Prefab with guid: 1bd0f777a6e3703419a1e836a14672c9)", "");
                
                // Create new instance
                GameObject newInstance = (GameObject)PrefabUtility.InstantiatePrefab(batPrefab);
                newInstance.transform.position = position;
                newInstance.transform.rotation = rotation;
                newInstance.transform.localScale = scale;
                newInstance.transform.SetParent(parent);
                newInstance.transform.SetSiblingIndex(siblingIndex);
                newInstance.name = name;
                
                // Destroy old instance
                Object.DestroyImmediate(obj);
                
                fixedCount++;
            }
        }
        
        if (fixedCount > 0)
        {
            Debug.Log($"Fixed {fixedCount} missing Bat prefab instances!");
            EditorUtility.DisplayDialog("Success", $"Fixed {fixedCount} missing Bat prefab instances!", "OK");
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        }
        else
        {
            Debug.Log("No missing Bat prefab instances found.");
            EditorUtility.DisplayDialog("Info", "No missing Bat prefab instances found in the scene.", "OK");
        }
    }
}