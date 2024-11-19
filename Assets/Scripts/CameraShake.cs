using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public float shakeDuration = 0.5f; // ---- Duration of the shake ----
    public float shakeMagnitude = 0.5f; // ---- Magnitude of the shake ----

    private Vector3 shakeOffset;

    public IEnumerator Shake(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            shakeOffset = new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        shakeOffset = Vector3.zero;
    }

    // ---- Shake offset is pretty much required, without it the camera shake will look snappy since it's beginning
    // at a position unrelated to the camera ----
    public Vector3 GetShakeOffset()
    {
        return shakeOffset;
    }
}