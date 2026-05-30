using UnityEngine;

public class MouseXPlay : MonoBehaviour
{
    private PersonalSettings personalSettings;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        personalSettings = FindAnyObjectByType<PersonalSettings>();
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * personalSettings.MouseXSensitivity * Time.deltaTime;
        transform.Rotate(Vector3.up * mouseX, Space.Self); // <-- Do not Add Space.World
    }
}
