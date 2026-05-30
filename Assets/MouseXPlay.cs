using UnityEngine;

public class MouseXPlay : MonoBehaviour
{
    private PersonalSettings personalSettings;
    private Rigidbody rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        personalSettings = FindAnyObjectByType<PersonalSettings>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * personalSettings.MouseXSensitivity * Time.deltaTime;
        //transform.Rotate(Vector3.up * mouseX, Space.Self); // <-- Do not Add Space.World

        //transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f); // Keeps the player upright, allowing rotation only on the Y axis

        // 2. Turn that into a perfect, purely upright Quaternion angle
        Quaternion turnRotation = Quaternion.Euler(0f, mouseX, 0f);

        // 3. Apply the rotation directly to the Rigidbody physics loop
        rb.MoveRotation(rb.rotation * turnRotation);
    }
}
