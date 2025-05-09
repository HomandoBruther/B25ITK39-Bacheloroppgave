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

    public int amountOfJumpCharges = 1;
    public bool enableAirJump = true;

    public int JumpCharge { get; private set; }
    private int airJumpCharge;
    public bool IsInAir { get; private set; } = false;
    private Rigidbody rb;

    private float horizontalInput, verticalInput;
    private SceneHandler sceneHandler;
    private GameObject theGameController;

    void Start()
    {
        JumpCharge = amountOfJumpCharges;
        airJumpCharge = amountOfJumpCharges;

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

        JumpCharge = 1;
    }

    void Update()
    {
        CheckGrounded();

        if (Input.GetKeyDown(KeyCode.Space) && JumpCharge > 0 && !IsInAir)
        {
            Jump(IsInAir);
        }

        if (Input.GetKeyDown(KeyCode.Space) && airJumpCharge > 0 && IsInAir)
        {
            Jump(IsInAir);
        }

        // Apply airbrake when in the air
        if (IsInAir && Input.GetKey(KeyCode.LeftControl))
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

        if (IsInAir)
        {
            AirSteering();
        }
    }

    private void GetInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
    }

    private void Jump(bool localIsInAir)
    {
        if(enableAirJump) {
            if (!localIsInAir) JumpCharge -= 1;
            if (localIsInAir) airJumpCharge -= 1;
        }
        else {
            JumpCharge -= 1;
            airJumpCharge -= 1;
        }
        
        Vector3 jumpUp = transform.up;
        rb.AddForce(jumpUp * jumpForce, ForceMode.Impulse);
    }

    private void CheckGrounded()
    {
        IsInAir = true;
        foreach (Transform wheel in wheelPositions)
        {
            if (Physics.Raycast(wheel.position, Vector3.down, groundCheckDistance, groundLayer))
            {
                IsInAir = false;
                JumpCharge = amountOfJumpCharges; // Reset jump charge when fully grounded
                airJumpCharge = amountOfJumpCharges;
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
