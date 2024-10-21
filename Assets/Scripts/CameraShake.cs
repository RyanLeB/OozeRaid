using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public float shakeDuration = 0.5f; // Duration of the shake
    public float shakeMagnitude = 0.5f; // Magnitude of the shake

    private void Awake()
    {
        StartCoroutine(Shake(shakeDuration, shakeMagnitude)); // ---- Start shaking on Awake ----
    }

    private IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPosition = transform.localPosition;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            // Shake the camera's position
            transform.localPosition = new Vector3(x, y, originalPosition.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Reset to original position
        transform.localPosition = originalPosition;
    }
}
