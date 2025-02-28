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
    string playerCar;
    

    //state change
    [SerializeField] float sightRange, attackRange;
    bool playerInSight, playerInAttackRange;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (PlayerData.PD.carChoice == 0) playerCar = ("SportCar_1");
        if (PlayerData.PD.carChoice == 1) playerCar = ("BusNoWheel");
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find(playerCar);
        animator = GetComponent<Animator>();

        boxLeftCollider = GetComponentInChildren<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        playerInSight = Physics.CheckSphere(transform.position, sightRange, playerLayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);

        if (!playerInSight && !playerInAttackRange) Patrol();
        if (playerInSight && !playerInAttackRange) Chase();
        if (playerInSight && playerInAttackRange) Attack();
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

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<CarController>();
        if(player != null)
        {
            PlayerData.PD.currentHealth -= 10;
        }
    }
}
