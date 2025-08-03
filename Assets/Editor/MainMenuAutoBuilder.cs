using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class MainMenuAutoBuilder : MonoBehaviour
{
    [MenuItem("Tools/Generate Cyberpunk Loading Menu")]
    public static void GenerateMenu()
    {
        // Canvas
        GameObject canvasGO = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasGO.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        // Background
        GameObject bg = new GameObject("Background", typeof(Image));
        bg.transform.SetParent(canvasGO.transform, false);
        RectTransform bgRect = bg.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        Image bgImage = bg.GetComponent<Image>();
        bgImage.color = new Color32(10, 10, 25, 255); // dark blue background

        // Logo / Title
        GameObject titleGO = new GameObject("GameTitle", typeof(TextMeshProUGUI));
        titleGO.transform.SetParent(canvasGO.transform, false);
        var titleText = titleGO.GetComponent<TextMeshProUGUI>();
        titleText.text = "ZERO CONTACT";
        titleText.fontSize = 80;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = new Color32(0, 255, 255, 255); // neon cyan
        titleText.enableWordWrapping = false;
        RectTransform titleRect = titleGO.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 1f);
        titleRect.anchorMax = new Vector2(0.5f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.anchoredPosition = new Vector2(0, -100);
        titleRect.sizeDelta = new Vector2(800, 150);

        // Menu Container
        GameObject menuPanel = new GameObject("MenuPanel", typeof(RectTransform), typeof(VerticalLayoutGroup));
        menuPanel.transform.SetParent(canvasGO.transform, false);
        RectTransform panelRect = menuPanel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.anchoredPosition = new Vector2(0, -50);
        panelRect.sizeDelta = new Vector2(400, 500);

        VerticalLayoutGroup layout = menuPanel.GetComponent<VerticalLayoutGroup>();
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.spacing = 30;
        layout.childForceExpandHeight = false;

        string[] buttons = { "Start Game", "Load Game", "Settings", "Exit" };
        foreach (string label in buttons)
        {
            GameObject btnGO = new GameObject(label.Replace(" ", "") + "Button", typeof(Button), typeof(Image));
            btnGO.transform.SetParent(menuPanel.transform, false);
            Image img = btnGO.GetComponent<Image>();
            img.color = new Color32(25, 25, 50, 200); // translucent dark

            GameObject textGO = new GameObject("Text", typeof(TextMeshProUGUI));
            textGO.transform.SetParent(btnGO.transform, false);
            TextMeshProUGUI tmp = textGO.GetComponent<TextMeshProUGUI>();
            tmp.text = label;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.fontSize = 36;
            tmp.color = new Color32(0, 255, 255, 255); // neon cyan

            RectTransform textRect = textGO.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            RectTransform btnRect = btnGO.GetComponent<RectTransform>();
            btnRect.sizeDelta = new Vector2(400, 70);
        }

        Debug.Log("🚀 Cyberpunk main menu generated!");
    }
}
