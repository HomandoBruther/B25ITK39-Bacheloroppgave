using UnityEngine;
using TMPro;

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

    private void Awake()
    {
        arrow3D = FindObjectOfType<Arrow3DController>();
        isPickupZone = CompareTag("PickupZone");
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
        PlayerData.PD.FillPassengers(Random.Range(1, 5));
        nextDropOff = FindRandomZone("DropOffZone");
        notificationTextUI.text = $"Passengers picked up! Next stop: {nextDropOff.name}";
        notificationAnim.Play("FadeIn");

        if (arrow3D != null) arrow3D.SetTarget(nextDropOff.transform);

        gameObject.SetActive(false);
    }

    private void HandleDropOff()
    {
        int passengersDropped = PlayerData.PD.currentPassengers;
        PlayerData.PD.currentPassengers = 0;

        nextPickup = FindRandomZone("PickupZone");

        if (nextPickup == null)
        {
            Debug.LogError("❌ No available PickupZone found!");
            notificationTextUI.text = $"{passengersDropped} passengers dropped off! No available pickup zone.";
        }
        else
        {
            notificationTextUI.text = $"{passengersDropped} passengers dropped off!\nNext stop: {nextPickup.name}";
            if (arrow3D != null) arrow3D.SetTarget(nextPickup.transform);
        }

        notificationAnim.Play("FadeIn");
        gameObject.SetActive(false);
    }


    private GameObject FindRandomZone(string tag)
    {
        GameObject[] zones = GameObject.FindGameObjectsWithTag(tag);

        Debug.Log($"🔍 Found {zones.Length} {tag} zones");

        return zones.Length > 0 ? zones[Random.Range(0, zones.Length)] : null;
    }

}
