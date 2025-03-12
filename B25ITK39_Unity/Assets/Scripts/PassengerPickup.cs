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

                Debug.Log("Passengers Picked Up: " + passengersAtStop); // Debugging

                foreach (GameObject passenger in spawnedPassengers)
                {
                    Destroy(passenger);
                }
                spawnedPassengers.Clear();

                if (PassengerPickupUI.instance != null)
                {
                    Debug.Log("Calling ShowPickupMessage()..."); // Debugging
                    PassengerPickupUI.instance.ShowPickupMessage(passengersAtStop);
                }
                else
                {
                    Debug.LogError("PassengerPickupUI.instance is NULL! Check if PickupPopupCanvas exists in GameLevel.");
                }

                gameObject.SetActive(false);
            }
        }
    }
}
