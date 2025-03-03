using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class EndZone : MonoBehaviour
{
    public GameObject endGameUI; // Dra EndGameUI fra Hierarchy hit i Inspector
    public TextMeshProUGUI scoreText; // Dra ScoreText fra Hierarchy hit i Inspector

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Noe traff EndZone: " + other.gameObject.name);

        if (other.CompareTag("Player")) // Sørg for at spilleren har "Player"-tag
        {
            Debug.Log("Spilleren traff EndZone!");
            EndGame();
        }
    }

    private void EndGame()
    {
        Debug.Log("EndGame() was called!");

        if (endGameUI != null)
        {
            endGameUI.SetActive(true); // Show End Screen UI

            // Update score BEFORE pausing the game
            if (scoreText != null)
            {
                scoreText.text = "Score: " + PlayerData.PD.points.ToString();
                Debug.Log("Score updated: " + PlayerData.PD.points);
            }
            else
            {
                Debug.LogError("ScoreText is not assigned in the Inspector!");
            }

            Time.timeScale = 0f; // Pause the game AFTER updating UI
            Debug.Log("Game is now paused!");
        }
        else
        {
            Debug.LogError("EndGameUI is not assigned in the Inspector!");
        }
    }

    public void PlayAgain()
    {
        Debug.Log("Restarting game...");

        // Reset time scale so the game is not frozen
        Time.timeScale = 1f;

        // Reset the player's score and passengers
        if (PlayerData.PD != null)
        {
            PlayerData.PD.points = 0;
            PlayerData.PD.currentPassengers = 0;
            PlayerData.PD.currentImportantPassengers = 0;
        }

        // Reload the scene to start fresh
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Debug.Log("Avslutter spillet!");
        Application.Quit();
    }
}
