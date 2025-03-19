using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
public class NotificationTriggerEvent : MonoBehaviour

{
    [Header("UI Content")]
    [SerializeField] private TMPro.TextMeshProUGUI notificationTextUI;

    [SerializeField] private Image characterIconUI;

    [Header("Message Customization")]
    [SerializeField] private Sprite yourIcon;
    [SerializeField] [TextArea] private string notificationMessage;


    [Header("Notification Removal")]
    [SerializeField] private bool removeAfterExit = false;
    [SerializeField] private bool disableAfterTimer = false;
    [SerializeField] private float disableTimer = 1.0f;

    [Header("Message Animation")]
    [SerializeField] private Animator notificationAnim;
    private BoxCollider objectCollider;

    private void Awake()
    {
        objectCollider = gameObject.GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Triggered by: " + other.gameObject.name);
        if (other.CompareTag("Player"))
        {
            gameObject.SetActive(true); // Ensures the object is active
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

        // Ensure the notification UI is active
        notificationAnim.gameObject.SetActive(true);

        notificationAnim.Play("FadeIn");
        notificationTextUI.text = notificationMessage;
        characterIconUI.sprite = yourIcon;

        if (disableAfterTimer)
        {
            yield return new WaitForSeconds(disableTimer);
            RemoveNotification();
        }
    }


    void RemoveNotification()
    {
        notificationAnim.Play("FadeOut");
        notificationAnim.gameObject.SetActive(false); // Instead of `gameObject.SetActive(false);`
    }
   }

