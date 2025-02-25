using System;
using UnityEngine;
using UnityEngine.AI;


public class enemyAIPatrol : MonoBehaviour
{

    GameObject player;
    NavMeshAgent agent;

    [SerializeField] LayerMask groundLayer, playerLayer;

    //patrol
    Vector3 destPoint;
    bool walkpointSet;
    [SerializeField] float walkRange;

    //string playerCar = ("SportsCar_1");
    string playerCar = ("BusNoWheel");

    //state change
    [SerializeField] float sightRange, attackRange;
    bool playerInSight, playerInAttackRange;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.Find(playerCar);
    }

    // Update is called once per frame
    void Update()
    {
        playerInSight = Physics.CheckSphere(transform.position, sightRange, playerLayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);

        if (!playerInSight && !playerInAttackRange) Patrol();
        if (playerInSight && !playerInAttackRange) Chase();
        if (playerInSight && playerInAttackRange) Attack();
        Chase();
    }

    void Chase()
    {
        agent.SetDestination(player.transform.position);
    }

    void Attack()
    {

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
}
