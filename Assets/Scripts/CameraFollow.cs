using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 5f;
    public Vector2 offset = new Vector2(2f, 2f);

    [Header("Level Bounds")]
    public float minX = -14f;
    public float maxX = 20f;

    void LateUpdate()
    {
        if (target == null) return;

        float targetX = Mathf.Clamp(target.position.x + offset.x, minX, maxX);
        float targetY = target.position.y + offset.y;

        Vector3 desiredPos = new Vector3(targetX, targetY, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, desiredPos, smoothSpeed * Time.deltaTime);
    }
}
