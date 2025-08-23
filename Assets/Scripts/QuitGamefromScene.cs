using UnityEngine;
using UnityEngine.SceneManagement;

public class QuitGamefromScene : MonoBehaviour
{
    public GameObject quitMenuUI;
    private bool isMenuOpen = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isMenuOpen)
            {
                Resume();
            }
            else
            {
                OpenMenu();
            }
        }
    }

    private void OpenMenu()
    {
        quitMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isMenuOpen = true;
    }

    public void Resume()
    {
        quitMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isMenuOpen = false;
    }
}
