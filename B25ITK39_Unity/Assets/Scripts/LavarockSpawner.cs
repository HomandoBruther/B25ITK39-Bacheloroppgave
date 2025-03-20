using UnityEngine;
using System.Collections;

// A Script that is used by the volcano to spawn balls or "Lavarocks" at random intervals.
public class LavarockSpawner : MonoBehaviour
{
    public bool enableSpawn = true;
    public GameObject[] prefabList; // Assign the prefab in Inspector
    private GameObject prefab;
    public float radius = 5.0f; // Same as the Gizmo radius
    public float maxUpwardForce = 1000f; // Strength of the upward force
    public float maxOutwardForce = 1000f; // Strength of the outward force
    public float maxLavaRockSpawnTime = 0.01f; // Max time between spawns
    public float maxSpawnTime = 60f; // Max delay before spawning starts
    public float maxEndSpawnTime = 5f; // Max duration before stopping
    

    private Coroutine spawnCoroutine; // Store the spawning coroutine
    private bool spawnRocks = false;

    private AudioSource audioSource;

    public AudioSource[] audioSourceList;

    private void Start()
    {
        if (enableSpawn)
            StartCoroutine(StartSpawning());
    }

    private IEnumerator StartSpawning()
    {
        while (true)
        {
            float initialDelay = Random.Range(0f, maxSpawnTime);
            yield return new WaitForSeconds(initialDelay); // Wait before spawning starts

            spawnRocks = true;

            // Start the spawn coroutine only if it's not running
            if (spawnCoroutine == null)
                spawnCoroutine = StartCoroutine(SpawnRoutine());


            float spawnDuration = Random.Range(2f, maxEndSpawnTime);
            yield return new WaitForSeconds(spawnDuration); // Wait before stopping

            spawnRocks = false; // Stop spawning

            // Stop the coroutine and reset it
            if (spawnCoroutine != null)
            {
                StopCoroutine(spawnCoroutine);
                spawnCoroutine = null;
            }
        }
    }

    private IEnumerator SpawnRoutine()
    {

        int randomNumber = Random.Range(0, audioSourceList.Length);

        audioSource = audioSourceList[randomNumber];

        audioSource.PlayOneShot(audioSource.clip, 0.3f);
        while (spawnRocks)
        {
            float waitTime = Random.Range(0f, maxLavaRockSpawnTime);
            yield return new WaitForSeconds(waitTime);

            SpawnPrefab();
        }
    }

    private void SpawnPrefab()
    {
        int randomNumber = Random.Range(0, prefabList.Length);

        prefab = prefabList[randomNumber];
        if (prefab != null)
        {
            float prefabRadius = GetPrefabRadius(prefab);
            Vector3 spawnPos = GetSafeSpawnPoint(prefabRadius);

            GameObject spawnedPrefab = Instantiate(prefab, spawnPos, Quaternion.identity);

            Rigidbody rb = spawnedPrefab.GetComponent<Rigidbody>();
            if (rb != null)
            {
                float outwardForce = Random.Range(maxOutwardForce / 2, maxOutwardForce);
                float upwardForce = Random.Range(maxUpwardForce / 2, maxUpwardForce);
                Vector3 direction = (spawnPos - transform.position).normalized; // Outward direction
                Vector3 force = (direction * outwardForce) + (Vector3.up * upwardForce); // Combined force
                rb.AddForce(force, ForceMode.Impulse);
            }
            else
            {
                Debug.LogWarning("Spawned prefab has no Rigidbody! It won’t move.");
            }
        }
        else
        {
            Debug.LogWarning("Prefab not assigned!");
        }
    }

    private Vector3 GetSafeSpawnPoint(float prefabRadius)
    {
        float safeRadius = Mathf.Max(0, radius - prefabRadius); // Ensure non-negative radius

        if (safeRadius == 0)
        {
            return transform.position; // If the prefab is too big, spawn at center
        }

        Vector2 randomCircle = Random.insideUnitCircle * safeRadius; // Random point in safe area
        Vector3 randomPos = new Vector3(randomCircle.x, 0, randomCircle.y); // Convert to 3D (XZ plane)

        return transform.position + randomPos; // Offset from the Gizmo center
    }

    private float GetPrefabRadius(GameObject obj)
    {
        Collider collider = obj.GetComponent<Collider>();

        if (collider is SphereCollider sphere)
        {
            return sphere.radius * Mathf.Max(obj.transform.lossyScale.x, obj.transform.lossyScale.y, obj.transform.lossyScale.z);
        }
        else if (collider is BoxCollider box)
        {
            Vector3 scaledSize = Vector3.Scale(box.size, obj.transform.lossyScale);
            return Mathf.Max(scaledSize.x, scaledSize.y, scaledSize.z) / 2f; // Use the largest half-size
        }
        else if (collider is CapsuleCollider capsule)
        {
            return capsule.radius * Mathf.Max(obj.transform.lossyScale.x, obj.transform.lossyScale.z);
        }
        else if (collider is MeshCollider meshCollider && meshCollider.sharedMesh != null)
        {
            Bounds meshBounds = meshCollider.sharedMesh.bounds;
            Vector3 scaledExtents = Vector3.Scale(meshBounds.extents, obj.transform.lossyScale);
            return Mathf.Max(scaledExtents.x, scaledExtents.y, scaledExtents.z); // Use largest extent
        }

        Debug.LogWarning("Prefab has no valid Collider! Defaulting to small radius.");
        return 0.2f; // Default fallback if no collider is found
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius); // Outer spawn area
    }
}