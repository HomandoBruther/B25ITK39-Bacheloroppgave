using UnityEngine;
using TMPro;

public class SceneHandler : MonoBehaviour {

    public GameObject bus;
    public GameObject sportsCar;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI speedText;


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

        scoreText.text = "Score: " + PlayerData.PD.points;
        healthText.text = "Health: " + PlayerData.PD.currentHealth + "/" + PlayerData.PD.maxHealth;
        moneyText.text = "Money: " + PlayerData.PD.money;
    }

    private void Update()
    {
        
    }

    // Update is called once per frame
}
