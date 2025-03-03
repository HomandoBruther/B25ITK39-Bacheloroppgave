using UnityEngine;


public class Billboard : MonoBehaviour
{
    private Transform cameraTransform;

    private void Start()
    {
        cameraTransform = Camera.main.transform; // Henter hovedkameraet
    }

    private void LateUpdate()
    {
        // Roter teksten mot kameraet
        transform.LookAt(cameraTransform);
        transform.rotation = Quaternion.LookRotation(cameraTransform.forward);
    }
}
