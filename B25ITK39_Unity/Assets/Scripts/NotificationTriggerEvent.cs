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
        arrow3D = FindObjectOfType<Arrow3DController>(); // Find arrow script
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

        // ✅ Ensure arrow points to first pickup zone at game start
        if (arrow3D != null && nextPickup != null)
        {
            arrow3D.SetTarget(nextPickup.transform);
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

        // ✅ Select a drop-off zone that is NOT the same as the pickup zone
        nextDropOff = FindRandomDropOffZone(nextPickup); // 💡 This is the only assignment needed!

        if (nextDropOff == null)
        {
            Debug.LogError("❌ No Drop-Off Zone assigned! Check if they exist in the scene.");
            return;
        }

        string formattedDropOffName = FormatStopName(nextDropOff.name);
        notificationTextUI.text = $"{passengersPickedUp} passengers picked up!\nNext stop: {formattedDropOffName}";
        notificationAnim.Play("FadeIn");

        if (arrow3D != null && nextDropOff != null)
        {
            Debug.Log($"📌 Setting arrow to DROP-OFF: {nextDropOff.name} at {nextDropOff.transform.position}");
            Invoke(nameof(UpdateArrowToDropOff), 0.1f);
        }



        HideAllPickupZonesExcept(null);
        HideAllDropOffZonesExcept(nextDropOff);

        Invoke(nameof(FadeOutNotification), 5f);
    }

    private void UpdateArrowToDropOff()
    {
        if (arrow3D != null && nextDropOff != null)
        {
            arrow3D.SetTarget(nextDropOff.transform);
        }
    }


    private void HandleDropOff()
    {
        if (PlayerData.PD == null)
        {
            Debug.LogError("❌ PlayerData.PD is NULL! Check if PlayerData exists in the scene.");
            return;
        }

        int passengersDelivered = PlayerData.PD.currentPassengers;
        int importantPassengersDelivered = PlayerData.PD.currentImportantPassengers;
        int scoreEarned = (passengersDelivered * 100) + (importantPassengersDelivered * 1000);

        Debug.Log($"Before Scoring - Passengers: {PlayerData.PD.currentPassengers}, Important: {PlayerData.PD.currentImportantPassengers}");

        PlayerData.PD.ScorePoints();

        Debug.Log($"Passengers Delivered: {passengersDelivered}, Important: {importantPassengersDelivered}, Score Earned: {scoreEarned}");

        // ✅ Next pickup zone remains unchanged, ensuring it does not repeat
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

            if (arrow3D != null && nextPickup != null)
            {
                Debug.Log($"📌 Setting arrow to PICKUP: {nextPickup.name} at {nextPickup.transform.position}");
                Invoke(nameof(UpdateArrowToPickup), 0.1f);
            }


            HideAllPickupZonesExcept(nextPickup);
            HideAllDropOffZonesExcept(null);
        }

        Debug.Log($"Final Notification Text: {notificationTextUI.text}");

        notificationAnim.Play("FadeIn");
        Invoke(nameof(FadeOutNotification), 5f);
    }


    private void UpdateArrowToPickup()
    {
        if (arrow3D != null && nextPickup != null)
        {
            arrow3D.SetTarget(nextPickup.transform);
        }
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

    private GameObject FindRandomDropOffZone(GameObject lastPickup)
    {
        List<GameObject> validDropOffs = allDropOffZones
            .Where(zone => lastPickup == null || !IsSameLocation(lastPickup, zone))
            .ToList();

        if (validDropOffs.Count == 0)
        {
            Debug.LogError("❌ No valid Drop-Off Zones available!");
            return null;
        }

        GameObject selectedDropOff = validDropOffs[Random.Range(0, validDropOffs.Count)];
        Debug.Log($"🚏 Selected Drop-Off: {selectedDropOff.name}, Excluded: {lastPickup?.name ?? "None"}");
        return selectedDropOff;
    }






    private GameObject FindNextPickupZone()
    {
        if (availablePickupZones.Count == 0)
        {
            availablePickupZones = new List<GameObject>(allPickupZones);
        }

        List<GameObject> validPickups = availablePickupZones
            .Where(zone => nextDropOff == null || !IsSameLocation(nextDropOff, zone))
            .ToList();

        if (validPickups.Count == 0)
        {
            Debug.LogWarning("⚠️ No valid pickup zones available, using all pickup zones again.");
            validPickups = new List<GameObject>(allPickupZones);
        }

        int randomIndex = Random.Range(0, validPickups.Count);
        GameObject selectedZone = validPickups[randomIndex];
        availablePickupZones.Remove(selectedZone);

        Debug.Log($"Next Pickup Zone: {selectedZone?.name ?? "None"}");
        return selectedZone;
    }

    private bool IsSameLocation(GameObject zone1, GameObject zone2)
    {
        if (zone1 == null || zone2 == null) return false;

        string name1 = zone1.name.Replace("Pickup", "").Replace("Stop", "");
        string name2 = zone2.name.Replace("Pickup", "").Replace("Stop", "");

        return name1 == name2;
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