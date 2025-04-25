using UnityEngine;
using Unity.Cinemachine;

public class CameraController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public GameObject forwardCameraGameObject;
    public GameObject backwardsCameraGameObject;
    public GameObject jumpCameraGameObject;
    public GameObject jumpBackwardsCameraGameObject;

    public int reverseCameraSpeedTrigger = -5;

    private GameObject activeCameraGameObject;

    private CinemachineCamera forwardCamera;
    private CinemachineCamera backwardsCamera;
    private CinemachineCamera jumpCamera;
    private CinemachineCamera jumpBackwardsCamera;

    private GameObject[] cameraList;

    private SceneHandler sceneHandler;
    private GameObject theGameController;
    private AirControl airControl;
    private GameObject player;

    void Start()
    {
        cameraList = GameObject.FindGameObjectsWithTag("Camera");
        theGameController = GameObject.FindGameObjectWithTag("GameController");
        sceneHandler = theGameController.GetComponent<SceneHandler>();
        player = GameObject.FindGameObjectWithTag("Player");
        airControl = player.GetComponent<AirControl>();
    }

    // Update is called once per frame
    void Update()
    {
        if (airControl.IsInAir)
        {
            ActivateJumpCamera();
        }
        else ActivateForwardCamera();
    }

    public void ActivateForwardCamera()
    {
        if (sceneHandler.speed >= reverseCameraSpeedTrigger)
            activeCameraGameObject = forwardCameraGameObject;
        else activeCameraGameObject = backwardsCameraGameObject;
        DeactivateCameras();
    }

    public void ActivateJumpCamera()
    {
        if (sceneHandler.speed >= reverseCameraSpeedTrigger)
            activeCameraGameObject = jumpCameraGameObject;
        else activeCameraGameObject = jumpBackwardsCameraGameObject;
        DeactivateCameras();
    }


    private void DeactivateCameras()
    {
            foreach (GameObject cam in cameraList)
            {
                cam.SetActive(cam == activeCameraGameObject);
            }
        
    }
}
