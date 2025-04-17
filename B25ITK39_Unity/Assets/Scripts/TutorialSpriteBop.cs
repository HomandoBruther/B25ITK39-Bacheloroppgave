using UnityEngine;

public class TutorialSpriteBop : MonoBehaviour
{
    public float amplitude = 0.2f;     // How much it moves up and down
    public float frequency = 1f;       // How fast it moves
    public float duration = 2f;        // How long to bop before stopping

    private Vector3 startPos;
    private float elapsedTime = 0f;
    private bool bopping = true;

    public bool enabled = true;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        if (bopping & enabled)
        {
            elapsedTime += Time.deltaTime;

            // Sin wave motion
            float offsetY = Mathf.Sin(elapsedTime * Mathf.PI * frequency) * amplitude;
            transform.position = startPos + new Vector3(0f, offsetY, 0f);

            if (elapsedTime >= duration)
            {
                // End bop and reset position
                bopping = false;
                transform.position = startPos;
            }
        }
    }
}