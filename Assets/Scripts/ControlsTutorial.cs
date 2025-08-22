using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class ControlsTutorial : MonoBehaviour
{
    [System.Serializable]
    public class TutorialStep
    {
        public string stepTitle;
        [TextArea(3, 5)]
        public string instruction;
        public string inputPrompt;
        public PlayerAction actionToDemo;
        public float demoDuration = 3f;
        public bool requiresInput = false;
        public KeyCode[] acceptedKeys;
    }
    
    public enum PlayerAction
    {
        None,
        MoveLeft,
        MoveRight,
        MoveForward,
        MoveBackward,
        Jump,
        ActivateShield,
        ShieldModePhysical,
        ShieldModeFire,
        ShieldModeElectric,
        Deflect,
        Bypass,
        ComboDeflect,
        DodgeRoll,
        Sprint
    }
    
    [Header("Tutorial Steps")]
    public List<TutorialStep> tutorialSteps = new List<TutorialStep>()
    {
        new TutorialStep { 
            stepTitle = "CONTROLS TUTORIAL", 
            instruction = "Welcome to Zero Contact! Press NEXT to begin learning the controls.", 
            inputPrompt = "Press NEXT to continue",
            actionToDemo = PlayerAction.None,
            demoDuration = 2f
        },
        new TutorialStep { 
            stepTitle = "Basic Movement", 
            instruction = "Use WASD or Arrow Keys to move your character around the area.", 
            inputPrompt = "Try moving in any direction",
            actionToDemo = PlayerAction.MoveForward,
            requiresInput = true,
            acceptedKeys = new KeyCode[] { KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.UpArrow, KeyCode.LeftArrow, KeyCode.DownArrow, KeyCode.RightArrow }
        },
        new TutorialStep { 
            stepTitle = "Jumping", 
            instruction = "Press SPACE to jump over obstacles and avoid ground attacks.", 
            inputPrompt = "Press SPACE to jump",
            actionToDemo = PlayerAction.Jump,
            requiresInput = true,
            acceptedKeys = new KeyCode[] { KeyCode.Space }
        },
        new TutorialStep { 
            stepTitle = "Sprint", 
            instruction = "Hold SHIFT while moving to sprint. This helps you escape danger quickly!", 
            inputPrompt = "Hold SHIFT and move",
            actionToDemo = PlayerAction.Sprint,
            requiresInput = true,
            acceptedKeys = new KeyCode[] { KeyCode.LeftShift, KeyCode.RightShift }
        },
        new TutorialStep { 
            stepTitle = "Zero Contact Shield", 
            instruction = "Your shield is your primary defense. Press E to activate/deactivate your shield.", 
            inputPrompt = "Press E to toggle shield",
            actionToDemo = PlayerAction.ActivateShield,
            requiresInput = true,
            acceptedKeys = new KeyCode[] { KeyCode.E }
        },
        new TutorialStep { 
            stepTitle = "Shield Modes - Physical", 
            instruction = "Press 1 for Physical Shield mode. This protects against melee and projectile attacks.", 
            inputPrompt = "Press 1 for Physical mode",
            actionToDemo = PlayerAction.ShieldModePhysical,
            requiresInput = true,
            acceptedKeys = new KeyCode[] { KeyCode.Alpha1, KeyCode.Keypad1 }
        },
        new TutorialStep { 
            stepTitle = "Shield Modes - Fire", 
            instruction = "Press 2 for Fire Shield mode. This protects against fire-based attacks from enemies.", 
            inputPrompt = "Press 2 for Fire mode",
            actionToDemo = PlayerAction.ShieldModeFire,
            requiresInput = true,
            acceptedKeys = new KeyCode[] { KeyCode.Alpha2, KeyCode.Keypad2 }
        },
        new TutorialStep { 
            stepTitle = "Shield Modes - Electric", 
            instruction = "Press 3 for Electric Shield mode. This protects against electrical attacks and stuns.", 
            inputPrompt = "Press 3 for Electric mode",
            actionToDemo = PlayerAction.ShieldModeElectric,
            requiresInput = true,
            acceptedKeys = new KeyCode[] { KeyCode.Alpha3, KeyCode.Keypad3 }
        },
        new TutorialStep { 
            stepTitle = "Deflect Attacks", 
            instruction = "Right-click (or press F) at the right moment to deflect attacks back at enemies!", 
            inputPrompt = "Right-click to practice deflecting",
            actionToDemo = PlayerAction.Deflect,
            requiresInput = true,
            acceptedKeys = new KeyCode[] { KeyCode.F, KeyCode.Mouse1 }
        },
        new TutorialStep { 
            stepTitle = "Bypass Mode", 
            instruction = "Hold Q to enter bypass mode. This lets certain attacks pass through without damage.", 
            inputPrompt = "Hold Q for bypass mode",
            actionToDemo = PlayerAction.Bypass,
            requiresInput = true,
            acceptedKeys = new KeyCode[] { KeyCode.Q }
        },
        new TutorialStep { 
            stepTitle = "Dodge Roll", 
            instruction = "Double-tap any movement direction or press CTRL to perform a dodge roll.", 
            inputPrompt = "Press CTRL to dodge",
            actionToDemo = PlayerAction.DodgeRoll,
            requiresInput = true,
            acceptedKeys = new KeyCode[] { KeyCode.LeftControl, KeyCode.RightControl }
        },
        new TutorialStep { 
            stepTitle = "Combat Strategy", 
            instruction = "Remember: Observe enemy attack patterns, switch shield modes accordingly, and time your deflects perfectly!", 
            inputPrompt = "You're ready for combat!",
            actionToDemo = PlayerAction.ComboDeflect,
            demoDuration = 5f
        }
    };
    
    [Header("UI References")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI instructionText;
    public TextMeshProUGUI inputPromptText;
    public TextMeshProUGUI progressText;
    public Button backButton;
    public Button nextButton;
    public Button menuButton;
    
    [Header("Demo References")]
    public GameObject demoPlayer;
    public PlayerDemonstration playerDemo;
    public GameObject shieldVisual;
    
    [Header("Settings")]
    public float typewriterSpeed = 0.05f;
    public AudioSource audioSource;
    public AudioClip buttonSound;
    public AudioClip successSound;
    
    [Header("Control Display")]
    public TextMeshProUGUI keyDisplayText;
    public TextMeshProUGUI actionDisplayText;
    
    private int currentStepIndex = 0;
    private bool isWaitingForInput = false;
    private bool isTyping = false;
    private Coroutine typewriterCoroutine;
    
    void Start()
    {
        Debug.Log("ControlsTutorial Start() called");
        
        // Find UI elements if not assigned
        FindUIElements();
        
        // Set up button listeners
        if (backButton != null)
        {
            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(PreviousStep);
            Debug.Log("Back button connected");
        }
        else
        {
            Debug.LogError("Back button is null!");
        }
            
        if (nextButton != null)
        {
            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick.AddListener(NextStep);
            Debug.Log("Next button connected");
        }
        else
        {
            Debug.LogError("Next button is null!");
        }
            
        if (menuButton != null)
        {
            menuButton.onClick.RemoveAllListeners();
            menuButton.onClick.AddListener(ReturnToMenu);
            Debug.Log("Menu button connected");
        }
        else
        {
            Debug.LogError("Menu button is null!");
        }
        
        // Create demo player if needed
        SetupDemoPlayer();
        
        // Start first step
        ShowStep(0);
    }
    
    void FindUIElements()
    {
        if (titleText == null)
            titleText = GameObject.Find("TutorialTitle")?.GetComponent<TextMeshProUGUI>();
            
        if (instructionText == null)
            instructionText = GameObject.Find("InstructionText")?.GetComponent<TextMeshProUGUI>();
            
        if (inputPromptText == null)
        {
            GameObject promptObj = GameObject.Find("InputPrompt");
            if (promptObj == null)
            {
                // Create input prompt
                GameObject instructPanel = GameObject.Find("InstructionPanel");
                if (instructPanel != null)
                {
                    promptObj = new GameObject("InputPrompt");
                    promptObj.transform.SetParent(instructPanel.transform, false);
                    inputPromptText = promptObj.AddComponent<TextMeshProUGUI>();
                    inputPromptText.fontSize = 20;
                    inputPromptText.alignment = TextAlignmentOptions.Center;
                    inputPromptText.color = new Color(1f, 1f, 0.5f);
                    
                    RectTransform promptRect = promptObj.GetComponent<RectTransform>();
                    promptRect.anchorMin = new Vector2(0, -0.5f);
                    promptRect.anchorMax = new Vector2(1, 0);
                    promptRect.sizeDelta = Vector2.zero;
                    promptRect.anchoredPosition = Vector2.zero;
                }
            }
            else
            {
                inputPromptText = promptObj.GetComponent<TextMeshProUGUI>();
            }
        }
        
        if (progressText == null)
            progressText = GameObject.Find("ProgressIndicator")?.GetComponent<TextMeshProUGUI>();
            
        if (backButton == null)
            backButton = GameObject.Find("BackButton")?.GetComponent<Button>();
            
        if (nextButton == null)
            nextButton = GameObject.Find("NextButton")?.GetComponent<Button>();
            
        if (menuButton == null)
            menuButton = GameObject.Find("MenuButton")?.GetComponent<Button>();
            
        if (keyDisplayText == null)
            keyDisplayText = GameObject.Find("KeyDisplay")?.GetComponent<TextMeshProUGUI>();
            
        if (actionDisplayText == null)
            actionDisplayText = GameObject.Find("ActionDisplay")?.GetComponent<TextMeshProUGUI>();
    }
    
    void SetupDemoPlayer()
    {
        if (demoPlayer == null)
        {
            // Try to find existing player
            demoPlayer = GameObject.Find("DemoPlayer");
            
            if (demoPlayer == null)
            {
                // Create a simple demo player
                demoPlayer = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                demoPlayer.name = "DemoPlayer";
                demoPlayer.transform.position = new Vector3(0, 1, 0);
                
                // Add player demonstration component
                playerDemo = demoPlayer.AddComponent<PlayerDemonstration>();
                
                // Create shield visual
                shieldVisual = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                shieldVisual.name = "ShieldVisual";
                shieldVisual.transform.SetParent(demoPlayer.transform);
                shieldVisual.transform.localPosition = Vector3.zero;
                shieldVisual.transform.localScale = Vector3.one * 2f;
                
                // Make shield transparent
                Renderer shieldRenderer = shieldVisual.GetComponent<Renderer>();
                shieldRenderer.material.color = new Color(0.2f, 0.8f, 1f, 0.3f);
                
                // Disable shield by default
                shieldVisual.SetActive(false);
            }
            else
            {
                playerDemo = demoPlayer.GetComponent<PlayerDemonstration>();
                if (playerDemo == null)
                    playerDemo = demoPlayer.AddComponent<PlayerDemonstration>();
                    
                shieldVisual = demoPlayer.transform.Find("ShieldVisual")?.gameObject;
            }
        }
        
        if (playerDemo != null)
            playerDemo.shieldVisual = shieldVisual;
    }
    
    void ShowStep(int stepIndex)
    {
        if (stepIndex < 0 || stepIndex >= tutorialSteps.Count)
            return;
        
        currentStepIndex = stepIndex;
        TutorialStep step = tutorialSteps[stepIndex];
        
        // Update UI
        if (titleText != null)
            titleText.text = step.stepTitle;
        
        // Start typewriter effect for instruction
        if (typewriterCoroutine != null)
            StopCoroutine(typewriterCoroutine);
        typewriterCoroutine = StartCoroutine(TypewriterEffect(step.instruction));
        
        if (inputPromptText != null)
            inputPromptText.text = step.inputPrompt;
        
        // Update progress
        if (progressText != null)
            progressText.text = $"Step {stepIndex + 1} / {tutorialSteps.Count}";
        
        // Update button states
        if (backButton != null)
            backButton.interactable = stepIndex > 0;
            
        if (nextButton != null)
            nextButton.interactable = !step.requiresInput;
        
        // Start player demonstration
        if (playerDemo != null && step.actionToDemo != PlayerAction.None)
        {
            playerDemo.DemonstrateAction(step.actionToDemo, step.demoDuration);
        }
        
        // Update control display
        UpdateControlDisplay(step);
        
        // Set input waiting state
        isWaitingForInput = step.requiresInput;
    }
    
    IEnumerator TypewriterEffect(string text)
    {
        isTyping = true;
        instructionText.text = "";
        
        foreach (char c in text)
        {
            instructionText.text += c;
            yield return new WaitForSeconds(typewriterSpeed);
        }
        
        isTyping = false;
    }
    
    void Update()
    {
        if (isWaitingForInput && currentStepIndex < tutorialSteps.Count)
        {
            TutorialStep step = tutorialSteps[currentStepIndex];
            
            foreach (KeyCode key in step.acceptedKeys)
            {
                if (Input.GetKeyDown(key))
                {
                    OnCorrectInput();
                    break;
                }
            }
        }
    }
    
    void OnCorrectInput()
    {
        isWaitingForInput = false;
        
        // Play success sound
        if (audioSource != null && successSound != null)
            audioSource.PlayOneShot(successSound);
        
        // Flash prompt text
        if (inputPromptText != null)
        {
            inputPromptText.text = "Great! Press NEXT to continue";
            inputPromptText.color = new Color(0.2f, 1f, 0.2f);
        }
        
        // Enable next button
        if (nextButton != null)
            nextButton.interactable = true;
    }
    
    public void NextStep()
    {
        Debug.Log($"NextStep called! Current step: {currentStepIndex}, isTyping: {isTyping}");
        
        if (isTyping) 
        {
            Debug.Log("Still typing, ignoring Next button");
            return;
        }
        
        PlayButtonSound();
        
        if (currentStepIndex < tutorialSteps.Count - 1)
        {
            Debug.Log($"Moving to step {currentStepIndex + 1}");
            ShowStep(currentStepIndex + 1);
        }
        else
        {
            Debug.Log("Tutorial complete, returning to menu");
            // Tutorial complete
            ReturnToMenu();
        }
    }
    
    public void PreviousStep()
    {
        if (isTyping) return;
        
        PlayButtonSound();
        
        if (currentStepIndex > 0)
        {
            ShowStep(currentStepIndex - 1);
        }
    }
    
    public void ReturnToMenu()
    {
        PlayButtonSound();
        SceneManager.LoadScene("MainMenu");
    }
    
    void PlayButtonSound()
    {
        if (audioSource != null && buttonSound != null)
            audioSource.PlayOneShot(buttonSound);
    }
    
    void UpdateControlDisplay(TutorialStep step)
    {
        if (keyDisplayText == null || actionDisplayText == null) return;
        
        switch (step.actionToDemo)
        {
            case PlayerAction.MoveForward:
            case PlayerAction.MoveBackward:
            case PlayerAction.MoveLeft:
            case PlayerAction.MoveRight:
                keyDisplayText.text = "W A S D";
                actionDisplayText.text = "Movement";
                break;
                
            case PlayerAction.Jump:
                keyDisplayText.text = "SPACE";
                actionDisplayText.text = "Jump";
                break;
                
            case PlayerAction.Sprint:
                keyDisplayText.text = "SHIFT";
                actionDisplayText.text = "Sprint";
                break;
                
            case PlayerAction.ActivateShield:
                keyDisplayText.text = "E";
                actionDisplayText.text = "Toggle Shield";
                break;
                
            case PlayerAction.ShieldModePhysical:
                keyDisplayText.text = "1";
                actionDisplayText.text = "Physical Shield";
                break;
                
            case PlayerAction.ShieldModeFire:
                keyDisplayText.text = "2";
                actionDisplayText.text = "Fire Shield";
                break;
                
            case PlayerAction.ShieldModeElectric:
                keyDisplayText.text = "3";
                actionDisplayText.text = "Electric Shield";
                break;
                
            case PlayerAction.Deflect:
                keyDisplayText.text = "F / RMB";
                actionDisplayText.text = "Deflect";
                break;
                
            case PlayerAction.Bypass:
                keyDisplayText.text = "Q";
                actionDisplayText.text = "Bypass Mode";
                break;
                
            case PlayerAction.DodgeRoll:
                keyDisplayText.text = "CTRL";
                actionDisplayText.text = "Dodge Roll";
                break;
                
            case PlayerAction.ComboDeflect:
                keyDisplayText.text = "COMBO";
                actionDisplayText.text = "Combat Flow";
                break;
                
            default:
                keyDisplayText.text = "-";
                actionDisplayText.text = "Watch Demo";
                break;
        }
    }
}