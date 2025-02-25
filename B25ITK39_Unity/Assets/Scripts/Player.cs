using UnityEngine;

public class Player : MonoBehaviour {

    public GameObject bus;
    public GameObject sportsCar;
    public GameObject busVirtualCamera;
    public GameObject sportsCarVirtualCamera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (PlayerData.PD.carChoice != 0)
        {
            sportsCar.SetActive(false);
            busVirtualCamera.SetActive(true);
        }

        if (PlayerData.PD.carChoice != 1)
        {
            bus.SetActive(false);
            sportsCarVirtualCamera.SetActive(true);
        }
    }

    // Update is called once per frame
}
