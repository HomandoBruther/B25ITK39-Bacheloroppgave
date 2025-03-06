using UnityEngine;
using System.Collections.Generic;

public class PassengerPickup : MonoBehaviour
{
    public GameObject passengerPrefab;  // Assign in Inspector
    public int minPassengers = 1;
    public int maxPassengers = 5;

    private List<GameObject> spawnedPassengers = new List<GameObject>();
    private int passengersAtStop;
    private bool pickedUp = false;

    private void Start()
    {
        passengersAtStop = Random.Range(minPassengers, maxPassengers + 1);
        SpawnPassengers();
    }

    private void SpawnPassengers()
    {
        Collider triggerCollider = GetComponent<Collider>(); // Get trigger box collider

        float minX = triggerCollider.bounds.min.x;
        float maxX = triggerCollider.bounds.max.x;
        float minZ = triggerCollider.bounds.min.z;
        float maxZ = triggerCollider.bounds.max.z;
        float spawnHeight = triggerCollider.bounds.min.y; // Ensure they spawn at ground level

        for (int i = 0; i < passengersAtStop; i++)
        {
            // Generate a random position inside the trigger box
            float randomX = Random.Range(minX, maxX);
            float randomZ = Random.Range(minZ, maxZ);
            Vector3 spawnPosition = new Vector3(randomX, spawnHeight, randomZ);

            // Instantiate the studentTest prefab
            GameObject passenger = Instantiate(passengerPrefab, spawnPosition, Quaternion.identity);

            // Add to the list of spawned passengers
            spawnedPassengers.Add(passenger);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !pickedUp)
        {
            CarController car = other.GetComponent<CarController>();
            if (car != null)
            {
                int leftover = PlayerData.PD.FillPassengers(passengersAtStop);
                pickedUp = true;

                // Destroy all spawned passengers
                foreach (GameObject passenger in spawnedPassengers)
                {
                    Destroy(passenger);
                }
                spawnedPassengers.Clear();

                Debug.Log(passengersAtStop + " passengers picked up!");
                if (leftover > 0)
                {
                    Debug.Log(leftover + " passengers couldn't fit in the vehicle!");
                }

                gameObject.SetActive(false);  // Disable pickup point after use
            }
        }
    }

}
