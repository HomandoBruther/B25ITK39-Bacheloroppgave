using UnityEngine;
using TMPro;

public class SceneHandler : MonoBehaviour {

    public GameObject bus;
    public GameObject sportsCar;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI timerText;

    private float timer = 0.0f;
    private int seconds = 0;
    private int minutes = 0;

    public int speed = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        
        UpdateTime();
        UpdateText();
    }

    private void UpdateTime()
    {
        timer += Time.deltaTime;
        seconds = Mathf.FloorToInt(timer);
        if (seconds >= 60)
        {
            minutes++;
            seconds = 0;
            timer = 0;
        }
        
    }

    private void UpdateText()
    {
        float SpeedinMS = speed;
        speed = Mathf.RoundToInt(SpeedinMS * 3.6f);

        scoreText.text = "Score: " + PlayerData.PD.points;
        healthText.text = "Health: " + PlayerData.PD.currentHealth + "/" + PlayerData.PD.maxHealth;
        moneyText.text = "Money: " + PlayerData.PD.money;
        timerText.text = minutes.ToString("D2") + ":" + seconds.ToString("D2");
        speedText.text = speed + "km/h";
    }
    
}
