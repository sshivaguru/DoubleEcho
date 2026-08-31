using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreen : MonoBehaviour
{
    private Button startButton;
    private Button quitButton;

    private void Awake()
    {
        startButton = GameObject.Find("Canvas/StartButton")?.GetComponent<Button>();
        quitButton = GameObject.Find("Canvas/QuitButton")?.GetComponent<Button>();

        if (startButton != null)
            startButton.onClick.AddListener(StartGame);

        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("MainGame");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
