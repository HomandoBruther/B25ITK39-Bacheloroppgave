using UnityEngine;
using System.Collections.Generic;

public class PassengerPickup : MonoBehaviour
{
    public GameObject passengerPrefab;
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
        Collider triggerCollider = GetComponent<Collider>();

        float minX = triggerCollider.bounds.min.x;
        float maxX = triggerCollider.bounds.max.x;
        float minZ = triggerCollider.bounds.min.z;
        float maxZ = triggerCollider.bounds.max.z;
        float spawnHeight = triggerCollider.bounds.min.y;

        for (int i = 0; i < passengersAtStop; i++)
        {
            float randomX = Random.Range(minX, maxX);
            float randomZ = Random.Range(minZ, maxZ);
            Vector3 spawnPosition = new Vector3(randomX, spawnHeight, randomZ);

            GameObject passenger = Instantiate(passengerPrefab, spawnPosition, Quaternion.identity);
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

                gameObject.SetActive(false);
            }
        }
    }
    public void ResetPickupZone()
    {
        pickedUp = false;
        passengersAtStop = Random.Range(minPassengers, maxPassengers + 1);
        SpawnPassengers();
        gameObject.SetActive(true);

        Debug.Log($"Pickup zone {gameObject.name} reset with {passengersAtStop} new passengers.");
    }

}
