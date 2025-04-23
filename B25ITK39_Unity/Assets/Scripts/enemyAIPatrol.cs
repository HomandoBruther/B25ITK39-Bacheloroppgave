using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class enemyAIPatrol : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float walkRange;
    [SerializeField] private float sightRange;
    [SerializeField] private float attackRange;
    [SerializeField] private string playerCar = "BusNoWheel";

    private GameObject player;
    private NavMeshAgent agent;
    private Animator animator;
    private BoxCollider boxLeftCollider;
    private AudioSource audioSource;
    private ZombieManager zombieManager;

    public AudioSource[] audioSourceList;

    private Vector3 destPoint;
    private bool walkpointSet;
    private bool alive = true;

    private float aiUpdateInterval = 0.25f;
    private float nextAIUpdateTime = 0f;
    private float chaseUpdateThreshold = 1.0f;
    private Vector3 lastChaseTarget;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        boxLeftCollider = GetComponentInChildren<BoxCollider>();
        player = GameObject.FindWithTag("Player"); // Assumes the player bus is tagged
        zombieManager = FindObjectOfType<ZombieManager>();
    }

    private void Update()
    {
        if (!alive) return;

        if (Time.time >= nextAIUpdateTime)
        {
            nextAIUpdateTime = Time.time + aiUpdateInterval;
            RunAI();
        }
    }

    private void RunAI()
    {
        bool playerInSight = Physics.CheckSphere(transform.position, sightRange, playerLayer);
        bool playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);

        if (!playerInSight && !playerInAttackRange)
            Patrol();
        else if (playerInSight && !playerInAttackRange)
            Chase();
        else if (playerInSight && playerInAttackRange)
            Attack();
    }

    private void Patrol()
    {
        if (!walkpointSet)
            SearchForDest();

        if (walkpointSet)
            agent.SetDestination(destPoint);

        if (Vector3.Distance(transform.position, destPoint) < 5f)
            walkpointSet = false;
    }

    private void SearchForDest()
    {
        float z = Random.Range(-walkRange, walkRange);
        float x = Random.Range(-walkRange, walkRange);
        Vector3 potentialPoint = new Vector3(transform.position.x + x, transform.position.y, transform.position.z + z);

        if (Physics.Raycast(potentialPoint + Vector3.up * 2f, Vector3.down, 3f, groundLayer))
        {
            destPoint = potentialPoint;
            walkpointSet = true;
        }
    }

    private void Chase()
    {
        if (Vector3.Distance(player.transform.position, lastChaseTarget) > chaseUpdateThreshold)
        {
            agent.SetDestination(player.transform.position);
            lastChaseTarget = player.transform.position;
        }
    }

    private void Attack()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Z_Attack 1"))
        {
            animator.SetTrigger("Attack");
            agent.SetDestination(transform.position);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (!alive) return;

        if (collision.CompareTag("Danger"))
        {
            KillZombie("Zombie Killed By Lava");
            return;
        }

        CarController carController = collision.gameObject.GetComponent<CarController>();
        if (carController != null)
        {
            float mySpeed = GetComponent<Rigidbody>()?.velocity.magnitude ?? 0f;
            float carSpeed = collision.attachedRigidbody?.velocity.magnitude ?? 0f;

            if (carSpeed + mySpeed >= 1f)
            {
                PlayerData.PD.points += 100;
                KillZombie("Zombie hit by car!");
                ActivateRagdoll(collision.attachedRigidbody, carSpeed, collision);
            }
        }
    }

    private void KillZombie(string log)
    {
        Debug.Log(log);
        alive = false;
        animator.enabled = false;
        PlayDeathSound();

        if (zombieManager != null)
        {
            zombieManager.OnZombieKilled(this.gameObject);
        }
    }

    private void ActivateRagdoll(Rigidbody collidingRigidbody, float RigidbodySpeed, Collider collision)
    {
        if (agent != null) agent.enabled = false;
        Rigidbody zombieRigidBody = GetComponent<Rigidbody>();
        if (zombieRigidBody != null) zombieRigidBody.isKinematic = true;

        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            Vector3 forceDir = collidingRigidbody.transform.forward + Vector3.up;
            float forceMag = RigidbodySpeed * 4f;
            rb.AddForce(forceDir.normalized * forceMag, ForceMode.Impulse);
        }

        foreach (Collider col in GetComponentsInChildren<Collider>())
        {
            Physics.IgnoreCollision(collision, col, true);
            foreach (Collider wheel in collision.GetComponentsInChildren<Collider>())
            {
                Physics.IgnoreCollision(wheel, col, true);
            }
        }
    }

    private void PlayDeathSound()
    {
        if (audioSourceList.Length == 0) return;

        int randomIndex = Random.Range(0, audioSourceList.Length);
        AudioClip clip = audioSourceList[randomIndex]?.clip;

        if (clip != null)
            AudioSource.PlayClipAtPoint(clip, transform.position, 1f);
    }

    // Animation event hooks
    private void EnableLeftAttack() => boxLeftCollider.enabled = true;
    private void DisableLeftAttack() => boxLeftCollider.enabled = false;
}