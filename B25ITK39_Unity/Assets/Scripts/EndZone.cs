using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class EndZone : MonoBehaviour
{
    public static EndZone instance; // Singleton for easy access

    public GameObject endGameUI;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI leaderboardText;

    public GameObject LoadingCanvas;

    private bool isLoading = false;

    private void Awake()
    {
        instance = this; // Set instance to allow other scripts to call EndGame()
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered EndZone!");
            EndGame();
        }
    }

    public void EndGame()
    {
        Debug.Log("EndGame() was called!");
        if (endGameUI != null)
        {
            endGameUI.SetActive(true);
            Time.timeScale = 0f; // Pause the game

            int currentScore = PlayerData.PD.points;
            scoreText.text = "Score: " + currentScore.ToString();

            LeaderboardManager leaderboardManager = FindObjectOfType<LeaderboardManager>();
            if (leaderboardManager != null)
            {
                leaderboardManager.DisplayLeaderboard();
            }
        }
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

    public void PlayAgain()
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
        Debug.Log("Quitting game...");
        Application.Quit();
    }

    void DeactivateAllCanvases()
{
    Canvas[] allCanvases = FindObjectsOfType<Canvas>();
    foreach (Canvas canvas in allCanvases)
    {
        if (canvas.gameObject != LoadingCanvas && canvas.gameObject != this.gameObject)
        {
            canvas.gameObject.SetActive(false);
        }
    }
}
}
