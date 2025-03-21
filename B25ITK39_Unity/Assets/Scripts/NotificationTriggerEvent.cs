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
    private static List<GameObject> availablePickupZones = new List<GameObject>(); // Controls randomized order

    private void Awake()
    {
        arrow3D = FindObjectOfType<Arrow3DController>();
        isPickupZone = CompareTag("PickupZone");

        // Only run pickup zone initialization ONCE
        if (allPickupZones.Count == 0)
        {
            InitializePickupZones();
        }

        if (isPickupZone)
        {
            if (nextPickup == null)
            {
                nextPickup = FindNextPickupZone();
            }

            // Hide all pickup zones except the first one
            HideAllPickupZonesExcept(nextPickup);
        }
    }

    private void InitializePickupZones()
    {
        allPickupZones = new List<GameObject>(GameObject.FindGameObjectsWithTag("PickupZone"));
        availablePickupZones = new List<GameObject>(allPickupZones); // Reset pickup cycle
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
        int passengersPickedUp = Random.Range(1, 5);
        PlayerData.PD.FillPassengers(passengersPickedUp);

        nextDropOff = FindRandomZone("DropOffZone");

        string formattedDropOffName = FormatStopName(nextDropOff.name);

        notificationTextUI.text = $"{passengersPickedUp} passengers picked up!\nNext stop: {formattedDropOffName}";
        notificationAnim.Play("FadeIn");

        if (arrow3D != null) arrow3D.SetTarget(nextDropOff.transform);

        HideAllPickupZonesExcept(null);

        Invoke(nameof(FadeOutNotification), 5f);
    }


    private void HandleDropOff()
    {
        int scoreEarned = PlayerData.PD.ScorePoints();

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
        }

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
            _ => stopName // Default: return original name
        };
    }

    private GameObject FindRandomZone(string tag)
    {
        GameObject[] zones = GameObject.FindGameObjectsWithTag(tag);
        return zones.Length > 0 ? zones[Random.Range(0, zones.Length)] : null;
    }

    private GameObject FindNextPickupZone()
    {
        // If all zones have been used, reset the cycle
        if (availablePickupZones.Count == 0)
        {
            availablePickupZones = new List<GameObject>(allPickupZones);
        }

        // Pick a random zone from the available ones
        int randomIndex = Random.Range(0, availablePickupZones.Count);
        GameObject selectedZone = availablePickupZones[randomIndex];

        // Remove the selected zone from the list (so it's not chosen again until reset)
        availablePickupZones.RemoveAt(randomIndex);

        Debug.Log($"Next Pickup Zone: {selectedZone.name}"); // Debugging line to verify selection

        return selectedZone;
    }

    private void HideAllPickupZonesExcept(GameObject activeZone)
    {
        foreach (GameObject zone in allPickupZones)
        {
            if (zone != null)
            {
                zone.SetActive(zone == activeZone); // Only keep one active
            }
        }
    }

    private void FadeOutNotification()
    {
        notificationAnim.Play("FadeOut");
    }
}
