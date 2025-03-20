using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class NotificationTriggerEvent : MonoBehaviour
{
    [Header("UI Content")]
    [SerializeField] private TextMeshProUGUI notificationTextUI;
    [SerializeField] private Image characterIconUI;

    [Header("Message Customization")]
    [SerializeField] private Sprite pickupIcon;
    [SerializeField] private Sprite dropOffIcon;

    [Header("Notification Removal")]
    [SerializeField] private bool removeAfterExit = false;
    [SerializeField] private bool disableAfterTimer = false;
    [SerializeField] private float disableTimer = 1.0f;

    [Header("Message Animation")]
    [SerializeField] private Animator notificationAnim;

    private BoxCollider objectCollider;
    private GameObject dropOffZone;
    private bool isPickupZone;

    private Arrow3DController arrow3D;

    private void Awake()
    {
        objectCollider = GetComponent<BoxCollider>();
        arrow3D = FindObjectOfType<Arrow3DController>();

        GameObject[] allGameObjects = FindObjectsOfType<GameObject>();
        Debug.Log($"🔍 Total GameObjects in Scene: {allGameObjects.Length}");

        foreach (var obj in allGameObjects)
        {
            Debug.Log($"📌 Found Object: {obj.name} | Tag: {obj.tag}");
        }

        GameObject[] dropOffZones = GameObject.FindGameObjectsWithTag("DropOffZone");

        if (dropOffZones.Length == 0)
        {
            Debug.LogError("🚨 No available drop-off zones found! Check your Hierarchy and Tags.");
        }
        else
        {
            foreach (var zone in dropOffZones)
            {
                Debug.Log($"✅ Found DropOffZone: {zone.name}");
            }
        }

        isPickupZone = CompareTag("PickupZone");
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameObject.SetActive(true);

            if (isPickupZone)
            {
                AssignRandomDropOff();
                StartCoroutine(EnablePickupNotification());
            }
            else
            {
                StartCoroutine(EnableDropOffNotification());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && removeAfterExit)
        {
            RemoveNotification();
        }
    }

    private IEnumerator EnablePickupNotification()
    {
        objectCollider.enabled = false;
        notificationAnim.gameObject.SetActive(true);
        notificationAnim.Play("FadeIn");

        int passengerCount = PlayerData.PD != null ? PlayerData.PD.currentPassengers : 0;
        notificationTextUI.text = $"{passengerCount} passengers picked up!\nNext stop: {dropOffZone?.name ?? "Unknown Location"}";

        DisableAllZonesExcept("DropOffZone", dropOffZone?.name);

        if (disableAfterTimer)
        {
            yield return new WaitForSeconds(disableTimer);
            RemoveNotification();
        }
    }

    private IEnumerator EnableDropOffNotification()
    {
        notificationAnim.gameObject.SetActive(true);
        notificationAnim.Play("FadeIn");

        int passengersDropped = PlayerData.PD.currentPassengers;
        PlayerData.PD.currentPassengers = 0;

        string nextPickupZone = GetRandomZone("PickupZone");
        notificationTextUI.text = $"{passengersDropped} passengers dropped off!\nNext stop: {nextPickupZone}";
        characterIconUI.sprite = dropOffIcon;

        if (disableAfterTimer)
        {
            yield return new WaitForSeconds(disableTimer);
            RemoveNotification();
        }

        AssignRandomDropOff();
    }

    private void RemoveNotification()
    {
        notificationAnim.Play("FadeOut");
        notificationAnim.gameObject.SetActive(false);
    }

    private void AssignRandomDropOff()
    {
        GameObject[] dropOffZones = GameObject.FindGameObjectsWithTag("DropOffZone");

        if (dropOffZones.Length == 0)
        {
            Debug.LogError("🚨 No available drop-off zones found! Assigning a backup.");
            return;
        }

        foreach (GameObject zone in dropOffZones)
        {
            zone.SetActive(true);
            Debug.Log($"✅ Activating DropOffZone: {zone.name}");
        }

        dropOffZone = dropOffZones[Random.Range(0, dropOffZones.Length)];
        Debug.Log($"🎯 Assigned DropOff: {dropOffZone.name}");
    }


    private GameObject FindRandomInactiveZone(string tag)
    {
        GameObject[] zones = GameObject.FindGameObjectsWithTag(tag);
        List<GameObject> inactiveZones = new List<GameObject>();

        foreach (GameObject zone in zones)
        {
            if (!zone.activeSelf)
            {
                inactiveZones.Add(zone);
            }
        }

        return inactiveZones.Count > 0 ? inactiveZones[Random.Range(0, inactiveZones.Count)] : null;
    }

    private void DisableAllZonesExcept(string tag, string activeZoneName)
    {
        GameObject[] zones = GameObject.FindGameObjectsWithTag(tag);
        foreach (GameObject zone in zones)
        {
            zone.SetActive(zone.name == activeZoneName);
        }
    }

    private string GetRandomZone(string tag)
    {
        GameObject[] zones = GameObject.FindGameObjectsWithTag(tag);
        return zones.Length > 0 ? zones[Random.Range(0, zones.Length)].name : "Unknown";
    }
}
