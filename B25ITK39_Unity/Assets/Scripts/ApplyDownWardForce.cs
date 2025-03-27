using UnityEngine;

public class ApplyDownwardForce : MonoBehaviour
{
    public float downwardForce = 10f;  
    public KeyCode buttonToHold = KeyCode.Space;  

    private Rigidbody rb;

    void Start()
    {
        // Get the Rigidbody component 
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Check if the button clicked
        if (Input.GetKey(buttonToHold))
        {
            // Apply downward force 
            rb.AddForce(Vector3.down * downwardForce);
        }
    }
}