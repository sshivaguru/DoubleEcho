using UnityEngine;

public class Henchman : MonoBehaviour
{
    [Header("Patrol Settings")]
    public float patrolRange = 5f;
    public float speed = 2.5f;

    [Header("Player Respawn")]
    public Transform playerStartTransform;
    public GameObject player;

    private Vector3 startPos;
    private int direction = 1;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // Only patrol in Runner phase
        if (GameManager.Instance == null ||
            GameManager.Instance.currentPhase != GameManager.GamePhase.Runner)
            return;

        transform.Translate(Vector3.right * direction * speed * Time.deltaTime);

        if (transform.position.x > startPos.x + patrolRange)
            direction = -1;
        else if (transform.position.x < startPos.x - patrolRange)
            direction = 1;

        // Flip sprite
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.flipX = direction < 0;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        Debug.Log("Henchman caught player — respawning!");
        // Respawn player
        if (player != null && playerStartTransform != null)
            player.transform.position = playerStartTransform.position;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (!col.gameObject.CompareTag("Player")) return;
        Debug.Log("Henchman caught player — respawning!");
        if (player != null && playerStartTransform != null)
            player.transform.position = playerStartTransform.position;
    }
}
