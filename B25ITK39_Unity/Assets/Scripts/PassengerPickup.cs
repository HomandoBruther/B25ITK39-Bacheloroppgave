using UnityEngine;
using System.Collections.Generic;

public class PassengerPickup : MonoBehaviour
{
    public GameObject passengerPrefab;
    public int minPassengers = 1;
    public int maxPassengers = 5;

    private List<GameObject> spawnedPassengers = new List<GameObject>();
    private int passengersAtStop;

    private void Start()
    {
        // Randomly determine the number of passengers at this stop
        passengersAtStop = Random.Range(minPassengers, maxPassengers + 1);
        SpawnPassengers();
    }

    private void SpawnPassengers()
    {
        // Spawns passengers within the bounds of the stop's trigger area
        Collider triggerCollider = GetComponent<Collider>();
        if (triggerCollider == null) return;

        float minX = triggerCollider.bounds.min.x;
        float maxX = triggerCollider.bounds.max.x;
        float minZ = triggerCollider.bounds.min.z;
        float maxZ = triggerCollider.bounds.max.z;
        float spawnHeight = triggerCollider.bounds.min.y;

        for (int i = 0; i < passengersAtStop; i++)
        {
            Vector3 spawnPosition = new Vector3(Random.Range(minX, maxX), spawnHeight, Random.Range(minZ, maxZ));
            GameObject passenger = Instantiate(passengerPrefab, spawnPosition, Quaternion.identity);
            spawnedPassengers.Add(passenger);
        }
    }

    public int GetPassengerCount()
    {
        // Returns the number of passengers at the stop
        return passengersAtStop;
    }

    public void ClearPassengers()
    {
        // Removes all spawned passengers from the scene
        foreach (GameObject passenger in spawnedPassengers)
        {
            if (passenger != null)
            {
                Destroy(passenger);
            }
        }
        spawnedPassengers.Clear();
    }
}
