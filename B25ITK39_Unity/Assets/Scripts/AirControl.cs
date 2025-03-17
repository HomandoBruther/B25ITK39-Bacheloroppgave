using System.Collections;
using UnityEngine;

public class AirControl : MonoBehaviour
{

    [Header("Dash Settings")]
    [SerializeField] private float dashForce = 5000f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 2f;
    private bool canJump = true;
    private bool isInAir = false;

    private Rigidbody rb;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        theGameController = GameObject.FindGameObjectWithTag("GameController");
        sceneHandler = theGameController.GetComponent<SceneHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && canJump)
        {
            StartCoroutine(Jump());
        }
        sceneHandler.speed = Mathf.FloorToInt(rb.linearVelocity.magnitude);
    }

    private float horizontalInput, verticalInput;
    private float currentSteerAngle, currentBreakForce;

    SceneHandler sceneHandler;
    GameObject theGameController;


    private void FixedUpdate()
    {
        GetInput();
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
        Vector3 inputDirection = transform.up * verticalInput + transform.right * horizontalInput;

        // If no input, default to forward
        if (inputDirection == Vector3.zero)
            inputDirection = transform.forward;

        inputDirection.Normalize();

        // Apply dash force in the calculated direction
        rb.AddForce(inputDirection * dashForce, ForceMode.Impulse);
        yield return new WaitForSeconds(dashDuration);
        yield return new WaitForSeconds(dashCooldown);
        canJump = true;
    }


    private void HandleAerialSteering()
    {
        /*
        currentSteerAngle = maxSteerAngle * horizontalInput;
        frontLeftWheelCollider.steerAngle = currentSteerAngle;
        frontRightWheelCollider.steerAngle = currentSteerAngle;
        */
    }

    private void UpdateWheels()
    {
        /*
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
        */
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        wheelCollider.GetWorldPose(out Vector3 pos, out Quaternion rot);
        wheelTransform.position = pos;
        wheelTransform.rotation = rot;
    }


}