using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class NotificationTriggerEvent : MonoBehaviour
{
    [Header("UI Content")]
    [SerializeField] private TMPro.TextMeshProUGUI notificationTextUI;
    [SerializeField] private Image characterIconUI;

    [Header("Message Customization")]
    [SerializeField] private Sprite yourIcon;

    [Header("Notification Removal")]
    [SerializeField] private bool removeAfterExit = false;
    [SerializeField] private bool disableAfterTimer = false;
    [SerializeField] private float disableTimer = 1.0f;

    [Header("Message Animation")]
    [SerializeField] private Animator notificationAnim;

    private BoxCollider objectCollider;
    private GameObject dropOffZone;

    // Pickup and drop-off zone management
    private GameObject[] allDropOffZones;

    private Arrow3DController arrow3D;
    // Mapping prefab names to display names
    private Dictionary<string, string> dropOffMappings = new Dictionary<string, string>()
    {
        { "StopHospital", "Hospital" },
        { "StopStore", "Store" },
        { "StopPowerPlant", "Power Plant" },
        { "StopPark", "Park" }
    };

    private void Awake()
    {
        objectCollider = gameObject.GetComponent<BoxCollider>();

        // Find all drop-off zones in the scene
        allDropOffZones = new GameObject[dropOffMappings.Count];

        arrow3D = FindObjectOfType<Arrow3DController>();

        int index = 0;
        foreach (string stopName in dropOffMappings.Keys)
        {
            GameObject stop = GameObject.Find(stopName);
            if (stop != null)
            {
                allDropOffZones[index] = stop;
                index++;
            }
        }

        AssignRandomDropOff();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Triggered by: " + other.gameObject.name);
        if (other.CompareTag("Player"))
        {
            gameObject.SetActive(true);
            AssignRandomDropOff(); // Choose a new drop-off point when triggered
            StartCoroutine(EnableNotification());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && removeAfterExit)
        {
            RemoveNotification();
        }
    }

    private IEnumerator EnableNotification()
    {
        objectCollider.enabled = false;
        notificationAnim.gameObject.SetActive(true);
        notificationAnim.Play("FadeIn");

        // Get passenger count
        int passengerCount = PlayerData.PD != null ? PlayerData.PD.currentPassengers : 0;

        // Get drop-off name
        string dropOffName = dropOffZone != null && dropOffMappings.ContainsKey(dropOffZone.name)
            ? dropOffMappings[dropOffZone.name]
            : "Unknown Location";

        // Update notification message for pickup
        notificationTextUI.text = $"{passengerCount} passengers picked up! Next stop: {dropOffName}";

        characterIconUI.sprite = yourIcon;

        // Disable all other drop-off points
        DisableAllDropOffZonesExcept(dropOffZone);

        if (disableAfterTimer)
        {
            yield return new WaitForSeconds(disableTimer);
            RemoveNotification();
        }
    }

    void RemoveNotification()
    {
        notificationAnim.Play("FadeOut");
        notificationAnim.gameObject.SetActive(false);
    }

    private void AssignRandomDropOff()
    {
        // Pick a random drop-off key from dictionary
        List<string> keys = new List<string>(dropOffMappings.Keys);
        string randomDropOffKey = keys[Random.Range(0, keys.Count)];

        // Find the GameObject in the scene
        dropOffZone = GameObject.Find(randomDropOffKey);

        if (dropOffZone == null)
        {
            Debug.LogWarning($"Drop-off point '{randomDropOffKey}' not found in the scene!");
        }
        if (arrow3D != null)
        {
            arrow3D.SetTarget(dropOffZone.transform);
        }
        else
        {
            Debug.Log($"Assigned drop-off point: {dropOffZone.name} ({dropOffMappings[randomDropOffKey]})");
        }
    }

    private void DisableAllDropOffZonesExcept(GameObject activeZone)
    {
        foreach (GameObject zone in allDropOffZones)
        {
            if (zone != null)
            {
                zone.SetActive(zone == activeZone); // Only enable the assigned drop-off point
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && dropOffZone != null)
        {
            int passengersDropped = PlayerData.PD.currentPassengers;
            PlayerData.PD.currentPassengers = 0; // Reset passengers to 0

            AssignRandomDropOff(); // Assign a new random drop-off point

            // Get new drop-off name
            string newDropOffName = dropOffZone != null && dropOffMappings.ContainsKey(dropOffZone.name)
                ? dropOffMappings[dropOffZone.name]
                : "Unknown Location";

            // Show drop-off notification
            notificationTextUI.text = $"{passengersDropped} passengers dropped off! Next stop: {newDropOffName}";
        }
    }
}
