using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame() => SceneManager.LoadScene("SampleScene");
    public void LoadGame() => Debug.Log("Loading not implemented.");
    public void OpenSettings() => Debug.Log("Settings menu coming soon.");
    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Game closed.");
    }
}
