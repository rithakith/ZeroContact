using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject gameOverMenu;

    private void OnEnable()
    {
        Damage.OnPlayerDeath += EnableGameOverMenu;
    }

    private void OnDisable()
    {
        Damage.OnPlayerDeath -= EnableGameOverMenu;
    }

    public void EnableGameOverMenu()
    {
        gameOverMenu.SetActive(true);
    }

    public void RestartLevel()
    {
        Debug.Log("RestartLevel called!");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // public void GoToMainMenu()
    // {
    //     SceneManager.LoadScene("MainMenu");
    // }
}
