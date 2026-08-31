using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GamePhase { Helper, Runner }
    public GamePhase currentPhase = GamePhase.Helper;

    [Header("UI")]
    public TMP_Text phaseText;

    [Header("References")]
    public Transform playerStartPos;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        UpdatePhaseUI();
    }

    public void SetPhase(GamePhase phase)
    {
        currentPhase = phase;
        UpdatePhaseUI();
    }

    void UpdatePhaseUI()
    {
        if (phaseText == null) return;
        if (currentPhase == GamePhase.Helper)
            phaseText.text = "STAGE 1: HELPER";
        else
            phaseText.text = "STAGE 2: RUNNER";
    }
}
