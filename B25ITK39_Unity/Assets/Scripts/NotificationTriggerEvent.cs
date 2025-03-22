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




    private void Awake()
    {


        arrow3D = FindObjectOfType<Arrow3DController>();
        isPickupZone = CompareTag("PickupZone");
        passengerPickup = GetComponent<PassengerPickup>();

        if (allPickupZones.Count == 0)
        {
            InitializePickupZones();
        }

        if (allDropOffZones.Count == 0)
        {
            InitializeDropOffZones();
        }

        if (isPickupZone)
        {
            if (nextPickup == null)
            {
                nextPickup = FindNextPickupZone();
            }
            HideAllPickupZonesExcept(nextPickup);
        }
    }


    private void InitializePickupZones()
    {
        allPickupZones = new List<GameObject>(GameObject.FindGameObjectsWithTag("PickupZone"));
        availablePickupZones = new List<GameObject>(allPickupZones);
    }

    private void InitializeDropOffZones()
    {
        allDropOffZones = new List<GameObject>(GameObject.FindGameObjectsWithTag("DropOffZone"));

        // Hide all drop-off zones at start
        HideAllDropOffZonesExcept(null);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (isPickupZone)
            {
                HandlePickup();
            }
            else
            {
                HandleDropOff();
            }
        }
    }

    private void HandlePickup()
    {

        if (passengerPickup == null)
        {
            Debug.LogError("❌ PassengerPickup script missing!");
            return;
        }


        int passengersPickedUp = passengerPickup.GetPassengerCount();
        PlayerData.PD.FillPassengers(passengersPickedUp);
        passengerPickup.ClearPassengers();

        nextDropOff = FindRandomZone("DropOffZone");

        string formattedDropOffName = FormatStopName(nextDropOff.name);

        notificationTextUI.text = $"{passengersPickedUp} passengers picked up!\nNext stop: {formattedDropOffName}";
        notificationAnim.Play("FadeIn");

        if (arrow3D != null) arrow3D.SetTarget(nextDropOff.transform);

        HideAllPickupZonesExcept(null);
        HideAllDropOffZonesExcept(nextDropOff);

        Invoke(nameof(FadeOutNotification), 5f);

        nextDropOff = FindRandomZone("DropOffZone");

        if (nextDropOff == null)
        {
            Debug.LogError("❌ No Drop-Off Zone assigned! Check if they exist in the scene.");
            return; // Prevent further errors
        }

    }

    private void HandleDropOff()
    {
        if (PlayerData.PD == null)
        {
            Debug.LogError("❌ PlayerData.PD is NULL! Check if PlayerData exists in the scene.");
            return;
        }

        int passengersDelivered = PlayerData.PD.currentPassengers;  // Read BEFORE ScorePoints() resets them
        int importantPassengersDelivered = PlayerData.PD.currentImportantPassengers;
        int scoreEarned = (passengersDelivered * 100) + (importantPassengersDelivered * 1000);

        Debug.Log($"Before Scoring - Passengers: {PlayerData.PD.currentPassengers}, Important: {PlayerData.PD.currentImportantPassengers}");


        PlayerData.PD.ScorePoints();  // Now it updates total points correctly

        Debug.Log($"Passengers Delivered: {passengersDelivered}, Important: {importantPassengersDelivered}, Score Earned: {scoreEarned}");

        nextPickup = FindNextPickupZone();

        if (nextPickup == null)
        {
            Debug.LogError("❌ No available PickupZone found!");
            notificationTextUI.text = $"{scoreEarned} points earned!\nNo available pickup zone.";
        }
        else
        {
            string formattedPickupName = FormatStopName(nextPickup.name);
            notificationTextUI.text = $"{scoreEarned} points earned!\nNext stop: {formattedPickupName}";
            if (arrow3D != null) arrow3D.SetTarget(nextPickup.transform);

            HideAllPickupZonesExcept(nextPickup);
            HideAllDropOffZonesExcept(null);
        }

        Debug.Log($"Final Notification Text: {notificationTextUI.text}"); // Debugging line

        notificationAnim.Play("FadeIn");
        Invoke(nameof(FadeOutNotification), 5f);
    }



    private string FormatStopName(string stopName)
    {
        return stopName switch
        {
            "StopStore" => "the Store",
            "StopHospital" => "the Hospital",
            "StopPowerPlant" => "the Power Plant",
            "StopPark" => "the Park",
            "PickupStore" => "the Store",
            "PickupHospital" => "the Hospital",
            "PickupPowerPlant" => "the Power Plant",
            "PickupPark" => "the Park",
            _ => stopName
        };
    }

    private GameObject FindRandomZone(string tag)
{
    GameObject[] allObjects = FindObjectsOfType<GameObject>(true); // Include inactive objects
    List<GameObject> dropOffZones = new List<GameObject>();

    foreach (GameObject obj in allObjects)
    {
        if (obj.CompareTag(tag)) // Manually check tag
        {
            dropOffZones.Add(obj);
        }
    }

    Debug.Log($"🔍 Manually found {dropOffZones.Count} objects with tag '{tag}' (including inactive)");

    return dropOffZones.Count > 0 ? dropOffZones[Random.Range(0, dropOffZones.Count)] : null;
}




    private GameObject FindNextPickupZone()
    {
        if (availablePickupZones.Count == 0)
        {
            availablePickupZones = new List<GameObject>(allPickupZones);
            if (availablePickupZones.Count == 0)
            {
                Debug.LogError("❌ No PickupZones available in the scene!");
                return null; // Prevent null reference issues
            }
        }

        int randomIndex = Random.Range(0, availablePickupZones.Count);
        GameObject selectedZone = availablePickupZones[randomIndex];
        availablePickupZones.RemoveAt(randomIndex);

        Debug.Log($"Next Pickup Zone: {selectedZone?.name ?? "None"}");

        return selectedZone;
    }


    private void HideAllPickupZonesExcept(GameObject activeZone)
    {
        foreach (GameObject zone in allPickupZones)
        {
            if (zone != null)
            {
                zone.SetActive(zone == activeZone);
            }
        }
    }

    private void HideAllDropOffZonesExcept(GameObject activeZone)
    {
        foreach (GameObject zone in allDropOffZones)
        {
            if (zone != null)
            {
                zone.SetActive(zone == activeZone); // Activate only the assigned drop-off
            }
        }
    }


    private void FadeOutNotification()
    {
        notificationAnim.Play("FadeOut");
    }
}