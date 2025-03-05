using System;
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
    

    //state change
    [SerializeField] float sightRange, attackRange;
    bool playerInSight, playerInAttackRange;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find(playerCar);
        animator = GetComponent<Animator>();

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

        if(Physics.Raycast(destPoint, Vector3.down, groundLayer))
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
        // Get the rigidbody of the zombie
        Rigidbody myRigidbody = GetComponent<Rigidbody>();
        float mySpeed = myRigidbody != null ? myRigidbody.linearVelocity.magnitude : 0f;

        // Check if the colliding object is the car
        CarController carController = collision.gameObject.GetComponent<CarController>();
        if (carController != null)
        {
            Rigidbody carRigidbody = collision.gameObject.GetComponent<Rigidbody>();
            float carSpeed = carRigidbody != null ? carRigidbody.linearVelocity.magnitude : 0f;

            Debug.Log($"Hit by car. Car Speed: {carSpeed}, My Speed: {mySpeed}");

            // Define a speed threshold
            float speedThreshold = 1f;

            if (carSpeed + mySpeed >= speedThreshold)
            {
                Debug.Log("Collision speed meets the threshold!");
                /*
                // Move the zombie slightly ahead of the car to prevent extreme physics effects
                Vector3 forwardOffset = collision.transform.forward * 1.5f; // Adjust distance if needed
                transform.position = collision.transform.position + forwardOffset;
                */

                // Disable animation & collider to prevent animation interference
                GetComponent<Animator>().enabled = false;
                GetComponent<Collider>().enabled = false;
                alive = false;

                // Enable ragdoll physics
                ActivateRagdoll(carRigidbody, carSpeed);
            }
        }

        var player = collision.GetComponent<CarController>();
        if (player != null)
        {
            PlayerData.PD.currentHealth -= 10;
        }
    }

    void ActivateRagdoll(Rigidbody collidingRigidbody, float RigidbodySpeed)
    {
        // Disable the NavMeshAgent if it's present
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != null) agent.enabled = false;

        Rigidbody zombieRigidBody = GetComponent<Rigidbody>();

        // Enable physics on all child rigidbodies
        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            rb.isKinematic = false; // Allow physics to take over
            rb.useGravity = true;
            rb.AddExplosionForce(10000f, transform.up + collidingRigidbody.transform.forward, 5000f, 2500f);
        }

        
        if (zombieRigidBody)
        {
            zombieRigidBody.isKinematic = true;
        }
        /*
        zombieRigidBody.AddForce(collidingRigidbody.transform.up * RigidbodySpeed*2);
        */
    }
}
