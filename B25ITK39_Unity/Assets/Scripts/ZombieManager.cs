using System.Collections.Generic;
using UnityEngine;

public class ZombieManager : MonoBehaviour
{
    [SerializeField] public int maxActiveZombies = 100;
    private List<GameObject> activeZombies = new List<GameObject>();
    private List<GameObject> allZombies = new List<GameObject>();

    void Start()
    {
        InitializeZombiePool();
    }

    void InitializeZombiePool()
    {
        allZombies = new List<GameObject>(GameObject.FindGameObjectsWithTag("Zombie"));

        // Turn all zombies off first
        foreach (var z in allZombies)
        {
            z.SetActive(false);
        }

        // Shuffle the list
        Shuffle(allZombies);

        // Enable only the first `maxActiveZombies`
        for (int i = 0; i < Mathf.Min(maxActiveZombies, allZombies.Count); i++)
        {
            var zombie = allZombies[i];
            zombie.SetActive(true);
            activeZombies.Add(zombie);
        }
    }

    void Shuffle(List<GameObject> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            GameObject temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    public void OnZombieKilled(GameObject deadZombie)
    {
        activeZombies.Remove(deadZombie);
        allZombies.Remove(deadZombie);

        foreach (GameObject zombie in allZombies)
        {
            if (!zombie.activeInHierarchy)
            {
                zombie.SetActive(true);
                activeZombies.Add(zombie);
                break;
            }
        }
        Destroy(deadZombie, 20f); // Remove completely from scene after 20 seconds
    }
}