
using UnityEngine;
using TMPro;

public class GameStartTrigger : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject startGameCanvas;
    [SerializeField] private TextMeshProUGUI startGameText;

    private CountdownTimer countdownTimer;
    private Arrow3DController arrow3D;
    private bool gameStarted = false;
    private NotificationTriggerEvent notifier;

    private void Start()
    {
        notifier = FindObjectOfType<NotificationTriggerEvent>();
        countdownTimer = FindObjectOfType<CountdownTimer>();
        arrow3D = FindObjectOfType<Arrow3DController>();

        if (startGameCanvas != null)
            startGameCanvas.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (gameStarted || !other.CompareTag("Player"))
            return;

        gameStarted = true;

        // Find the first pickup zone using the NotificationTriggerEvent logic
        GameObject[] pickups = GameObject.FindGameObjectsWithTag("PickupZone");
        if (pickups.Length == 0) return;

        GameObject closestPickup = FindClosest(pickups, transform.position);

        // Start the countdown
        float distance = Vector3.Distance(transform.position, closestPickup.transform.position);
        countdownTimer?.StartCountdown(distance);

        // Set arrow to first pickup
        if (arrow3D != null)
            arrow3D.SetTarget(closestPickup.transform);

        if (notifier != null)
        {
            string stopName = notifier.FormatStopName(closestPickup.name);
            if (startGameCanvas != null && startGameText != null)
            {
                startGameCanvas.SetActive(true);
                startGameText.text = $"Game started!\nFirst stop: {stopName}";
                Invoke(nameof(HideStartGameUI), 5f);
            }

            // Trigger a fake "notification" animation
            notifier.SendMessage("FadeOutNotification", SendMessageOptions.DontRequireReceiver); // clear old
            notifier.notificationTextUI.text = $"Game Started!\nFirst stop: {stopName}\n";
            notifier.notificationAnim.Play("FadeIn");
            Invoke(nameof(FadeOutNotification), 5f);
        }


        // Optional: disable this trigger after first use
        gameObject.SetActive(false);
    }

    private GameObject FindClosest(GameObject[] objects, Vector3 fromPosition)
    {
        GameObject closest = null;
        float minDist = Mathf.Infinity;

        foreach (GameObject obj in objects)
        {
            float dist = Vector3.Distance(obj.transform.position, fromPosition);
            if (dist < minDist)
            {
                closest = obj;
                minDist = dist;
            }
        }

        return closest;
    }

    private void HideStartGameUI()
    {
        if (startGameCanvas != null)
            startGameCanvas.SetActive(false);
    }
    private void FadeOutNotification()
    {
        notifier?.notificationAnim?.Play("FadeOut");
    }

}
