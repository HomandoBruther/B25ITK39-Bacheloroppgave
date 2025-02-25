using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public int carChoice;
    public int health;
    public static PlayerData PD;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MakeThisTheOnlyGameManager();
    }

    void MakeThisTheOnlyGameManager()
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
}
