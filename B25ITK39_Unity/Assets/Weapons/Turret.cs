using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    [SerializeField] private float turretRange = 13f;
    [SerializeField] private float turretRotationSpeed = 5f;
    [SerializeField] private LayerMask enemyLayer;

    private Transform targetZombie;
    private Gun currentGun;
    private float fireRate;
    private float fireRateDelta;

    private void Start()
    {
        currentGun = GetComponentInChildren<Gun>();
        fireRate = currentGun.GetRateOfFire();
    }

    private void Update()
    {
        FindNearestZombie();

        if (targetZombie == null) return; // No zombies in range

        Vector3 targetPosition = targetZombie.position; // Keep Y for proper aiming
        float distanceToZombie = Vector3.Distance(transform.position, targetPosition);


        if (distanceToZombie > turretRange) return; // Zombie out of range

        RotateTurretTowards(targetPosition); // Now rotates in full 3D

        fireRateDelta -= Time.deltaTime;
        if (fireRateDelta <= 0)
        {
            currentGun.Fire();
            fireRateDelta = fireRate;
        }
    }

    private void FindNearestZombie()
    {
        Collider[] zombiesInRange = Physics.OverlapSphere(transform.position, turretRange, enemyLayer);
        float shortestDistance = Mathf.Infinity;
        Transform nearestZombie = null;

        foreach (var zombieCollider in zombiesInRange)
        {
            float distanceToZombie = Vector3.Distance(transform.position, zombieCollider.transform.position);
            if (distanceToZombie < shortestDistance)
            {
                shortestDistance = distanceToZombie;
                nearestZombie = zombieCollider.transform;
            }
        }

        targetZombie = nearestZombie;

    }

    private void RotateTurretTowards(Vector3 targetPosition)
    {
        Vector3 directionToTarget = (targetPosition - transform.position).normalized; // Full 3D direction
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget); // Full rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turretRotationSpeed * Time.deltaTime);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, turretRange);
    }
}
