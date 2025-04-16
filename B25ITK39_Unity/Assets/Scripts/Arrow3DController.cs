using UnityEngine;
using System.Collections;

public class Arrow3DController : MonoBehaviour
{
    public Transform target;  // The bus stop (drop-off location)
    public Transform player;  // The player (bus)
    public float heightAboveBus = 5f;

    [Header("Bobbing Settings")]
    public float bobbingAmplitude = 0.5f;
    public float bobbingFrequency = 2f;

    [Header("Pulsing Settings")]
    public float pulsingAmplitude = 0.2f;
    public float pulsingFrequency = 3f;

    [Header("Intro Spin Settings")]
    public float spinDuration = 1f; // Duration of full 360 spin in seconds
    public float spins = 2f;

    private Vector3 initialScale;
    private float initialY;
    private bool introSpinComplete = false;

    private void Start()
    {
        // Auto-assign the player
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }

        initialY = heightAboveBus;
        initialScale = transform.localScale;

        StartCoroutine(PlayIntroSpin());
    }

    private IEnumerator PlayIntroSpin()
{
    float elapsed = 0f;
    float totalAngle = 360f * spins;

    while (elapsed < spinDuration)
    {
        float deltaAngle = (Time.deltaTime / spinDuration) * totalAngle;
        transform.Rotate(Vector3.up, deltaAngle, Space.World);
        elapsed += Time.deltaTime;
        yield return null;
    }

    introSpinComplete = true;
}

    private void Update()
    {
        if (player == null) return;

        // Bobbing movement
        float bobbingOffset = Mathf.Sin(Time.time * bobbingFrequency) * bobbingAmplitude;
        transform.position = player.position + Vector3.up * (initialY + bobbingOffset);

        // Pulsing scale
        float pulse = (Mathf.Sin(Time.time * pulsingFrequency) * 0.5f + 0.5f);
        float scaleMultiplier = 1 + pulse * pulsingAmplitude;
        transform.localScale = initialScale * scaleMultiplier;

        // Only point at the target after the intro spin is complete
        if (introSpinComplete && target != null)
        {
            Vector3 direction = target.position - transform.position;
            direction.y = 0;

            if (direction.sqrMagnitude > 0.01f)
            {
                transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
            }
        }
    }

    public void SetTarget(Transform newTarget)
    {
        if (newTarget == null) return;
        target = newTarget;
    }
}