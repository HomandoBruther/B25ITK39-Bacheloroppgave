using UnityEngine;

public class PlayerRespawnHandler : MonoBehaviour
{
    Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Danger"))
        {
            Die();
        }
    }
    void Die()
    {
        Respawn();
    }

    void Respawn()
    {
        transform.position = startPos;
    }
}
