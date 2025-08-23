using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.InputSystem;

public class ControlsTutorialManagerEnhanced : MonoBehaviour
{
    [System.Serializable]
    public class TutorialScreen
    {
        public string title;
        [TextArea(5, 10)]
        public string content;
        public bool hasDemo;
        public DemoType demoType;
        public string buttonText = "NEXT";
    }
    
    public enum DemoType
    {
        None,
        Jump,
        MoveLeftRight,
        ShieldDeflect,
        ShieldAbsorb,
        ShieldBypass,
        EnemyShowcase
    }
    
    [Header("UI References")]
    public GameObject screenContainer;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI contentText;
    public Button nextButton;
    public TextMeshProUGUI buttonText;
    public Button backButton;
    public TextMeshProUGUI backButtonText;
    public GameObject demoArea;
    public Image backgroundOverlay;
    
    [Header("Demo Layout - Split Screen")]
    public GameObject leftPanel;
    public GameObject rightPanel;
    public TextMeshProUGUI demoInstructionText;
    
    // No try it button needed - always interactive
    
    [Header("Demo Elements")]
    public GameObject playerPrefab;
    public Transform demoSpawnPoint;
    public GameObject enemyDemoPrefab;
    public Transform enemySpawnPoint;
    public Camera demoCamera;
    
    [Header("Enemy Prefabs")]
    public GameObject spikePrefab;
    public GameObject batPrefab;
    public Transform[] enemyShowcaseSpawnPoints;
    
