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
        // Get the Collider of the trigger zone
        Collider triggerCollider = GetComponent<Collider>();

        // Calculate the bottom of the trigger zone (y-position of the bottom)
        float spawnHeight = triggerCollider.bounds.min.y;

        for (int i = 0; i < passengersAtStop; i++)
        {
            // Offset the spawn position based on the trigger's bounds
            Vector3 spawnPosition = new Vector3(
                transform.position.x + (i * 1.5f),  // Offset for spacing
                spawnHeight,  // Spawn at the bottom of the trigger zone
                transform.position.z
            );

            // Instantiate the passenger prefab
            GameObject passenger = Instantiate(passengerPrefab, spawnPosition, Quaternion.identity);

            // Make sure the passenger doesn't collide with the car
            if (passenger.TryGetComponent<Collider>(out Collider collider))
            {
                collider.isTrigger = true;  // Set the passenger to be a trigger
            }

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
                car.PickupPassengers(passengersAtStop);
                pickedUp = true;

                // Destroy all spawned passengers
                foreach (GameObject passenger in spawnedPassengers)
                {
                    Destroy(passenger);
                }
                spawnedPassengers.Clear();

                Debug.Log(passengersAtStop + " passengers picked up!");
                gameObject.SetActive(false);  // Disable pickup point after use
            }
        }
    }
}
