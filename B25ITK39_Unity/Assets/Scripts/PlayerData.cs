using UnityEngine;

public class PlayerData : MonoBehaviour
{
    
    
    public static PlayerData PD;

    //Car Stats
    public int carChoice = 1;
    
    public int maxHealth;
    public int currentHealth;
    public int currentPassengers;
    public int currentImportantPassengers;
    public int maxPassengers;

    //Player Stats
    public string name;
    public int points = 0;
    public int money = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MakeThisTheOnlyPlayerData();
        maxHealth = 1000;
        maxPassengers = 30;
        currentHealth = maxHealth;
    }

    void MakeThisTheOnlyPlayerData()
    {
    https://discussions.unity.com/t/accessing-variables-on-a-script-on-a-dontdestroyonload-gameobject/149506
        if (PD == null)
        {
            DontDestroyOnLoad(gameObject);
            PD = this;
        }
        else
        {
            if (PD != this)
            {
                Destroy(gameObject);
            }
        }
    }

    public int FillPassengers(int passengers)
    {
        if (maxPassengers >= currentPassengers + passengers + currentImportantPassengers)
        {
            currentPassengers += passengers;
            return 0;
        }
        else
        {
            currentPassengers = maxPassengers;
            return (currentPassengers + passengers) - maxPassengers;
        }  
    }

    public int FillImportantPassengers(int passengers)
    {
        if (maxPassengers >= currentPassengers + passengers + currentImportantPassengers)
        {
            currentPassengers += passengers;
            return 0;
        }
        else
        {
            currentPassengers = maxPassengers;
            return (currentPassengers + passengers) - maxPassengers;
        }
    }

    int EmptyPassengers()
    {
        int emptiedPassengers = currentPassengers;
        currentPassengers = 0;

        return emptiedPassengers;
    }

    int EmptyImportantPassengers()
    {
        int emptiedImporantPassengers = currentImportantPassengers;
        currentImportantPassengers = 0;

        return emptiedImporantPassengers;
    }

    public int ScorePoints()
    {
        int currentScoring = 0;
        currentScoring += EmptyPassengers() * 100;
        currentScoring += EmptyImportantPassengers() * 1000;

        points += currentScoring;
        money += currentScoring / 10;

        return currentScoring;
    }
}