    [Header("Visual Styling")]
    public Color normalButtonColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);
    public Color hoverButtonColor = new Color(0.3f, 0.3f, 0.3f, 0.9f);
    public Color pressedButtonColor = new Color(0.1f, 0.1f, 0.1f, 0.8f);
    public Color tryItButtonColor = new Color(0.2f, 0.5f, 0.8f, 0.9f);
    public Color textColor = Color.white;
    public float fadeSpeed = 2f;
    public float screenTransitionTime = 0.5f;
    
    [Header("Tutorial Screens")]
    public TutorialScreen[] tutorialScreens = new TutorialScreen[]
    {
        new TutorialScreen
        {
            title = "YOUR CHILDHOOD FRIEND HAS BEEN TAKEN",
            content = "Alien forces have abducted her.\nEvery moment we waste, she slips further away.\n\nYou must master the Zero Contact Shield\nto survive the alien dungeons ahead.\n\nFollow these instructions carefully.\nHer life depends on it.",
            hasDemo = false,
            demoType = DemoType.None,
            buttonText = "NEXT - Time is running out"
        },
        new TutorialScreen
        {
            title = "ZERO CONTACT SHIELD SYSTEM",
            content = "This adaptive shield is your only defense\nagainst the alien threat.\n\nMaster it quickly.\nYour friend needs you.",
            hasDemo = false,
            demoType = DemoType.None,
            buttonText = "BEGIN TRAINING"
        },
        new TutorialScreen
        {
            title = "BASIC MOVEMENT - JUMPING",
            content = "Press SPACE to JUMP\n\nEssential for dodging ground attacks\nand navigating vertical obstacles.",
            hasDemo = true,
            demoType = DemoType.Jump,
            buttonText = "NEXT when ready"
        },
        new TutorialScreen
        {
            title = "BASIC MOVEMENT - LEFT/RIGHT",
            content = "Press LEFT or RIGHT to MOVE\n\nQuick positioning saves lives.\nStay mobile to survive.",
            hasDemo = true,
            demoType = DemoType.MoveLeftRight,
            buttonText = "NEXT when ready"
        },
        new TutorialScreen
        {
            title = "ACTIVATING YOUR SHIELD",
            content = "Your shield adapts to counter specific threats.\nWrong mode = certain death.\n\nLearn each mode. Fast.",
            hasDemo = false,
            demoType = DemoType.None,
            buttonText = "LEARN SHIELD MODES"
        },
        new TutorialScreen
        {
            title = "SHIELD MODE: DEFLECT",
            content = "Press SPACE to DEFLECT\n\nReflects physical projectiles back at enemies.\nPerfect timing multiplies damage.",
            hasDemo = true,
            demoType = DemoType.ShieldDeflect,
            buttonText = "NEXT when mastered"
        },
        new TutorialScreen
        {
            title = "SHIELD MODE: ABSORB",
            content = "Hold SPACE to ABSORB\n\nNeutralizes elemental attacks (fire, electricity).\nConverts absorbed energy for shield power.",
            hasDemo = true,
            demoType = DemoType.ShieldAbsorb,
            buttonText = "NEXT when mastered"
        },
        new TutorialScreen
        {
            title = "SHIELD MODE: BYPASS",
            content = "Double-tap SPACE to BYPASS\n\nPhase through unblockable attacks.\nDrains shield energy - use wisely.",
            hasDemo = true,
            demoType = DemoType.ShieldBypass,
            buttonText = "NEXT when mastered"
        },
        new TutorialScreen
        {
            title = "ENEMY ATTACK PATTERNS",
            content = "Every alien telegraphs their attack.\nWatch. Learn. React.\n\nSpikes = Physical damage, predictable patterns\nBats = Flying enemies, swooping attacks",
            hasDemo = true,
            demoType = DemoType.EnemyShowcase,
            buttonText = "CONTINUE"
        },
        new TutorialScreen
        {
            title = "ELEMENTAL DUNGEONS AHEAD",
            content = "Each dungeon amplifies specific attack types:\n\nPHYSICAL ZONE: Brutal melee combat\nFIRE ZONE: Constant burning threats\nELECTRIC ZONE: Lightning-fast strikes\n\nAdapt or perish.",
            hasDemo = false,
            demoType = DemoType.None,
            buttonText = "ALMOST READY"
        },
        new TutorialScreen
        {
            title = "TIME TO MOVE",
            content = "Your friend is counting on you.\nYou've learned the basics.\n\nRemember:\n- Stay mobile\n- Match shield to attack type\n- Perfect timing saves energy\n\nGo. Save her.",
            hasDemo = false,
            demoType = DemoType.None,
            buttonText = "BEGIN THE RESCUE"
        }
    };
    
    private int currentScreenIndex = 0;
    private GameObject currentDemoPlayer;
    private GameObject currentDemoEnemy;
    private Coroutine demoCoroutine;
    private bool isDemoActive = false;
    private bool isInteractiveMode = true;  // Always interactive
    
    void Start()
    {
        SetupUI();
        ShowScreen(0);
        
        // Ensure no button is selected by default to prevent Space key activation
        if (UnityEngine.EventSystems.EventSystem.current != null)
        {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
        }
    }
    
    void SetupUI()
    {
        if (nextButton != null)
        {
            nextButton.onClick.AddListener(NextScreen);
            SetButtonColors(nextButton, normalButtonColor, hoverButtonColor, pressedButtonColor);
            
            // Disable keyboard navigation on the button
            Navigation nav = nextButton.navigation;
            nav.mode = Navigation.Mode.None;
            nextButton.navigation = nav;
        }
        
        if (backButton != null)
        {
            backButton.onClick.AddListener(PreviousScreen);
            SetButtonColors(backButton, normalButtonColor, hoverButtonColor, pressedButtonColor);
            
            // Disable keyboard navigation on the button
            Navigation nav = backButton.navigation;
            nav.mode = Navigation.Mode.None;
            backButton.navigation = nav;
        }
        
        if (titleText != null) titleText.color = textColor;
        if (contentText != null) contentText.color = textColor;
        if (buttonText != null) buttonText.color = textColor;
        if (backButtonText != null) backButtonText.color = textColor;
    }
    
    void SetButtonColors(Button button, Color normal, Color highlighted, Color pressed)
    {
        ColorBlock colors = button.colors;
        colors.normalColor = normal;
        colors.highlightedColor = highlighted;
        colors.pressedColor = pressed;
        colors.colorMultiplier = 1f;
        colors.fadeDuration = 0.1f;
        button.colors = colors;
    }
    
    void ShowScreen(int index)
    {
        if (index < 0 || index >= tutorialScreens.Length) return;
        
        currentScreenIndex = index;
        TutorialScreen screen = tutorialScreens[index];
        
        // Always interactive mode
        isInteractiveMode = true;
        
        StartCoroutine(TransitionToScreen(screen));
    }
    
    IEnumerator TransitionToScreen(TutorialScreen screen)
    {
        float elapsedTime = 0f;
        
        // Fade out
        while (elapsedTime < screenTransitionTime / 2)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / (screenTransitionTime / 2));
            SetUIAlpha(alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        UpdateScreenContent(screen);
        
        // Fade in
        elapsedTime = 0f;
        while (elapsedTime < screenTransitionTime / 2)
        {
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / (screenTransitionTime / 2));
            SetUIAlpha(alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        SetUIAlpha(1f);
        
        if (screen.hasDemo)
        {
            StartDemo(screen.demoType);
        }
    }
    
    void UpdateScreenContent(TutorialScreen screen)
    {
        if (titleText != null)
            titleText.text = screen.title;
            
        if (buttonText != null)
            buttonText.text = screen.buttonText;
            
        // Handle different layouts for demo vs non-demo screens
        if (screen.hasDemo)
        {
            // Split screen layout
            if (leftPanel != null) leftPanel.SetActive(true);
            if (rightPanel != null) rightPanel.SetActive(true);
            if (demoArea != null) demoArea.SetActive(true);
            
            // Hide center content, show demo instruction
            if (contentText != null) contentText.gameObject.SetActive(false);
            if (demoInstructionText != null)
            {
                demoInstructionText.gameObject.SetActive(true);
                demoInstructionText.text = screen.content;
            }
            
            // No try it button needed
            
            // Enable demo camera
            if (demoCamera != null) demoCamera.gameObject.SetActive(true);
        }
        else
        {
            // Full screen layout for non-demo screens
            if (leftPanel != null) leftPanel.SetActive(false);
            if (rightPanel != null) rightPanel.SetActive(false);
            if (demoArea != null) demoArea.SetActive(false);
            
            // Show center content
            if (contentText != null)
            {
                contentText.gameObject.SetActive(true);
                contentText.text = screen.content;
            }
            if (demoInstructionText != null) demoInstructionText.gameObject.SetActive(false);
            
            // No try it button needed
            
            // Disable demo camera
            if (demoCamera != null) demoCamera.gameObject.SetActive(false);
        }
    }
    
    void SetUIAlpha(float alpha)
    {
        if (titleText != null)
        {
            Color c = titleText.color;
            c.a = alpha;
            titleText.color = c;
        }
        
        if (contentText != null)
        {
            Color c = contentText.color;
            c.a = alpha;
            contentText.color = c;
        }
        
        if (buttonText != null)
        {
            Color c = buttonText.color;
            c.a = alpha;
            buttonText.color = c;
        }
        
        if (demoInstructionText != null)
        {
            Color c = demoInstructionText.color;
            c.a = alpha;
            demoInstructionText.color = c;
        }
    }
    
    void StartDemo(DemoType demoType)
    {
        if (demoCoroutine != null)
        {
            StopCoroutine(demoCoroutine);
        }
        
        CleanupDemo();
        isDemoActive = true;
        
        // Always spawn player for interactive mode (except for enemy showcase)
        if (demoType != DemoType.EnemyShowcase)
        {
            SpawnInteractivePlayer();
        }
        else
        {
            // For enemy showcase, spawn enemies instead
            SpawnEnemyShowcase();
        }
    }
    
    // No toggle needed - always interactive
    
    void SpawnInteractivePlayer()
    {
        if (playerPrefab != null && demoSpawnPoint != null)
        {
            currentDemoPlayer = Instantiate(playerPrefab, demoSpawnPoint.position, Quaternion.identity);
            currentDemoPlayer.name = "InteractivePlayer";
            
            // Make sure player input is enabled
            PlayerInput playerInput = currentDemoPlayer.GetComponent<PlayerInput>();
            if (playerInput != null)
            {
                playerInput.enabled = true;
            }
            
           
            
            // Focus camera on player
            if (demoCamera != null)
            {
                demoCamera.transform.position = new Vector3(demoSpawnPoint.position.x, demoSpawnPoint.position.y, -10);
            }
        }
    }
    
    void SpawnEnemyShowcase()
    {
        // Simple display of enemies
        if (demoSpawnPoint != null)
        {
            // Spawn spike on the left
            if (spikePrefab != null)
            {
                Vector3 spikePos = demoSpawnPoint.position + Vector3.left * 2;
                GameObject spike = Instantiate(spikePrefab, spikePos, Quaternion.identity);
                spike.name = "DemoSpike";
                
                // Scale up the spike to be more visible
                spike.transform.localScale = Vector3.one * 3f; // 3x bigger
            }
            
            // Spawn bat on the right
            if (batPrefab != null)
            {
                Vector3 batPos = demoSpawnPoint.position + Vector3.right * 2 + Vector3.up * 1;
                GameObject bat = Instantiate(batPrefab, batPos, Quaternion.identity);
                bat.name = "DemoBat";
                
                // Scale up the bat to be more visible
                bat.transform.localScale = Vector3.one * 3f; // 3x bigger
            }
        }
        
        // Focus camera on demo area
        if (demoCamera != null && demoSpawnPoint != null)
        {
            demoCamera.transform.position = new Vector3(demoSpawnPoint.position.x, demoSpawnPoint.position.y, -10);
        }
    }
    
    // Demo coroutines (automated)
    IEnumerator DemoJump()
    {
        if (playerPrefab != null && demoSpawnPoint != null)
        {
            currentDemoPlayer = Instantiate(playerPrefab, demoSpawnPoint.position, Quaternion.identity);
            currentDemoPlayer.name = "DemoPlayer";
            
            // Disable player input for demo
            PlayerInput playerInput = currentDemoPlayer.GetComponent<PlayerInput>();
            if (playerInput != null)
            {
                playerInput.enabled = false;
            }
            
            PlayerDemonstration demo = currentDemoPlayer.GetComponent<PlayerDemonstration>();
            if (demo == null)
            {
                demo = currentDemoPlayer.AddComponent<PlayerDemonstration>();
            }
            
            // Focus camera
            if (demoCamera != null)
            {
                demoCamera.transform.position = new Vector3(demoSpawnPoint.position.x, demoSpawnPoint.position.y, -10);
            }
            
            while (false)  // Never run automated demos
            {
                if (demo != null)
                {
                    demo.DemoJump();
                }
                yield return new WaitForSeconds(2f);
            }
        }
    }
    
    IEnumerator DemoMove()
    {
        if (playerPrefab != null && demoSpawnPoint != null)
        {
            currentDemoPlayer = Instantiate(playerPrefab, demoSpawnPoint.position, Quaternion.identity);
            currentDemoPlayer.name = "DemoPlayer";
            
            PlayerInput playerInput = currentDemoPlayer.GetComponent<PlayerInput>();
            if (playerInput != null)
            {
                playerInput.enabled = false;
            }
            
            PlayerDemonstration demo = currentDemoPlayer.GetComponent<PlayerDemonstration>();
            if (demo == null)
            {
                demo = currentDemoPlayer.AddComponent<PlayerDemonstration>();
            }
            
            if (demoCamera != null)
            {
                demoCamera.transform.position = new Vector3(demoSpawnPoint.position.x, demoSpawnPoint.position.y, -10);
            }
            
            while (false)  // Never run automated demos
            {
                if (demo != null)
                {
                    demo.DemoMoveLeftRight();
                }
                yield return new WaitForSeconds(4f);
            }
        }
    }
    
    IEnumerator DemoShieldDeflect()
    {
        if (playerPrefab != null && demoSpawnPoint != null)
        {
            currentDemoPlayer = Instantiate(playerPrefab, demoSpawnPoint.position, Quaternion.identity);
            currentDemoPlayer.name = "DemoPlayer";
            
            PlayerInput playerInput = currentDemoPlayer.GetComponent<PlayerInput>();
            if (playerInput != null)
            {
                playerInput.enabled = false;
            }
            
            PlayerDemonstration demo = currentDemoPlayer.GetComponent<PlayerDemonstration>();
            if (demo == null)
            {
                demo = currentDemoPlayer.AddComponent<PlayerDemonstration>();
            }
            
            if (demoCamera != null)
            {
                demoCamera.transform.position = new Vector3(demoSpawnPoint.position.x, demoSpawnPoint.position.y, -10);
            }
            
            while (false)  // Never run automated demos
            {
                if (demo != null)
                {
                    demo.DemoShieldDeflect();
                }
                yield return new WaitForSeconds(3f);
            }
        }
    }
    
    IEnumerator DemoShieldAbsorb()
    {
        if (playerPrefab != null && demoSpawnPoint != null)
        {
            currentDemoPlayer = Instantiate(playerPrefab, demoSpawnPoint.position, Quaternion.identity);
            currentDemoPlayer.name = "DemoPlayer";
            
            PlayerInput playerInput = currentDemoPlayer.GetComponent<PlayerInput>();
            if (playerInput != null)
            {
                playerInput.enabled = false;
            }
            
            PlayerDemonstration demo = currentDemoPlayer.GetComponent<PlayerDemonstration>();
            if (demo == null)
            {
                demo = currentDemoPlayer.AddComponent<PlayerDemonstration>();
            }
            
            if (demoCamera != null)
            {
                demoCamera.transform.position = new Vector3(demoSpawnPoint.position.x, demoSpawnPoint.position.y, -10);
            }
            
            while (false)  // Never run automated demos
            {
                if (demo != null)
                {
                    demo.DemoShieldAbsorb();
                }
                yield return new WaitForSeconds(3f);
            }
        }
    }
    
    IEnumerator DemoShieldBypass()
    {
        if (playerPrefab != null && demoSpawnPoint != null)
        {
            currentDemoPlayer = Instantiate(playerPrefab, demoSpawnPoint.position, Quaternion.identity);
            currentDemoPlayer.name = "DemoPlayer";
            
            PlayerInput playerInput = currentDemoPlayer.GetComponent<PlayerInput>();
            if (playerInput != null)
            {
                playerInput.enabled = false;
            }
            
            PlayerDemonstration demo = currentDemoPlayer.GetComponent<PlayerDemonstration>();
            if (demo == null)
            {
                demo = currentDemoPlayer.AddComponent<PlayerDemonstration>();
            }
            
            if (demoCamera != null)
            {
                demoCamera.transform.position = new Vector3(demoSpawnPoint.position.x, demoSpawnPoint.position.y, -10);
            }
            
            while (false)  // Never run automated demos
            {
                if (demo != null)
                {
                    demo.DemoShieldBypass();
                }
                yield return new WaitForSeconds(3f);
            }
        }
    }
    
    void CleanupDemo()
    {
        isDemoActive = false;
        
        if (currentDemoPlayer != null)
        {
            Destroy(currentDemoPlayer);
        }
        
        if (currentDemoEnemy != null)
        {
            Destroy(currentDemoEnemy);
        }
        
        // Clean up any demo enemies
        GameObject[] demoEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in demoEnemies)
        {
            if (enemy.name.StartsWith("Demo"))
            {
                Destroy(enemy);
            }
        }
    }
    
    public void NextScreen()
    {
        if (currentScreenIndex == tutorialScreens.Length - 1)
        {
            // On last screen (BEGIN THE RESCUE), load the game scene
            SceneManager.LoadScene("SampleScene");
        }
        else
        {
            ShowScreen(currentScreenIndex + 1);
        }
    }
    
    public void PreviousScreen()
    {
        if (currentScreenIndex > 0)
        {
            ShowScreen(currentScreenIndex - 1);
        }
        else
        {
            // On first screen, back button returns to main menu
            SceneManager.LoadScene("MainMenu");
        }
    }
    
    void OnDestroy()
    {
        CleanupDemo();
    }
    
    void Update()
    {
        // No keyboard navigation for next/previous - only button clicks
        // Removed Enter key navigation to avoid conflicts
        
        // Only Escape key returns to main menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}