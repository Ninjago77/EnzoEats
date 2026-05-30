using UnityEngine;
using UnityEngine.InputSystem;

public class MouseYCam : MonoBehaviour
{
    private float xRotation = 0f;
    private PersonalSettings personalSettings;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        personalSettings = FindAnyObjectByType<PersonalSettings>();

        Cursor.lockState = CursorLockMode.Locked;

        Debug.Log("P/Cursor Locked");
    }

    // Update is called once per frame
    void LateUpdate()
    {
        float mouseY = Input.GetAxis("Mouse Y") * personalSettings.MouseYSensitivity * Time.deltaTime;
        // 2. Handle Vertical Rotation (Pitch) on the Camera
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Prevents looking upside down

        // Apply vertical rotation to this camera
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}
