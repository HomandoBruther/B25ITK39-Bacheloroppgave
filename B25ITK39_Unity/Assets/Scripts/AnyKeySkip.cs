using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadSceneOnInput : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;

    public GameObject TutorialCanvas;
    public GameObject LoadingCanvas;

    private bool hasStartedLoading = false;

    void Update()
    {
        if (Input.anyKeyDown && !hasStartedLoading)
        {
            hasStartedLoading = true;
            StartCoroutine(LoadSceneAfterCanvasChange());
        }
    }

    IEnumerator LoadSceneAfterCanvasChange()
    {
        DeactivateAllCanvases();
        LoadingCanvas.SetActive(true);

        // Wait one frame so Unity can update the UI
        yield return null;

        // Optional: wait a bit longer to allow UI animations/sounds to play
        // yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(sceneToLoad);
    }

    void DeactivateAllCanvases()
    {
        Canvas[] allCanvases = FindObjectsOfType<Canvas>();
        foreach (Canvas canvas in allCanvases)
        {
            canvas.gameObject.SetActive(false);
        }
    }
}