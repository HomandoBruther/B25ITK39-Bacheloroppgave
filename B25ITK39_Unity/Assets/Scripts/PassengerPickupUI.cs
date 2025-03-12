using System.Collections;
using UnityEngine;
using TMPro;

public class PassengerPickupUI : MonoBehaviour
{
    public static PassengerPickupUI instance; // Singleton pattern
    public TextMeshProUGUI pickupText; // Assign in Inspector
    public GameObject pickupPopupCanvas; // Assign in Inspector

    private void Awake()
    {
        if (pickupPopupCanvas == null)
            Debug.LogError("❌ pickupPopupCanvas is NULL! Assign it in the Inspector.");
        else
            Debug.Log("✅ pickupPopupCanvas assigned successfully!");

        if (pickupText == null)
            Debug.LogError("❌ pickupText is NULL! Assign it in the Inspector.");
        else
            Debug.Log("✅ pickupText assigned successfully!");

        pickupPopupCanvas.SetActive(false);
    }


    public void ShowPickupMessage(int passengersPickedUp)
    {
        if (pickupText == null || pickupPopupCanvas == null)
        {
            Debug.LogError("Pickup UI components are NOT assigned! Assign them in the Inspector.");
            return;
        }

        Debug.Log("Showing pop-up for " + passengersPickedUp + " passengers."); // Debugging
        pickupText.text = passengersPickedUp + " passengers picked up!";
        pickupPopupCanvas.SetActive(true);

        StartCoroutine(ShowAndHidePopup());
    }

    private IEnumerator ShowAndHidePopup()
    {
        yield return new WaitForSeconds(2f); // Show for 2 seconds
        pickupPopupCanvas.SetActive(false);
    }
}
