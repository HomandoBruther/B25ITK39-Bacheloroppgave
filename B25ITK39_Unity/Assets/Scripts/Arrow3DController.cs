using UnityEngine;

public class Arrow3DController : MonoBehaviour
{
    public Transform target;  // The bus stop (drop-off location)
    public Transform player;  // The player (bus)
    public float heightAboveBus = 5f; // Height adjustment for the arrow

    private void Start()
    {
        // Automatically find the player if not assigned
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }
    }

    private void Update()
    {
        if (player == null) return;

        // Keep the arrow positioned above the player
        transform.position = player.position + Vector3.up * heightAboveBus;

        if (target != null)
        {
            // Rotate the arrow to always face the target
            Vector3 direction = target.position - transform.position;
            direction.y = 0; // Keep rotation level

            if (direction.sqrMagnitude > 0.01f)
            {
                transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
            }
        }
    }

    public void SetTarget(Transform newTarget)
    {
        // Assign a new target for the arrow to point to
        if (newTarget == null) return;
        target = newTarget;
    }
}
