using UnityEngine;

public class EngineSound : MonoBehaviour
{
    
    AudioSource audioSource;

    public float minimumPitch = 0.5f;
    public float maximumPitch = 2.5f;

    public float engineSpeed = 0.5f;



    void Start()
    {

        audioSource = GetComponent<AudioSource>();
        audioSource.pitch = minimumPitch;
        
    }

    // Update is called once per frame
    void Update()
    {
        audioSource.pitch = engineSpeed;

        if(engineSpeed < minimumPitch) {
            audioSource.pitch = minimumPitch;
        }else if (engineSpeed > maximumPitch){
            audioSource.pitch = maximumPitch;
        }else{
            audioSource.pitch = engineSpeed;
        }
    }
}
