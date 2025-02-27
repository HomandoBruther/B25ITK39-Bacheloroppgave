using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float projectileSpeed = 20f;
    [SerializeField] private float lifetime = 5f; // Safety destroy if it doesn't hit anything

    private void Start()
    {
        Destroy(gameObject, lifetime); // Destroy after 'lifetime' seconds to avoid clutter
    }

    void Update()
    {
        transform.position += transform.forward * projectileSpeed * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Optional: Check for layers or tags to avoid hitting unwanted objects (like the turret itself)
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Environment"))
        {
            // You can add damage logic here if needed
            Destroy(gameObject); // Destroy projectile on impact
        }
    }
}
