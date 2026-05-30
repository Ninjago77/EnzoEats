using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 4f;
    public float sprintSpeed = 14f;
    [Space]
    public float maxVelocityChange = 10f;
    //public float XZFriction = .01f;
    [Space]
    public float jumpHeight = 1.5f;

    private Vector2 input;
    private Rigidbody rb;
    private bool isSprinting = false;
    private bool isJumping = false;

    private bool isGrounded = false;
    private int jumpCount = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerStay(Collider other)
    {
        isGrounded = true;
        jumpCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        input.Normalize();

        isSprinting = Input.GetButton("Sprint");
        isJumping = Input.GetButton("Jump");
    }

    private void FixedUpdate()
    {
        if (isJumping)
        {
            //rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpHeight, rb.linearVelocity.z);
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y + (jumpHeight/(Mathf.Pow(2f,jumpCount))), rb.linearVelocity.z);
            //rb.linearVelocity = new Vector3(rb.linearVelocity.x, Mathf.Sqrt((2f * Mathf.Abs(Physics.gravity.y) * jumpHeight) + (Mathf.Pow(Mathf.Max(0f, rb.linearVelocity.y), 2f) / 2f)), rb.linearVelocity.z);
            isGrounded = false;
            jumpCount++;
        }

        //if (input.magnitude > 0.5f) {
            rb.AddForce(CalculateMovement((isSprinting && isGrounded) ? sprintSpeed : walkSpeed), ForceMode.VelocityChange);
        //} 
        //else
        //{
        //    rb.linearVelocity = new Vector3(rb.linearVelocity.x * XZFriction * Time.fixedDeltaTime, rb.linearVelocity.y, rb.linearVelocity.z * XZFriction * Time.fixedDeltaTime);
            
        //}
    }

    Vector3 CalculateMovement(float _speed)
    {
        Vector3 targetVelocity = new Vector3(input.x,0,input.y);
        targetVelocity = transform.TransformDirection(targetVelocity);

        targetVelocity *= _speed;

        Vector3 currentVelocity = rb.linearVelocity;

        if (input.magnitude > 0.5f)
        {
            Vector3 velocityChange = targetVelocity - currentVelocity;

            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = 0;

            return velocityChange;

        } else
        {
            return Vector3.zero;
        }

    }

}
