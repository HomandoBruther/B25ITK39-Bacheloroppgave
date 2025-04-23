using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public GameObject controlsUI;
    public GameObject settingsUI;
    public GameObject LoadingCanvas;

    public static bool GameIsPaused = false;
    public static bool isShowControlActive = false;
    public static bool isSettingsActive = false;

    private bool isLoading = false;

    void Start()
    {
        Time.timeScale = 1f;

        pauseMenuUI.SetActive(false);
        controlsUI.SetActive(false);
        settingsUI.SetActive(false);

        GameIsPaused = false;
        isShowControlActive = false;
        isSettingsActive = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isSettingsActive)
            {
                ReturnToPauseMenu();
            }
            else if (isShowControlActive)
            {
                ReturnToPauseMenu();
            }
            else if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        pauseMenuUI.SetActive(false);
        GameIsPaused = false;
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        pauseMenuUI.SetActive(true);
        GameIsPaused = true;
    }

    public void ShowControls()
    {
        Time.timeScale = 0f;
        pauseMenuUI.SetActive(false);
        controlsUI.SetActive(true);
        settingsUI.SetActive(false);
        isShowControlActive = true;
    }

    public void ReturnToPauseMenu()
    {
        Time.timeScale = 0f;
        pauseMenuUI.SetActive(true);
        controlsUI.SetActive(false);
        settingsUI.SetActive(false);
        isShowControlActive = false;
        isSettingsActive = false;
    }

    public void ShowSettings()
    {
        Time.timeScale = 0f;
        pauseMenuUI.SetActive(false);
        settingsUI.SetActive(true);
        controlsUI.SetActive(false);
        isSettingsActive = true;
    }

    public void RestartGame()
    {
        if (!isLoading)
            StartCoroutine(LoadSceneWithDelay("StageOne"));
    }

    public void LoadMenu()
    {
        if (!isLoading)
            StartCoroutine(LoadSceneWithDelay("MainMenu"));
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private IEnumerator LoadSceneWithDelay(string sceneName)
{
    isLoading = true;
    Debug.Log("Started coroutine to load scene...");

    DeactivateAllCanvases();
    LoadingCanvas.SetActive(true);

    Time.timeScale = 1f;
    PlayerData.PD.points = 0;
    PlayerData.PD.currentPassengers = 0;
    PlayerData.PD.currentImportantPassengers = 0;

        yield return null;

    Debug.Log($"Loading scene: {sceneName}");
    SceneManager.LoadScene(sceneName);
}

    void DeactivateAllCanvases()
    {
        Canvas[] allCanvases = FindObjectsOfType<Canvas>();
        foreach (Canvas canvas in allCanvases)
        {
            if (canvas.gameObject != LoadingCanvas && canvas.gameObject != this.gameObject) canvas.gameObject.SetActive(false);
        }
    }
}