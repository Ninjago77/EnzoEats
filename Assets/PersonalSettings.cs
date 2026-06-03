using UnityEngine;

public class PersonalSettings : MonoBehaviour
{
    public float MouseXSensitivity = 500f;
    public float MouseYSensitivity = 500f;


    public static PersonalSettings Instance { get; private set; }
    private void Awake()
    {
        // Singleton pattern to prevent duplicates
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy the duplicate
            return;
        }

        Instance = this;

        // Keeps this GameObject between scene loads
        DontDestroyOnLoad(gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
