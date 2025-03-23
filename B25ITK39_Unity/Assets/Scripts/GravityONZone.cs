using UnityEngine;

public class GravityONTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = true;
        }
    }
}
