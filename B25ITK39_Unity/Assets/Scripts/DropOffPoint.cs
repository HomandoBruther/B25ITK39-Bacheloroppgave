using UnityEngine;

public class DropOffPoint : MonoBehaviour
{
    public GameObject highlightEffect; // Sett et synlig område her

    private void Start()
    {
        if (highlightEffect != null)
        {
            highlightEffect.SetActive(true); // Alltid synlig til passasjerer er droppet av
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

                // Skjul highlight etter dropp-off
                if (highlightEffect != null)
                {
                    highlightEffect.SetActive(false);
                }
            }
        }
    }
}
