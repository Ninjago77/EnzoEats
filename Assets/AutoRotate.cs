using UnityEngine;

public class AutoRotate : MonoBehaviour
{
    public Vector3 rotationSpeed = new Vector3(0f, 50f, 0f);

    void Update()
    {
        // Rotates the object based on the speed and the time passed between frames
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

}
