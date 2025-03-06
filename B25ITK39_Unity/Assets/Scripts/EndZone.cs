using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class EndZone : MonoBehaviour
{
    public GameObject endGameUI;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI leaderboardText;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered EndZone!");
            EndGame();
        }
    }

    private void EndGame()
    {
        Debug.Log("EndGame() was called!");
        if (endGameUI != null)
        {
            endGameUI.SetActive(true);
            Time.timeScale = 0f; // Pause the game

            int currentScore = PlayerData.PD.points;
            scoreText.text = "Score: " + currentScore.ToString();

            // ✅ Ensure leaderboard is updated
            LeaderboardManager leaderboardManager = FindObjectOfType<LeaderboardManager>();
            if (leaderboardManager != null)
            {
                leaderboardManager.DisplayLeaderboard();
            }
        }
    }

    public void PlayAgain()
    {
        Time.timeScale = 1f;
        PlayerData.PD.points = 0;
        PlayerData.PD.currentPassengers = 0;
        PlayerData.PD.currentImportantPassengers = 0;

        // ✅ Reset leaderboard submission so player can submit in next game
        FindObjectOfType<LeaderboardManager>()?.ResetSubmission();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}
