using UnityEngine;

public class Arrow3DController : MonoBehaviour
{
    public Transform target;  // The bus stop (drop-off location)
    public Transform player;  // The player (bus)
    public float heightAboveBus = 5f; // Adjust height of arrow above bus

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
        if (player == null) return;

        // 🔥 Ensure the arrow stays above the bus at all times
        transform.position = player.position + Vector3.up * heightAboveBus;

        if (target != null)
        {
            // 🔄 Point towards the target (bus stop)
            Vector3 direction = target.position - transform.position;
            direction.y = 0; // Keep it level

            if (direction.sqrMagnitude > 0.01f)
            {
                transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
            }
        }
    }

    public void SetTarget(Transform newTarget)
    {
        if (newTarget == null)
        {
            Debug.LogError("❌ Attempted to set arrow target to NULL!");
            return;
        }

        target = newTarget;
        Debug.Log($"🔄 Arrow target updated: {target.name} at position {target.position}");
    }
}
