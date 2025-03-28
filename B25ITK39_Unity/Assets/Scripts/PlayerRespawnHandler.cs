using System.Collections;
using UnityEngine;

public class PlayerRespawnHandler : MonoBehaviour
{
    Vector3 startPos;
    private GameObject player;
    private Rigidbody rb;
    public Transform Respawn_Point_0;

    public void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    public void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player OnTriggerEnter activated - Next die");
            Die();
        }
    }
    public void Die()
    {
        Debug.Log("Die activated, next respawn");
        player.SetActive(false);
        player.transform.localRotation = Respawn_Point_0.localRotation;
        player.transform.localPosition = Respawn_Point_0.localPosition;
        StartCoroutine(Respawn(5));
        Debug.Log("Countdown finnished ...");

    }

    public IEnumerator Respawn(int secs)
    {
        yield return new WaitForSeconds(secs);
        Debug.Log("Respawn activated, you should now have been teleported to");
        player.transform.position = Respawn_Point_0.transform.position;
        player.SetActive(true);
    }
}
