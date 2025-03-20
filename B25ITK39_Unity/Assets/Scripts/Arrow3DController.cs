using UnityEngine;

public class Arrow3DController : MonoBehaviour
{
    public Transform target;  // The bus stop (drop-off location)
    public Transform player;  // The player (bus)

    private void Start()
    {
        // Find the player automatically if not assigned
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }
    }

    private void Update()
    {
        if (target == null || player == null) return;

        // Get direction to target
        Vector3 direction = (target.position - player.position).normalized;

        // Adjust rotation with an offset
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = targetRotation * Quaternion.Euler(90, 0, 0); // Adjust X-axis by 90 degrees
    }
    // Function to update the target dynamically
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
