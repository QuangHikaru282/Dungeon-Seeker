using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Settings")]
    public float smoothSpeed = 5f;
    public Vector3 offset = new Vector3(0f, 1.5f, -10f);

    [Header("Bounds (tuỳ chọn)")]
    public bool useBounds;
    public float minX, maxX, minY, maxY;

    [Header("Shake")]
    public float shakeDuration = 0.3f;
    public float shakeMagnitude = 0.15f;

    private float shakeTimer;

    public void TriggerShake()
    {
        shakeTimer = shakeDuration;
    }

    void LateUpdate()
    {
        if (!target) return;

        Vector3 desired = target.position + offset;

        if (useBounds)
        {
            desired.x = Mathf.Clamp(desired.x, minX, maxX);
            desired.y = Mathf.Clamp(desired.y, minY, maxY);
        }

        transform.position = Vector3.Lerp(transform.position, desired, smoothSpeed * Time.deltaTime);

        // Shake
        if (shakeTimer > 0)
        {
            transform.position += (Vector3)Random.insideUnitCircle * shakeMagnitude;
            shakeTimer -= Time.deltaTime;
        }
    }
}