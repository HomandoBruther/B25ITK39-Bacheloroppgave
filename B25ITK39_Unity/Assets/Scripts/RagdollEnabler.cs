using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollEnabler : MonoBehaviour
{

    [SerializeField]
    private Animator Animator;
    [SerializeField]
    private Transform RagdollRoot;
    [SerializeField]
    private bool StartRagdoll = false;
    // Only public for Ragdoll Runtime GUI for explosive force
    public Rigidbody[] Rigidbodies;
    private CharacterJoint[] Joints;
    private Collider[] Colliders;




    // Start is called before the first frame update
    void Start()
    {
        setRigidbodyState(true);
        setColliderState(false);
        GetComponent<Animator>().enabled = true;
    }

    public void ragdollOnDeath()
    {
        Animator.enabled = false;
        foreach (CharacterJoint joint in Joints)
        {
            joint.enableCollision = true;
        }
        foreach (Collider collider in Colliders)
        {
            collider.enabled = true;
        }
        foreach (Rigidbody rigidbody in Rigidbodies)
        {
            rigidbody.linearVelocity = Vector3.zero;
            rigidbody.detectCollisions = true;
            rigidbody.useGravity = true;
        }
    }

    void setRigidbodyState(bool state)
    {

        Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rigidbody in rigidbodies)
        {
            rigidbody.isKinematic = state;
        }

        GetComponent<Rigidbody>().isKinematic = !state;

    }


    void setColliderState(bool state)
    {

        Collider[] colliders = GetComponentsInChildren<Collider>();

        foreach (Collider collider in colliders)
        {
            collider.enabled = state;
        }

        GetComponent<Collider>().enabled = !state;

    }

}