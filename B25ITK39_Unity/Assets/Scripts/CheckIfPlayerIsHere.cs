using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckIfPlayerIsHere : MonoBehaviour
{
    PlayerRespawnHandler respawnHandler;
    public Transform checkPoint;

    // Get the component from the bus
    private void Awake()
    {
        respawnHandler = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerRespawnHandler>();
    }

    // When player enters the trigger set a new location for respawn
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player has touched the zone");
            respawnHandler.UpdateCheckPoint(checkPoint);
        }
    }

}
