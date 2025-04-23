using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;



public class enemyAIPatrol : MonoBehaviour
{

    GameObject player;
    NavMeshAgent agent;

    [SerializeField] LayerMask groundLayer, playerLayer;

    Animator animator;

    BoxCollider boxLeftCollider;


    //patrol
    Vector3 destPoint;
    bool walkpointSet;
    [SerializeField] float walkRange;
    string playerCar = ("BusNoWheel");

    bool alive = true;

    private AudioSource audioSource;

    public AudioSource[] audioSourceList;



    //state change
    [SerializeField] float sightRange, attackRange;
    bool playerInSight, playerInAttackRange;

    private ZombieManager zombieManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find(playerCar);
        animator = GetComponent<Animator>();
        zombieManager = FindObjectOfType<ZombieManager>();

        boxLeftCollider = GetComponentInChildren<BoxCollider>();

    }



    // Update is called once per frame
    void Update()
    {
        if (this.alive)
        {
            playerInSight = Physics.CheckSphere(transform.position, sightRange, playerLayer);
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);

            if (!playerInSight && !playerInAttackRange) Patrol();
            if (playerInSight && !playerInAttackRange) Chase();
            if (playerInSight && playerInAttackRange) Attack();
        }

    }

    void Chase()
    {
        agent.SetDestination(player.transform.position);
    }

    void Attack()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Z_Attack 1"))
        {
            animator.SetTrigger("Attack");
            agent.SetDestination(transform.position);
        }

    }



    void Patrol()
    {
        if (!walkpointSet) SearchForDest();
        if (walkpointSet) agent.SetDestination(destPoint);
        if (Vector3.Distance(transform.position, destPoint) < 10) walkpointSet = false;
    }


    void SearchForDest()
    {
        float z = UnityEngine.Random.Range(-walkRange, walkRange);
        float x = UnityEngine.Random.Range(-walkRange, walkRange);

        destPoint = new Vector3(transform.position.x + x, transform.position.y, transform.position.z + z);

        if (Physics.Raycast(destPoint, Vector3.down, groundLayer))
        {
            walkpointSet = true;
        }

    }

    void EnableLeftAttack()
    {
        boxLeftCollider.enabled = true;
    }

    void DisableLeftAttack()
    {
        boxLeftCollider.enabled = false;
    }

    //void ZombieDeath()

    /*private void OnTriggerEnter(Collider other)
   {
       
   }
    */

    void OnTriggerEnter(Collider collision)
    {
        // === DANGER ZONE KILL LOGIC ===
        if (collision.CompareTag("Danger") && alive)
        {
            Debug.Log("Zombie Killed By Lava");
            alive = false;
            GetComponent<Animator>().enabled = false;
            PlayDeathSound();

            if (zombieManager != null)
            {
                zombieManager.OnZombieKilled(this.gameObject);
            }

            return; // Stop here so it doesn't also trigger car collision logic
        }

        // === CAR COLLISION LOGIC ===
        if (alive)
        {
            Rigidbody myRigidbody = GetComponent<Rigidbody>();
            float mySpeed = myRigidbody != null ? myRigidbody.linearVelocity.magnitude : 0f;

            CarController carController = collision.gameObject.GetComponent<CarController>();
            if (carController != null)
            {
                Rigidbody carRigidbody = collision.gameObject.GetComponent<Rigidbody>();
                float carSpeed = carRigidbody != null ? carRigidbody.linearVelocity.magnitude : 0f;

                float speedThreshold = 1f;

                if (carSpeed + mySpeed >= speedThreshold)
                {
                    Debug.Log("Zombie hit by car!");

                    alive = false;
                    GetComponent<Animator>().enabled = false;
                    PlayerData.PD.points += 100;

                    ActivateRagdoll(carRigidbody, carSpeed, collision);
                    PlayDeathSound();

                    if (zombieManager != null)
                    {
                        zombieManager.OnZombieKilled(this.gameObject);
                    }
                }
            }

            // Optional: damage the player
            /*
            var player = collision.GetComponent<CarController>();
            if (player != null)
            {
                PlayerData.PD.currentHealth -= 10;
            }
            */
        }
    }

    void ActivateRagdoll(Rigidbody collidingRigidbody, float RigidbodySpeed, Collider collision)
    {
        // Disable the NavMeshAgent if it's present
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != null) agent.enabled = false;

        Rigidbody zombieRigidBody = GetComponent<Rigidbody>();

        // Enable physics on all child rigidbodies
        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            Vector3 forceDirection = collidingRigidbody.transform.forward + Vector3.up; // Slight upward lift
            float forceMagnitude = RigidbodySpeed * 4f; // Scale by car speed

            rb.AddForce(forceDirection.normalized * forceMagnitude, ForceMode.Impulse);
        }

        foreach (Collider col in GetComponentsInChildren<Collider>())
        {
            Physics.IgnoreCollision(collision.GetComponent<Collider>(), col.GetComponent<Collider>(), true);
            //Turning off collider for the wheels
            foreach (Collider wheel in collision.GetComponentsInChildren<Collider>())
            {
                Physics.IgnoreCollision(wheel.GetComponent<Collider>(), col.GetComponent<Collider>(), true);
            }
        }



        if (zombieRigidBody)
        {
            zombieRigidBody.isKinematic = true;
        }
        /*
        zombieRigidBody.AddForce(collidingRigidbody.transform.up * RigidbodySpeed*2);
        */
    }

    void PlayDeathSound()
    {
        int randomNumber = UnityEngine.Random.Range(0, audioSourceList.Length);
        AudioClip clip = audioSourceList[randomNumber].clip;

        AudioSource.PlayClipAtPoint(clip, transform.position, 1f);
    }



}