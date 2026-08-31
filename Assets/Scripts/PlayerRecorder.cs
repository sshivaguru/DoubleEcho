using System.Collections.Generic;
using UnityEngine;

public class PlayerRecorder : MonoBehaviour
{
    public static List<Vector3> recordedPositions = new List<Vector3>();
    private bool recording = false;

    void Start()
    {
        recordedPositions.Clear();
        // Auto-start recording at beginning of Helper phase
        StartRecording();
    }

    public void StartRecording()
    {
        recordedPositions.Clear();
        recording = true;
    }

    public void StopRecording()
    {
        recording = false;
    }

    void FixedUpdate()
    {
        if (recording && GameManager.Instance != null &&
            GameManager.Instance.currentPhase == GameManager.GamePhase.Helper)
        {
            recordedPositions.Add(transform.position);
        }
    }
}
