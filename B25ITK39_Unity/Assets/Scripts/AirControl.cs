using System.Collections;
using UnityEngine;

public class AirControl : MonoBehaviour
{
    [Header("Dash Settings")]
    [SerializeField] private float jumpForce = 50000f;
    [SerializeField] private float jumpDuration = 0.2f;

    [Header("Air Control Settings")]
    [SerializeField] private float airRotationSpeed = 50f;
    [SerializeField] private float groundCheckDistance = 1.5f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Airbrake Settings")]
    [SerializeField] private float airbrakeDamping = 5f; // More damping = stronger slowdown
    [SerializeField] private float normalDamping = 0.1f; // Default ground damping

    [Header("Wheels")]
    [SerializeField] private Transform[] wheelPositions;

    private int jumpCharge = 1;
    private bool isInAir = false;
    private Rigidbody rb;

    private float horizontalInput, verticalInput;
    private SceneHandler sceneHandler;
    private GameObject theGameController;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Ensure Rigidbody exists
        if (rb == null)
        {
            Debug.LogError("Rigidbody not found on " + gameObject.name);
            return;
        }

        theGameController = GameObject.FindGameObjectWithTag("GameController");

        // Ensure SceneHandler exists
        if (theGameController != null)
        {
            sceneHandler = theGameController.GetComponent<SceneHandler>();
        }
        else
        {
            Debug.LogError("GameController not found in scene!");
        }

        jumpCharge = 1;
    }

    void Update()
    {
        CheckGrounded();

        if (Input.GetKeyDown(KeyCode.Space) && jumpCharge > 0)
        {
            Jump();
        }

        // Apply airbrake when in the air
        if (isInAir && Input.GetKey(KeyCode.LeftControl))
        {
            rb.linearDamping = airbrakeDamping; // Increase damping for controlled slowdown
        }
        else
        {
            rb.linearDamping = normalDamping; // Reset damping when not braking
        }

        // Prevent NullReferenceException before using sceneHandler
        if (sceneHandler != null)
        {
            sceneHandler.speed = Mathf.FloorToInt(rb.linearVelocity.magnitude);
        }
    }

    private void FixedUpdate()
    {
        GetInput();

        if (isInAir)
        {
            AirSteering();
        }
    }

    private void GetInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
    }

    private void Jump()
    {
        jumpCharge = 0;
        Vector3 jumpUp = transform.up;
        rb.AddForce(jumpUp * jumpForce, ForceMode.Impulse);
    }

    private void CheckGrounded()
    {
        isInAir = true;
        foreach (Transform wheel in wheelPositions)
        {
            if (Physics.Raycast(wheel.position, Vector3.down, groundCheckDistance, groundLayer))
            {
                isInAir = false;
                jumpCharge = 1; // Reset jump charge when fully grounded
                break;
            }
        }
    }

    private void AirSteering()
    {
        float pitch = verticalInput * airRotationSpeed * Time.fixedDeltaTime;
        float yaw = horizontalInput * airRotationSpeed * Time.fixedDeltaTime;
        float roll = 0f;

        if (Input.GetKey(KeyCode.Q)) roll = airRotationSpeed * Time.fixedDeltaTime;
        if (Input.GetKey(KeyCode.E)) roll = -airRotationSpeed * Time.fixedDeltaTime;

        transform.Rotate(pitch, yaw, roll, Space.Self);
    }
}
