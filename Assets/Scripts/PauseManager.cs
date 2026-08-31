using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }

    [Header("Pause Canvas")]
    public GameObject pauseCanvas;

    private Button resumeButton;
    private Button restartButton;
    private Button quitToTitleButton;
    private bool isPaused = false;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        BindPauseButtons();
    }

    private void BindPauseButtons()
    {
        if (pauseCanvas == null)
            pauseCanvas = GameObject.Find("PauseCanvas");

        if (pauseCanvas == null)
            return;

        resumeButton = pauseCanvas.transform.Find("Btn_RESUME")?.GetComponent<Button>();
        restartButton = pauseCanvas.transform.Find("Btn_RESTART")?.GetComponent<Button>();
        quitToTitleButton = pauseCanvas.transform.Find("Btn_QUIT_TO_TITLE")?.GetComponent<Button>();

        if (resumeButton != null)
            resumeButton.onClick.AddListener(Resume);

        if (restartButton != null)
            restartButton.onClick.AddListener(RestartLevel);

        if (quitToTitleButton != null)
            quitToTitleButton.onClick.AddListener(QuitToTitle);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        if (pauseCanvas != null)
            pauseCanvas.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void Resume()
    {
        isPaused = false;
        if (pauseCanvas != null) pauseCanvas.SetActive(false);
        Time.timeScale = 1f;
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitToTitle()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("TitleScreen");
    }
}
