using UnityEngine;

public class PlayerData : MonoBehaviour
{
    
    
    public static PlayerData PD;

    //Car Stats
    public int carChoice;
    public int health;
    public int currentPassengers;
    public int currentImportantPassengers;
    public int maxPassengers;

    //Player Stats
    public string name;
    public int points;
    public int money;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MakeThisTheOnlyPlayerData();
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

    int scorePoints()
    {
        int currentScoring = 0;
        currentScoring += EmptyPassengers() * 100;
        currentScoring += EmptyImportantPassengers() * 1000;

        points += currentScoring;

        return currentScoring;
    }
}
