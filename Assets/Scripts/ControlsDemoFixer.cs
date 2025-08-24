using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using System.Collections;

public class ControlsDemoFixer : MonoBehaviour
{
    [Header("Debug Info")]
    public bool hasCheckedReferences = false;
    public bool playerPrefabAssigned = false;
    public bool demoSpawnPointAssigned = false;
    public bool demoCameraAssigned = false;
    public string lastError = "";

    void Start()
    {
        StartCoroutine(CheckAndFixDemo());
    }

    public IEnumerator CheckAndFixDemo()
    {
        yield return new WaitForSeconds(0.5f); // Wait for scene to fully load

        GameObject tutorialManagerObj = GameObject.Find("ControlsTutorialManager");
        if (tutorialManagerObj == null)
        {
            lastError = "ControlsTutorialManager GameObject not found!";
            Debug.LogError(lastError);
            yield break;
        }

        // Get the tutorial manager component
        ControlsTutorialManagerEnhanced tutorialManager = tutorialManagerObj.GetComponent<ControlsTutorialManagerEnhanced>();
        
        if (tutorialManager == null)
        {
            lastError = "ControlsTutorialManagerEnhanced component not found! Adding it...";
            Debug.LogWarning(lastError);
            
            // Add the component
            tutorialManager = tutorialManagerObj.AddComponent<ControlsTutorialManagerEnhanced>();
            
            // Try to wire up references
            SetupTutorialManagerReferences(tutorialManager);
        }

        // Check critical references
        hasCheckedReferences = true;
        playerPrefabAssigned = tutorialManager.playerPrefab != null;
        demoSpawnPointAssigned = tutorialManager.demoSpawnPoint != null;
        demoCameraAssigned = tutorialManager.demoCamera != null;

        if (!playerPrefabAssigned)
        {
            lastError = "Player prefab is not assigned!";
            Debug.LogError(lastError);
            
            #if UNITY_EDITOR
            // Try to load it from the known path
            string playerPrefabPath = "Assets/Characters/Player/Prefabs/Player.prefab";
            GameObject playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(playerPrefabPath);
            if (playerPrefab != null)
            {
                tutorialManager.playerPrefab = playerPrefab;
                playerPrefabAssigned = true;
                Debug.Log("Successfully loaded and assigned player prefab from: " + playerPrefabPath);
            }
            #endif
        }

        if (!demoSpawnPointAssigned)
        {
            lastError = "Demo spawn point is not assigned!";
            Debug.LogError(lastError);
            
            // Try to find it
            GameObject spawnPoint = GameObject.Find("PlayerDemoSpawn");
            if (spawnPoint != null)
            {
                tutorialManager.demoSpawnPoint = spawnPoint.transform;
                demoSpawnPointAssigned = true;
                Debug.Log("Found and assigned demo spawn point");
            }
        }

        if (!demoCameraAssigned)
        {
            lastError = "Demo camera is not assigned!";
            Debug.LogError(lastError);
            
            // Try to find it
            GameObject[] demoCameras = GameObject.FindObjectsOfType<GameObject>();
            foreach (var obj in demoCameras)
            {
                if (obj.name == "DemoCamera" && obj.GetComponent<Camera>() != null)
                {
                    tutorialManager.demoCamera = obj.GetComponent<Camera>();
                    demoCameraAssigned = true;
                    Debug.Log("Found and assigned demo camera");
                    break;
                }
            }
        }

        // Also ensure demo area is properly set up
        if (tutorialManager.demoArea == null)
        {
            GameObject demoArea = GameObject.Find("DemoArea");
            if (demoArea != null)
            {
                tutorialManager.demoArea = demoArea;
                Debug.Log("Found and assigned demo area");
            }
        }

        // Log final status
        if (playerPrefabAssigned && demoSpawnPointAssigned && demoCameraAssigned)
        {
            lastError = "All references are properly assigned!";
            Debug.Log("Demo system is ready to use!");
        }
        else
        {
            Debug.LogError("Demo system is missing critical references. Please check the Inspector.");
        }
    }

    void SetupTutorialManagerReferences(ControlsTutorialManagerEnhanced manager)
    {
        // UI References
        manager.screenContainer = GameObject.Find("ScreenContainer");
        
        GameObject titleObj = GameObject.Find("TitleText");
        if (titleObj != null) manager.titleText = titleObj.GetComponent<TextMeshProUGUI>();
        
        GameObject contentObj = GameObject.Find("ContentText");
        if (contentObj != null) manager.contentText = contentObj.GetComponent<TextMeshProUGUI>();
        
        GameObject nextButtonObj = GameObject.Find("NextButton");
        if (nextButtonObj != null) manager.nextButton = nextButtonObj.GetComponent<Button>();
        
        GameObject buttonTextObj = GameObject.Find("ButtonText");
        if (buttonTextObj != null) manager.buttonText = buttonTextObj.GetComponent<TextMeshProUGUI>();
        
        // Demo References
        manager.demoArea = GameObject.Find("DemoArea");
        manager.leftPanel = GameObject.Find("LeftPanel");
        manager.rightPanel = GameObject.Find("RightPanel");
        
        GameObject demoInstructionObj = GameObject.Find("DemoInstructionText");
        if (demoInstructionObj != null) manager.demoInstructionText = demoInstructionObj.GetComponent<TextMeshProUGUI>();
        
        GameObject demoSpawnObj = GameObject.Find("PlayerDemoSpawn");
        if (demoSpawnObj != null) manager.demoSpawnPoint = demoSpawnObj.transform;
        
        GameObject enemySpawnObj = GameObject.Find("EnemyDemoSpawn");
        if (enemySpawnObj != null) manager.enemySpawnPoint = enemySpawnObj.transform;
        
        // Find demo camera
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        foreach (var obj in allObjects)
        {
            if (obj.name == "DemoCamera" && obj.GetComponent<Camera>() != null)
            {
                manager.demoCamera = obj.GetComponent<Camera>();
                break;
            }
        }
        
        // Background overlay
        GameObject bgOverlay = GameObject.Find("Background");
        if (bgOverlay != null)
        {
            Image img = bgOverlay.GetComponent<Image>();
            if (img != null) manager.backgroundOverlay = img;
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ControlsDemoFixer))]
public class ControlsDemoFixerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        ControlsDemoFixer fixer = (ControlsDemoFixer)target;
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Status", EditorStyles.boldLabel);
        
        if (fixer.hasCheckedReferences)
        {
            EditorGUILayout.LabelField("Player Prefab:", fixer.playerPrefabAssigned ? "✓ Assigned" : "✗ Missing");
            EditorGUILayout.LabelField("Demo Spawn Point:", fixer.demoSpawnPointAssigned ? "✓ Assigned" : "✗ Missing");
            EditorGUILayout.LabelField("Demo Camera:", fixer.demoCameraAssigned ? "✓ Assigned" : "✗ Missing");
            
            if (!string.IsNullOrEmpty(fixer.lastError))
            {
                EditorGUILayout.Space();
                EditorGUILayout.HelpBox(fixer.lastError, 
                    fixer.lastError.Contains("properly assigned") ? MessageType.Info : MessageType.Error);
            }
        }
        else
        {
            EditorGUILayout.LabelField("Not checked yet - Play the scene to check");
        }
        
        EditorGUILayout.Space();
        if (GUILayout.Button("Force Check Now"))
        {
            if (Application.isPlaying)
            {
                fixer.StartCoroutine(fixer.CheckAndFixDemo());
            }
            else
            {
                Debug.LogWarning("Can only check references in Play mode");
            }
        }
    }
}
#endif