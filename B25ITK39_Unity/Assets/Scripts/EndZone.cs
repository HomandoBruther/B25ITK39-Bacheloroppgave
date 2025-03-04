using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // For UI text handling
using System.Collections.Generic;
using System.IO;
using System.Linq;


public class EndZone : MonoBehaviour
{
    public GameObject endGameUI;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI leaderboardText; // Add this in the Inspector!

    private List<int> leaderboard = new List<int>(); // Stores scores

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

            // Get current score
            int currentScore = PlayerData.PD.points;
            scoreText.text = "Score: " + currentScore.ToString();

            // Save and update leaderboard
            UpdateLeaderboard(currentScore);
        }
    }
    public class LeaderboardData
    {
        public List<int> scores = new List<int>();
    }

    private void UpdateLeaderboard(int newScore)
    {
        leaderboard = LoadLeaderboard();
        leaderboard.Add(newScore);
        leaderboard.Sort((a, b) => b.CompareTo(a));

        // Keep only top 5
        if (leaderboard.Count > 5)
            leaderboard.RemoveAt(leaderboard.Count - 1);

        SaveLeaderboard();

        // Display leaderboard
        leaderboardText.text = "Leaderboard\n";
        for (int i = 0; i < leaderboard.Count; i++)
        {
            string highlight = leaderboard[i] == newScore ? "<color=yellow>" : "";
            string endHighlight = leaderboard[i] == newScore ? "</color>" : "";
            leaderboardText.text += $"{highlight}{i + 1}. {leaderboard[i]}{endHighlight}\n";
        }

        // Show player rank if they are not in the top 5
        if (!leaderboard.Contains(newScore))
        {
            int playerRank = leaderboard.IndexOf(newScore) + 1;
            leaderboardText.text += $"\nYour Rank: {playerRank}";
        }
    }
    

    private void SaveLeaderboard()
    {
        LeaderboardData data = new LeaderboardData { scores = leaderboard };
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + "/leaderboard.json", json);
    }

    private List<int> LoadLeaderboard()
    {
        string path = Application.persistentDataPath + "/leaderboard.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            LeaderboardData data = JsonUtility.FromJson<LeaderboardData>(json);
            return data.scores.OrderByDescending(s => s).Take(5).ToList();
        }
        return new List<int>();
    }

    public void PlayAgain()
    {
        Time.timeScale = 1f;
        PlayerData.PD.points = 0;
        PlayerData.PD.currentPassengers = 0;
        PlayerData.PD.currentImportantPassengers = 0;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}