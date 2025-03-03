using UnityEngine;

public class DropOffPoint : MonoBehaviour
{
    public GameObject highlightEffect; // Sett highlight-plane her
    public GameObject dropOffText; // Sett "DROP OFF HERE"-teksten her

    private void Start()
    {
        if (highlightEffect != null)
        {
            highlightEffect.SetActive(true); // Highlight synlig fra start
        }

        if (dropOffText != null)
        {
            dropOffText.SetActive(true); // Tekst synlig fra start
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CarController car = other.GetComponent<CarController>();
            if (car != null)
            {
                car.DropOffPassengers();
                Debug.Log("Passengers dropped off!");

                // Skjul både highlight og tekst
                if (highlightEffect != null)
                {
                    highlightEffect.SetActive(false);
                }

                if (dropOffText != null)
                {
                    dropOffText.SetActive(false);
                }
            }
        }
    }
}
