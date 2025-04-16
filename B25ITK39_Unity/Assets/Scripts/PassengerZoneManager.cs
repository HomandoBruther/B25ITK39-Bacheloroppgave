using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PassengerZoneManager : MonoBehaviour
{
    public GameObject nextPickup;
    public GameObject nextDropOff;

    public List<GameObject> allPickupZones = new List<GameObject>();
    public List<GameObject> availablePickupZones = new List<GameObject>();
    public List<GameObject> allDropOffZones = new List<GameObject>();
}