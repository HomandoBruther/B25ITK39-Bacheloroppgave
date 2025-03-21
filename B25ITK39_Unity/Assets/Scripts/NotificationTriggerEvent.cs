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
        int passengersPickedUp = Random.Range(1, 5);
        PlayerData.PD.FillPassengers(passengersPickedUp);

        nextDropOff = FindRandomZone("DropOffZone");

        notificationTextUI.text = $"{passengersPickedUp} passengers picked up!\n Next stop: {nextDropOff.name}";
        notificationAnim.Play("FadeIn");

        if (arrow3D != null) arrow3D.SetTarget(nextDropOff.transform);
    }

    private void HandleDropOff()
    {
        int scoreEarned = PlayerData.PD.ScorePoints(); // Updates points & money
        nextPickup = FindRandomZone("PickupZone");

        if (nextPickup == null)
        {
            Debug.LogError("❌ No available PickupZone found!");
            notificationTextUI.text = $"{scoreEarned} points earned!\nNo available pickup zone.";
        }
        else
        {
            notificationTextUI.text = $"{scoreEarned} points earned!\nNext stop: {nextPickup.name}";
            if (arrow3D != null) arrow3D.SetTarget(nextPickup.transform);
        }

        notificationAnim.Play("FadeIn");
    }

    private GameObject FindRandomZone(string tag)
    {
        GameObject[] zones = GameObject.FindGameObjectsWithTag(tag);
        Debug.Log($"🔍 Found {zones.Length} {tag} zones");
        return zones.Length > 0 ? zones[Random.Range(0, zones.Length)] : null;
    }
}
