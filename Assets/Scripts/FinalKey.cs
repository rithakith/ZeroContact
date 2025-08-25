using TMPro;
using UnityEngine;

public class FinalKey : MonoBehaviour
{
    public GameObject victoryScreen;
    public GameObject needMoreUI;
    private AudioSource audioSource;
    public AudioSource backgroundMusic;
    public int requiredCrystals = 40;

    private bool playerNearby = false;
    public TMP_Text NeedMoreText;

    public GameObject hint;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

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
                // Stop background music
                if (backgroundMusic != null)
                    backgroundMusic.Stop();
                audioSource.Play();
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
        {
            playerNearby = true;
            hint.SetActive(true);
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerNearby = false;
            hint.SetActive(false);
        }

    }

    private void HideNeedMore()
    {
        needMoreUI.SetActive(false);
    }
}
