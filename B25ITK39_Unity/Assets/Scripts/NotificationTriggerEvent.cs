using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System;

public class NotificationTriggerEvent : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] public TextMeshProUGUI notificationTextUI;

    [Header("Icons")]
    [SerializeField] private Sprite pickupIcon;
    [SerializeField] private Sprite dropOffIcon;

    [Header("Animation")]
    [SerializeField] public Animator notificationAnim;

    private Arrow3DController arrow3D;
    private bool isPickupZone;

    
    private PassengerPickup passengerPickup;
    private CountdownTimer countdownTimer;
    private PassengerZoneManager zoneManager;

    // Initialize references and set up pickup/drop-off zones
    private void Awake()
    {
        
        zoneManager = FindObjectOfType<PassengerZoneManager>();

        

        arrow3D = FindObjectOfType<Arrow3DController>();
        isPickupZone = CompareTag("PickupZone");
        passengerPickup = GetComponent<PassengerPickup>();
        countdownTimer = FindObjectOfType<CountdownTimer>();

        if (zoneManager.allPickupZones.Count == 0) {
            Debug.Log("Initializing pickup zones");
            InitializePickupZones();
            }
        if (zoneManager.allDropOffZones.Count == 0) {
            Debug.Log("Initializing Dropoff zones");
            InitializeDropOffZones();
        }

        if (isPickupZone)
        {
            if (zoneManager.nextPickup == null) zoneManager.nextPickup = FindNextPickupZone();
            HideAllPickupZonesExcept(zoneManager.nextPickup);
        }

        if (arrow3D != null && zoneManager.nextPickup != null)
        {
            arrow3D.SetTarget(zoneManager.nextPickup.transform);
        }
    }

    // Populate all pickup zones
    private void InitializePickupZones()
    {
        zoneManager.allPickupZones = new List<GameObject>(GameObject.FindGameObjectsWithTag("PickupZone"));
        zoneManager.availablePickupZones = new List<GameObject>(zoneManager.allPickupZones);
    }

    // Populate all drop-off zones and hide them initially
    private void InitializeDropOffZones()
    {
        zoneManager.allDropOffZones = new List<GameObject>(GameObject.FindGameObjectsWithTag("DropOffZone"));
        HideAllDropOffZonesExcept(null);
    }

    // Trigger event when player enters a pickup or drop-off zone
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (isPickupZone) HandlePickup();
            else HandleDropOff();
        }
    }

    // Handles logic when player reaches a pickup zone
    private void HandlePickup()
    {
        if (passengerPickup == null) return;

        int passengersPickedUp = passengerPickup.GetPassengerCount();
        PlayerData.PD.FillPassengers(passengersPickedUp);
        passengerPickup.ClearPassengers();

        zoneManager.nextDropOff = FindRandomDropOffZone(zoneManager.nextPickup);
        if (zoneManager.nextDropOff == null) return;

        float distanceToNextStop = Vector3.Distance(transform.position, zoneManager.nextDropOff.transform.position);
        countdownTimer?.StartCountdown(distanceToNextStop);

        notificationTextUI.text = $"{passengersPickedUp} passengers picked up!\nNext stop: {FormatStopName(zoneManager.nextDropOff.name)}";
        notificationAnim.Play("FadeIn");


        if (arrow3D != null) Invoke(nameof(UpdateArrowToDropOff), 0.1f);

        HideAllPickupZonesExcept(null);
        HideAllDropOffZonesExcept(zoneManager.nextDropOff);
        Invoke(nameof(FadeOutNotification), 5f);
    }

    // Updates arrow direction to drop-off location
    private void UpdateArrowToDropOff()
    {
        if (arrow3D != null) arrow3D.SetTarget(zoneManager.nextDropOff.transform);
    }

    // Handles logic when player reaches a drop-off zone
    private void HandleDropOff()
    {
        if (PlayerData.PD == null) return;

        int passengersDelivered = PlayerData.PD.currentPassengers;
        int importantPassengersDelivered = PlayerData.PD.currentImportantPassengers;
        int scoreEarned = (passengersDelivered * 100) + (importantPassengersDelivered * 1000);

        PlayerData.PD.ScorePoints();
        zoneManager.nextPickup = FindNextPickupZone();

        notificationTextUI.text = zoneManager.nextPickup == null
            ? $"{scoreEarned} points earned!\nNo available pickup zone."
            : $"{scoreEarned} points earned!\nNext stop: {FormatStopName(zoneManager.nextPickup.name)}";

        if (arrow3D != null) Invoke(nameof(UpdateArrowToPickup), 0.1f);

        HideAllPickupZonesExcept(zoneManager.nextPickup);
        HideAllDropOffZonesExcept(null);

        float distanceToNextStop = Vector3.Distance(transform.position, zoneManager.nextPickup.transform.position);
        countdownTimer?.StartCountdown(distanceToNextStop);

        notificationAnim.Play("FadeIn");
        Invoke(nameof(FadeOutNotification), 5f);
    }

    // Updates arrow direction to pickup location
    private void UpdateArrowToPickup()
    {
        if (arrow3D != null) arrow3D.SetTarget(zoneManager.nextPickup.transform);
    }

    // Formats zone names for UI display
    public string FormatStopName(string stopName)
    {
        return stopName switch
        {
            "StopStore" => "the Store",
            "StopHospital" => "the Hospital",
            "StopPowerPlant" => "the Power Plant",
            "StopPark" => "the Park",
            "StopDowntown" => "Downtown",
            //"StopBank" => "the Bank",
            //"StopStadium" => "the Stadium",
            "PickupStore" => "the Store",
            "PickupHospital" => "the Hospital",
            "PickupPowerPlant" => "the Power Plant",
            "PickupPark" => "the Park",
            "PickupDowntown" => "Downtown",
            //"PickupBank" => "The Bank",
            //"PickupStadium" => "Stadium",
            _ => stopName
        };
    }

    // Finds a random drop-off zone different from the last pickup
    private GameObject FindRandomDropOffZone(GameObject lastPickup)
    {
        List<GameObject> validDropOffs = zoneManager.allDropOffZones
            .Where(zone => lastPickup == null || !IsSameLocation(lastPickup, zone))
            .ToList();

        return validDropOffs.Count > 0 ? validDropOffs[UnityEngine.Random.Range(0, validDropOffs.Count)] : null;
    }

    // Finds the next available pickup zone
    private GameObject FindNextPickupZone()
    {
        if (zoneManager.availablePickupZones.Count == 0) zoneManager.availablePickupZones = new List<GameObject>(zoneManager.allPickupZones);

        List<GameObject> validPickups = zoneManager.availablePickupZones
            .Where(zone => zoneManager.nextDropOff == null || !IsSameLocation(zoneManager.nextDropOff, zone))
            .ToList();

        if (validPickups.Count == 0) validPickups = new List<GameObject>(zoneManager.allPickupZones);

        GameObject selectedZone = validPickups[UnityEngine.Random.Range(0, validPickups.Count)];
        zoneManager.availablePickupZones.Remove(selectedZone);
        return selectedZone;
    }

    // Checks if two zones refer to the same location
    private bool IsSameLocation(GameObject zone1, GameObject zone2)
    {
        if (zone1 == null || zone2 == null) return false;

        string name1 = zone1.name.Replace("Pickup", "").Replace("Stop", "");
        string name2 = zone2.name.Replace("Pickup", "").Replace("Stop", "");

        return name1 == name2;
    }

    // Hides all pickup zones except the active one
    private void HideAllPickupZonesExcept(GameObject activeZone)
    {
        foreach (GameObject zone in zoneManager.allPickupZones) zone?.SetActive(zone == activeZone);
    }

    // Hides all drop-off zones except the active one
    private void HideAllDropOffZonesExcept(GameObject activeZone)
    {
        foreach (GameObject zone in zoneManager.allDropOffZones) zone?.SetActive(zone == activeZone);
    }

    // Plays fade-out animation for notifications
    private void FadeOutNotification()
    {
        notificationAnim.Play("FadeOut");
    }
}
