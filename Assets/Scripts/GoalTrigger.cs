using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GoalTrigger : MonoBehaviour
{
    [Header("References")]
    public Transform playerStartTransform;
    public GameObject echoPrefab;
    public GameObject winCanvas;

    [Header("Henchman")]
    public GameObject henchman;

    private bool triggered = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;
        triggered = true;

        var gm = GameManager.Instance;
        if (gm == null) return;

        if (gm.currentPhase == GameManager.GamePhase.Helper)
            FinishHelperPhase(other.gameObject);
        else
            FinishRunnerPhase();
    }

    void FinishHelperPhase(GameObject player)
    {
        // Stop recording
        var recorder = player.GetComponent<PlayerRecorder>();
        if (recorder != null) recorder.StopRecording();

        // Switch phase
        GameManager.Instance.SetPhase(GameManager.GamePhase.Runner);

        // Reset player to start
        if (playerStartTransform != null)
            player.transform.position = playerStartTransform.position;

        // Spawn Echo
        if (echoPrefab != null && PlayerRecorder.recordedPositions.Count > 0)
        {
            Vector3 startPos = PlayerRecorder.recordedPositions.Count > 0
                ? PlayerRecorder.recordedPositions[0]
                : (playerStartTransform != null ? playerStartTransform.position : Vector3.zero);

            var echoObj = Instantiate(echoPrefab, startPos, Quaternion.identity);
            var playback = echoObj.GetComponent<EchoPlayback>();
            if (playback != null)
                playback.StartPlayback(PlayerRecorder.recordedPositions);
        }

        // Activate henchman for Runner phase
        if (henchman != null)
            henchman.SetActive(true);

        triggered = false; // Allow goal to trigger again for Runner phase
        Debug.Log("Helper phase done — Runner phase started!");
    }

    void FinishRunnerPhase()
    {
        Debug.Log("Runner done — YOU ESCAPED!");
        if (winCanvas != null)
        {
            winCanvas.SetActive(true);
            Time.timeScale = 0f;
        }
    }
}
