using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class NotificationTriggerEvent : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI notificationTextUI;

    [Header("Icons")]
    [SerializeField] private Sprite pickupIcon;
    [SerializeField] private Sprite dropOffIcon;

    [Header("Animation")]
    [SerializeField] private Animator notificationAnim;

    private Arrow3DController arrow3D;
    private bool isPickupZone;
    private static GameObject nextDropOff;
    private static GameObject nextPickup;
    private static List<GameObject> allPickupZones = new List<GameObject>();
    private static List<GameObject> availablePickupZones = new List<GameObject>();
    private static List<GameObject> allDropOffZones = new List<GameObject>();
    private PassengerPickup passengerPickup;
    private CountdownTimer countdownTimer;

    // Initialize references and set up pickup/drop-off zones
    private void Awake()
    {
        arrow3D = FindObjectOfType<Arrow3DController>();
        isPickupZone = CompareTag("PickupZone");
        passengerPickup = GetComponent<PassengerPickup>();
        countdownTimer = FindObjectOfType<CountdownTimer>();

        if (allPickupZones.Count == 0) InitializePickupZones();
        if (allDropOffZones.Count == 0) InitializeDropOffZones();

        if (isPickupZone)
        {
            if (nextPickup == null) nextPickup = FindNextPickupZone();
            HideAllPickupZonesExcept(nextPickup);
        }

        if (arrow3D != null && nextPickup != null)
        {
            arrow3D.SetTarget(nextPickup.transform);
        }
    }

    // Populate all pickup zones
    private void InitializePickupZones()
    {
        allPickupZones = new List<GameObject>(GameObject.FindGameObjectsWithTag("PickupZone"));
        availablePickupZones = new List<GameObject>(allPickupZones);
    }

    // Populate all drop-off zones and hide them initially
    private void InitializeDropOffZones()
    {
        allDropOffZones = new List<GameObject>(GameObject.FindGameObjectsWithTag("DropOffZone"));
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

        nextDropOff = FindRandomDropOffZone(nextPickup);
        if (nextDropOff == null) return;

        float distanceToNextStop = Vector3.Distance(transform.position, nextDropOff.transform.position);
        countdownTimer?.StartCountdown(distanceToNextStop);

        notificationTextUI.text = $"{passengersPickedUp} passengers picked up!\nNext stop: {FormatStopName(nextDropOff.name)}";
        notificationAnim.Play("FadeIn");

        if (arrow3D != null) Invoke(nameof(UpdateArrowToDropOff), 0.1f);

        HideAllPickupZonesExcept(null);
        HideAllDropOffZonesExcept(nextDropOff);
        Invoke(nameof(FadeOutNotification), 5f);
    }

    // Updates arrow direction to drop-off location
    private void UpdateArrowToDropOff()
    {
        if (arrow3D != null) arrow3D.SetTarget(nextDropOff.transform);
    }

    // Handles logic when player reaches a drop-off zone
    private void HandleDropOff()
    {
        if (PlayerData.PD == null) return;

        int passengersDelivered = PlayerData.PD.currentPassengers;
        int importantPassengersDelivered = PlayerData.PD.currentImportantPassengers;
        int scoreEarned = (passengersDelivered * 100) + (importantPassengersDelivered * 1000);

        PlayerData.PD.ScorePoints();
        nextPickup = FindNextPickupZone();

        notificationTextUI.text = nextPickup == null
            ? $"{scoreEarned} points earned!\nNo available pickup zone."
            : $"{scoreEarned} points earned!\nNext stop: {FormatStopName(nextPickup.name)}";

        if (arrow3D != null) Invoke(nameof(UpdateArrowToPickup), 0.1f);

        HideAllPickupZonesExcept(nextPickup);
        HideAllDropOffZonesExcept(null);

        float distanceToNextStop = Vector3.Distance(transform.position, nextPickup.transform.position);
        countdownTimer?.StartCountdown(distanceToNextStop);

        notificationAnim.Play("FadeIn");
        Invoke(nameof(FadeOutNotification), 5f);
    }

    // Updates arrow direction to pickup location
    private void UpdateArrowToPickup()
    {
        if (arrow3D != null) arrow3D.SetTarget(nextPickup.transform);
    }

    // Formats zone names for UI display
    private string FormatStopName(string stopName)
    {
        return stopName switch
        {
            "StopStore" => "the Store",
            "StopHospital" => "the Hospital",
            "StopPowerPlant" => "the Power Plant",
            "StopPark" => "the Park",
            "StopDowntown" => "Downtown",
            "StopBank" => "the Bank",
            "StopStadium" => "the Stadium",
            "PickupStore" => "the Store",
            "PickupHospital" => "the Hospital",
            "PickupPowerPlant" => "the Power Plant",
            "PickupPark" => "the Park",
            "PickupDowntown" => "Downtown",
            "PickupStadium" => "Stadium",
            _ => stopName
        };
    }

    // Finds a random drop-off zone different from the last pickup
    private GameObject FindRandomDropOffZone(GameObject lastPickup)
    {
        List<GameObject> validDropOffs = allDropOffZones
            .Where(zone => lastPickup == null || !IsSameLocation(lastPickup, zone))
            .ToList();

        return validDropOffs.Count > 0 ? validDropOffs[Random.Range(0, validDropOffs.Count)] : null;
    }

    // Finds the next available pickup zone
    private GameObject FindNextPickupZone()
    {
        if (availablePickupZones.Count == 0) availablePickupZones = new List<GameObject>(allPickupZones);

        List<GameObject> validPickups = availablePickupZones
            .Where(zone => nextDropOff == null || !IsSameLocation(nextDropOff, zone))
            .ToList();

        if (validPickups.Count == 0) validPickups = new List<GameObject>(allPickupZones);

        GameObject selectedZone = validPickups[Random.Range(0, validPickups.Count)];
        availablePickupZones.Remove(selectedZone);
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
        foreach (GameObject zone in allPickupZones) zone?.SetActive(zone == activeZone);
    }

    // Hides all drop-off zones except the active one
    private void HideAllDropOffZonesExcept(GameObject activeZone)
    {
        foreach (GameObject zone in allDropOffZones) zone?.SetActive(zone == activeZone);
    }

    // Plays fade-out animation for notifications
    private void FadeOutNotification()
    {
        notificationAnim.Play("FadeOut");
    }
}
