using System.Collections;
using UnityEngine;

public class AirControl : MonoBehaviour
{
    [Header("Dash Settings")]
    [SerializeField] private float dashForce = 5000f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 2f;
    [Header("Air Control Settings")]
    [SerializeField] private float airRotationSpeed = 50f;
    [SerializeField] private float groundCheckDistance = 1.5f;
    [SerializeField] private LayerMask groundLayer;

    private bool canJump = true;
    private bool isInAir = false;
    private Rigidbody rb;

    private float horizontalInput, verticalInput;
    SceneHandler sceneHandler;
    GameObject theGameController;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        theGameController = GameObject.FindGameObjectWithTag("GameController");
        sceneHandler = theGameController.GetComponent<SceneHandler>();
    }

    void Update()
    {
        // Check ground status
        CheckGrounded();

        // Handle jump (dash) input
        if (Input.GetKeyDown(KeyCode.Space) && canJump)
        {
            StartCoroutine(Jump());
        }

        // Update speed in scene handler
        sceneHandler.speed = Mathf.FloorToInt(rb.linearVelocity.magnitude);
    }

    private void FixedUpdate()
    {
        GetInput();

        // Apply air control if in the air
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

    private IEnumerator Jump()
    {
        canJump = false;

        // Get input direction
        Vector3 jumpUp = transform.up;

        /*
        Vector3 inputDirection = transform.up * verticalInput + transform.right * horizontalInput;
        if (inputDirection == Vector3.zero)
            inputDirection = transform.forward;
        inputDirection.Normalize();
        */

        // Apply dash force
        rb.AddForce(jumpUp * dashForce, ForceMode.Impulse);
        yield return new WaitForSeconds(dashDuration);
        yield return new WaitForSeconds(dashCooldown);
        canJump = true;
    }

    private void CheckGrounded()
    {
        isInAir = !Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
    }

    private void AirSteering()
    {
        float pitch = verticalInput * -airRotationSpeed * Time.fixedDeltaTime;
        float yaw = horizontalInput * airRotationSpeed * Time.fixedDeltaTime;
        float roll = 0f;

        if (Input.GetKey(KeyCode.Q)) roll = airRotationSpeed * Time.fixedDeltaTime;
        if (Input.GetKey(KeyCode.E)) roll = -airRotationSpeed * Time.fixedDeltaTime;

        transform.Rotate(pitch, yaw, roll, Space.Self);
    }
}
