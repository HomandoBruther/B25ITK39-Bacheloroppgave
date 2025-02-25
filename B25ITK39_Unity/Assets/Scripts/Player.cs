using UnityEngine;

public class Player : MonoBehaviour {

    public GameObject bus;
    public GameObject sportsCar;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (PlayerData.PD.carChoice != 0)
        {
            sportsCar.SetActive(false);
        }

        if (PlayerData.PD.carChoice != 1)
        {
            bus.SetActive(false);
        }
    }

    // Update is called once per frame
}
