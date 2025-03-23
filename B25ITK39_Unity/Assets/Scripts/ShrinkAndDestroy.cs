using System.Collections;
using UnityEngine;

public class ShrinkAndDestroy : MonoBehaviour
{
    public float shrinkDelay = 20f; // Time before shrinking starts
    public float shrinkSpeed = 0.5f; // Speed of shrinking
    public float minSize = 0.1f; // Minimum scale before destruction

    private bool isShrinking = false;

    void Start()
    {
        StartCoroutine(StartShrinking());
    }

    IEnumerator StartShrinking()
    {
        yield return new WaitForSeconds(shrinkDelay);
        isShrinking = true;
    }

    void Update()
    {
        if (isShrinking)
        {
            transform.localScale *= (1 - shrinkSpeed * Time.deltaTime);
            if (transform.localScale.magnitude < minSize)
            {
                Destroy(gameObject);
            }
        }
    }
}