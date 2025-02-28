using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float projectileSpeed = 20f;
    [SerializeField] private float impactForce = 100f;
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
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Try to apply force to the ragdoll
            RagdollEnabler ragdoll = collision.gameObject.GetComponentInParent<RagdollEnabler>();
            if (ragdoll != null)
            {
                ragdoll.EnableRagdoll(); // Activate Ragdoll
                ApplyForceToRagdoll(ragdoll, collision);
            }

            Destroy(gameObject); // Destroy the projectile
        }
    }

    private void ApplyForceToRagdoll(RagdollEnabler ragdoll, Collision collision)
    {
        Rigidbody hitRigidbody = collision.rigidbody; // Get hit limb's Rigidbody
        if (hitRigidbody != null)
        {
            Vector3 forceDirection = collision.contacts[0].point - transform.position;
            forceDirection.Normalize();

            hitRigidbody.AddForce(forceDirection * impactForce, ForceMode.Impulse);
        }
    }
}
