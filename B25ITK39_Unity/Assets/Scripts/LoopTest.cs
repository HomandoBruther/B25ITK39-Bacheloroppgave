using UnityEngine;
using Unity.Cinemachine;

public class LoopDeLoopHandler : MonoBehaviour
{
    [Header("Sticky Force Settings")]
    public float stickyForce = 30f;

    [Header("Cinemachine Settings")]
    public CinemachineCamera loopCamera;

    private Rigidbody busRb;
    private bool insideLoop = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Make sure your bus has the "Player" tag
        {
            busRb = other.GetComponent<Rigidbody>();

            /*
            if (busRb != null)
            {
                insideLoop = true;
                if (loopCamera != null)
                {
                    loopCamera.Priority = 20;  // Ensure loop camera takes over
                }
            }
            */
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            insideLoop = false;
            if (loopCamera != null)
            {
                loopCamera.Priority = 0;  // Return control to the default camera
            }
        }
    }

    private void FixedUpdate()
    {
        if (insideLoop && busRb != null)
        {
            // Apply sticky force to keep the bus pressed against the loop
            Vector3 localUp = -busRb.transform.up;
            busRb.AddForce(localUp * stickyForce, ForceMode.Force);
        }
    }
}
