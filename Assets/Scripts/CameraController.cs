using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float sensitivity = 0.1f; // ---- Sensitivity for camera movement ----
    public float maxOffset = 5f; // ---- Maximum offset for camera movement ----
    public float smoothSpeed = 0.125f; // ---- Speed of the smoothing ----

    private Vector3 initialPosition;

    void Start()
    {
        // ---- Store the initial position of the camera ----
        initialPosition = transform.localPosition;
    }

    void Update()
    {
        // ---- Get the mouse position ----
        Vector3 mousePosition = Input.mousePosition;

        // ---- Calculate the offset from the center of the screen ----
        float offsetX = (mousePosition.x - Screen.width / 2) * sensitivity;
        float offsetY = (mousePosition.y - Screen.height / 2) * sensitivity;

        // ---- Clamp the offset to the maximum allowed value ----
        offsetX = Mathf.Clamp(offsetX, -maxOffset, maxOffset);
        offsetY = Mathf.Clamp(offsetY, -maxOffset, maxOffset);

        // ---- Calculate the target position ----
        Vector3 targetPosition = initialPosition + new Vector3(offsetX, offsetY, 0);

        // ---- Smoothly move the camera towards the target position ----
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, smoothSpeed);
    }
}
