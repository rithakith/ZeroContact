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
        ShieldBypass
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
            content = "Every alien telegraphs their attack.\nWatch. Learn. React.\n\nPhysical = Heavy, slow swings\nFire = Glowing, area damage\nElectric = Sparking, chain strikes",
            hasDemo = false,
            demoType = DemoType.None,
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
    }
    
    void SetupUI()
    {
        if (nextButton != null)
        {
            nextButton.onClick.AddListener(NextScreen);
            SetButtonColors(nextButton, normalButtonColor, hoverButtonColor, pressedButtonColor);
        }
        
        if (backButton != null)
        {
            backButton.onClick.AddListener(PreviousScreen);
            SetButtonColors(backButton, normalButtonColor, hoverButtonColor, pressedButtonColor);
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
        
        // Always spawn player for interactive mode
        SpawnInteractivePlayer();
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
            
            // Add instruction overlay
            if (demoInstructionText != null)
            {
                string currentText = demoInstructionText.text;
                if (!currentText.Contains("Use arrow keys"))
                {
                    demoInstructionText.text += "\n\nUse arrow keys or WASD to control";
                }
            }
            
            // Focus camera on player
            if (demoCamera != null)
            {
                demoCamera.transform.position = new Vector3(demoSpawnPoint.position.x, demoSpawnPoint.position.y, -10);
            }
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
    }
    
    public void NextScreen()
    {
        if (currentScreenIndex == tutorialScreens.Length - 1)
        {
            SceneManager.LoadScene("MainMenu");
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
        // Only use Enter key for next, not Space (Space is for jumping)
        if (Input.GetKeyDown(KeyCode.Return))
        {
            NextScreen();
        }
        
        // Back button with left arrow or backspace
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.Backspace))
        {
            PreviousScreen();
        }
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}