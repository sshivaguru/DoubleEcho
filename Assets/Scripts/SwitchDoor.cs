using UnityEngine;

public class SwitchDoor : MonoBehaviour
{
    [Header("Door / Piston to control")]
    public GameObject door;

    private bool triggered = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;
        if (door != null)
            door.SetActive(false); // Retract/open the door

        // Visual feedback: tint the switch
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.color = Color.green;

        Debug.Log("Switch triggered — door opened!");
    }
}
