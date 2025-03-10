using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float projectileSpeed = 20f;
    [SerializeField] private float impactForce = 100f;
    [SerializeField] private float lifetime = 1f; // Safety destroy if it doesn't hit anything


    private void Start()
    {
        Destroy(gameObject, lifetime); // Destroy after 'lifetime' seconds to avoid clutter

    }

    void Update()
    {
        transform.position += transform.forward * projectileSpeed * Time.deltaTime;
    }


    private void OnTriggerEnter(Collider other)
    {
        RagdollEnabler enemy = other.GetComponent<RagdollEnabler>();

        if (enemy != null)
        {
            enemy.ragdollOnDeath();
        }

        // Destroy bullet on impact
        Destroy(gameObject);
    }


}
