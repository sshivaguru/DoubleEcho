using System.Collections.Generic;
using UnityEngine;

public class EchoPlayback : MonoBehaviour
{
    private List<Vector3> positions;
    private int frameIndex = 0;
    private bool playing = false;

    public void StartPlayback(List<Vector3> recordedPositions)
    {
        positions = new List<Vector3>(recordedPositions);
        frameIndex = 0;
        playing = true;
        if (positions.Count > 0)
            transform.position = positions[0];
    }

    void FixedUpdate()
    {
        if (!playing || positions == null || positions.Count == 0) return;
        if (frameIndex < positions.Count)
        {
            transform.position = positions[frameIndex];
            frameIndex++;
        }
        else
        {
            // Loop or stop at the end
            playing = false;
        }
    }
}
