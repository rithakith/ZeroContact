using TMPro;
using UnityEngine;

public class FinalKey : MonoBehaviour
{
    public GameObject victoryScreen;
    public GameObject needMoreUI;
    public int requiredCrystals = 40;

    private bool playerNearby = false;
    public TMP_Text NeedMoreText;

    private void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            if (Damage.Instance == null)
            {
                Debug.LogError("Damage.Instance is NULL! Is the Player in the scene?");
                return;
            }
            int crystalCount = Damage.Instance.crystalCount; // using singleton
            if (crystalCount >= requiredCrystals)
            {
                victoryScreen.SetActive(true);
                Time.timeScale = 0f;
            }
            else
            {
                needMoreUI.SetActive(true);

                // Show proper message immediately
                if (crystalCount == 0)
                {
                    NeedMoreText.text = "You have no crystals!";
                }
                else
                {
                    int missing = requiredCrystals - crystalCount;
                    NeedMoreText.text = "You Need " + missing + " more crystals!";
                }

                Invoke(nameof(HideNeedMore), 2f);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            playerNearby = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            playerNearby = false;
    }

    private void HideNeedMore()
    {
        needMoreUI.SetActive(false);
    }
}
