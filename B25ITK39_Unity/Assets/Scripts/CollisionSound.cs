using UnityEngine;

public class CollisionSound : MonoBehaviour
{
    public AudioSource[] audioSourceList;
    private AudioSource audioSource; // Assign this in the Inspector

    private void Start()
    {
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>(); // Add an AudioSource if missing
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > 2f) // Prevent tiny collisions from playing sound
        {
            int randomNumber = UnityEngine.Random.Range(0, audioSourceList.Length);
            AudioClip clip = audioSourceList[randomNumber].clip;

            AudioSource.PlayClipAtPoint(clip, transform.position, 1f);
        }
    }
}