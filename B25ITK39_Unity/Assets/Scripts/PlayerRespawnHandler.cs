using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerRespawnHandler : MonoBehaviour
{
    private GameObject player;
    public Rigidbody rb;
    [SerializeField] Animator transition;

    public GameObject respawnText;
    public float dotFrequency = 0.5f;

    // Gather Meshes
    public MeshRenderer busMesh;
    public MeshRenderer FL;
    public MeshRenderer FR;
    public MeshRenderer RL;
    public MeshRenderer RR;

    private Transform checkPointPos;

    // Boolean to check if player can die (Delay needed to avoid script repeating)
    private bool canDie;

    public void Start()
    {
        player = GameObject.FindWithTag("Player");
        canDie = true;
    }

    // Uppdates the checkpoint location (Gets the location from the other triggers on the map)
    public void UpdateCheckPoint(Transform pos)
    {
        Debug.Log("New respawn point set");
        checkPointPos = pos;
    }

    // Player has touched the lava which will activate the die script
    public void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Danger") && canDie == true)
        {
            canDie = false;
            Die();
        }
    }

    // This handles player death (Will need to be looked at to make sure of no-odd behaviour)
    public void Die()
    {
        Debug.Log("You died, wait for respawn");
        transition.SetTrigger("Start");
        rb.angularVelocity = new Vector3(0, 0, 0);
        rb.linearVelocity = new Vector3(0, 0, 0);

        busMesh.enabled=false;
        FL.enabled = false;
        FR.enabled = false;
        RL.enabled = false;
        RR.enabled = false;

        StartCoroutine(Respawn(2));

    }

    // This respawns the player 
    public IEnumerator Respawn(int secs)
{
    //Respawn Text Manipulator
    float currentSecs = 0;

    if (respawnText != null)
    {
        // Cache TMP component once
        TextMeshProUGUI tmp = respawnText.GetComponent<TextMeshProUGUI>();
        string baseText = "Respawning";

        tmp.text = baseText;
        respawnText.SetActive(true); // Ensure it's visible

        while (currentSecs < secs)
        {
            currentSecs += dotFrequency;
            tmp.text = tmp.text + ".";
            yield return new WaitForSeconds(dotFrequency);
        }

        tmp.text = baseText; // Reset to clean version
    }

    transition.SetTrigger("End");

    if (respawnText != null)
        respawnText.SetActive(false);

    //Respawn
    busMesh.enabled = true;
    FL.enabled = true;
    FR.enabled = true;
    RL.enabled = true;
    RR.enabled = true;

    player.transform.position = checkPointPos.position;
    player.transform.localRotation = checkPointPos.localRotation;

    rb.angularVelocity = Vector3.zero;
    rb.linearVelocity = Vector3.zero;

    canDie = true;
}
}
