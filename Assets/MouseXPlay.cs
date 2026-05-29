using UnityEngine;

public class MouseXPlay : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * 500f * Time.deltaTime;
        transform.Rotate(Vector3.up * mouseX, Space.World); // <-- Add Space.World
    }
}
