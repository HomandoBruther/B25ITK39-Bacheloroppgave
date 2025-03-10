using System;
using System.Collections;
using UnityEngine;

public class CarController : MonoBehaviour
{
    private float horizontalInput, verticalInput;
    private float currentSteerAngle, currentBreakForce;
    private float currentDampeningForce;
    private bool isBreaking;
    public int passengerCount = 0;

    SceneHandler sceneHandler;
    GameObject theGameController;

    [Header("Settings")]
    [SerializeField] private float motorForce, breakForce, dampeningForce, maxSteerAngle;

    [Header("Dash Settings")]
    [SerializeField] private float dashForce = 5000f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 2f;
    private bool canDash = true;


    // Wheel Colliders
    [SerializeField] private WheelCollider frontLeftWheelCollider, frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider, rearRightWheelCollider;

    // Wheels
    [SerializeField] private Transform frontLeftWheelTransform, frontRightWheelTransform;
    [SerializeField] private Transform rearLeftWheelTransform, rearRightWheelTransform;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        theGameController = GameObject.FindGameObjectWithTag("GameController");
        sceneHandler = theGameController.GetComponent<SceneHandler>();
    }

    private void FixedUpdate()
    {
        GetInput();
        HandleMotor();
        HandleSteering();
        UpdateWheels();
        //Debug.Log("Current WheelCollider.motorTorque " + frontLeftWheelCollider.motorTorque);
        Debug.Log("Current wheelDampingRate " + frontLeftWheelCollider.wheelDampingRate);

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }
        sceneHandler.speed = Mathf.FloorToInt(rb.linearVelocity.magnitude);
    }

    private void GetInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        isBreaking = Input.GetKey(KeyCode.Space);
    }

    private void HandleMotor()
    {
        rearLeftWheelCollider.motorTorque = verticalInput * motorForce;
        rearRightWheelCollider.motorTorque = verticalInput * motorForce;
        frontLeftWheelCollider.motorTorque = verticalInput * motorForce;
        frontRightWheelCollider.motorTorque = verticalInput * motorForce;
        currentBreakForce = isBreaking ? breakForce : 0f;
        currentDampeningForce = isBreaking ? dampeningForce : 0f;

        ApplyBreaking();
    }

    private void ApplyBreaking()
    {
        frontRightWheelCollider.brakeTorque = currentBreakForce;
        frontLeftWheelCollider.brakeTorque = currentBreakForce;
        rearLeftWheelCollider.brakeTorque = currentBreakForce;
        rearRightWheelCollider.brakeTorque = currentBreakForce;

        frontRightWheelCollider.wheelDampingRate = currentDampeningForce;
        frontLeftWheelCollider.wheelDampingRate = currentDampeningForce;
    }

    private void HandleSteering()
    {
        currentSteerAngle = maxSteerAngle * horizontalInput;
        frontLeftWheelCollider.steerAngle = currentSteerAngle;
        frontRightWheelCollider.steerAngle = currentSteerAngle;
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        wheelCollider.GetWorldPose(out Vector3 pos, out Quaternion rot);
        wheelTransform.position = pos;
        wheelTransform.rotation = rot;
    }

    private IEnumerator Dash()
    {
        canDash = false;

        // Get input direction
        Vector3 inputDirection = transform.forward * verticalInput + transform.right * horizontalInput;

        // If no input, default to forward
        if (inputDirection == Vector3.zero)
            inputDirection = transform.forward;

        inputDirection.Normalize();

        // Apply dash force in the calculated direction
        rb.AddForce(inputDirection * dashForce, ForceMode.Impulse);
        yield return new WaitForSeconds(dashDuration);
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
    //- Har gjort at PlayerData.PD håndterer passasjerer
    public void PickupPassengers(int amount)
    {
        if (PlayerData.PD != null)
        {
            int leftover = PlayerData.PD.FillPassengers(amount);
            Debug.Log("Total Passengers: " + PlayerData.PD.currentPassengers);

            if (leftover > 0)
            {
                Debug.Log(leftover + " passengers couldn't fit!");
            }
        }
    }

    public void DropOffPassengers()
    {
        if (PlayerData.PD != null)
        {
            int pointsGained = PlayerData.PD.ScorePoints();
            Debug.Log("Scored: " + pointsGained + " points!");
        }
    }
}


