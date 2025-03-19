using UnityEngine;

public class EngineSound : MonoBehaviour
{
    
    AudioSource audioSource;
    Rigidbody rigidBody;



    public float minSpeed = 0f;
    public float maxSpeed = 20f;
    public float minimumPitch = 0.5f;
    public float maximumPitch = 2.5f;

    //public float engineSpeed = 0.5f;



    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        audioSource.pitch = minimumPitch;
        
        if (audioSource == null)
        {
            Debug.LogError("No AudioSource found on " + gameObject.name);
        }
        if (rigidBody == null)
        {
            Debug.LogError("No Rigidbody found on " + gameObject.name);
        }
    }

    
    void Update()
    {
        if (rigidBody != null)
        {
            float speed = rigidBody.linearVelocity.magnitude; // Get bus speed
            float normalizedSpeed = Mathf.InverseLerp(minSpeed, maxSpeed, speed);
            audioSource.pitch = Mathf.Lerp(minimumPitch, maximumPitch, normalizedSpeed);
        }

        /*
        audioSource.pitch = engineSpeed;

        if(engineSpeed < minimumPitch) {
            audioSource.pitch = minimumPitch;
        }else if (engineSpeed > maximumPitch){
            audioSource.pitch = maximumPitch;
        }else{
            audioSource.pitch = engineSpeed;
        }
        */
    }
}
