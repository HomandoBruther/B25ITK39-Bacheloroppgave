using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // For UI text
using UnityEngine.UI; // For Button UI

[System.Serializable]
public class LeaderboardEntry
{
    public string username;
    public int score;
}

[System.Serializable]
public class LeaderboardData
{
    public List<LeaderboardEntry> entries = new List<LeaderboardEntry>();
}

public class LeaderboardManager : MonoBehaviour
{
    public TMP_InputField usernameInput; // Assign in Inspector
    public TextMeshProUGUI leaderboardText; // Assign in Inspector
    public Button submitButton; // Assign in Inspector

    private List<LeaderboardEntry> leaderboard = new List<LeaderboardEntry>();
    private bool hasSubmittedScore = false; // Prevent multiple submissions

    private string GetFilePath() => Application.persistentDataPath + "/leaderboard.json";

    private void Start()
    {
        leaderboard = LoadLeaderboard();
        DisplayLeaderboard();

        // Enforce character limit in InputField
        if (usernameInput != null)
        {
            usernameInput.characterLimit = 10;
        }
    }

    public void SubmitScoreFromUI()
    {
        if (hasSubmittedScore)
        {
            Debug.Log("Score already submitted!");
            return;
        }

        string playerName = usernameInput.text.Trim();

        // Check if username exceeds 10 characters
        if (playerName.Length > 10)
        {
            Debug.Log("Username must be 10 characters or less!");
            return;
        }

        int currentScore = PlayerData.PD.points;
        SubmitScore(currentScore);

        hasSubmittedScore = true; // Mark submission as done
        if (usernameInput != null) usernameInput.interactable = false; // Disable input field
        if (submitButton != null) submitButton.interactable = false; // Disable submit button
    }

    public void SubmitScore(int newScore)
    {
        string playerName = usernameInput.text.Trim();
        if (string.IsNullOrWhiteSpace(playerName))
            playerName = "Player"; // Default name

        Debug.Log($"Submitting Score: {playerName} - {newScore}");

        // ✅ Load existing leaderboard first
        List<LeaderboardEntry> existingLeaderboard = LoadLeaderboard();

        // ✅ Add new score to the list
        existingLeaderboard.Add(new LeaderboardEntry { username = playerName, score = newScore });

        // ✅ Sort & keep only the top 10 scores
        leaderboard = existingLeaderboard.OrderByDescending(e => e.score).Take(10).ToList();

        // ✅ Save updated leaderboard
        SaveLeaderboard();

        // ✅ Display updated leaderboard
        DisplayLeaderboard();
    }

    private void SaveLeaderboard()
    {
        LeaderboardData data = new LeaderboardData { entries = leaderboard };
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(GetFilePath(), json);

        Debug.Log("Leaderboard saved: " + json); // Log saved data
    }

    private List<LeaderboardEntry> LoadLeaderboard()
    {
        string path = GetFilePath();
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            LeaderboardData data = JsonUtility.FromJson<LeaderboardData>(json);

            Debug.Log("Loaded leaderboard: " + json);

            return data.entries.OrderByDescending(e => e.score).Take(10).ToList();
        }

        Debug.Log("No leaderboard file found. Creating new one.");
        return new List<LeaderboardEntry>(); // ✅ Return an empty list instead of null
    }

    public void DisplayLeaderboard()
    {
        Debug.Log("Displaying leaderboard...");
        leaderboardText.text = "Leaderboard\n";

        for (int i = 0; i < leaderboard.Count; i++)
        {
            Debug.Log($"Rank {i + 1}: {leaderboard[i].username} - {leaderboard[i].score}");
            leaderboardText.text += $"{i + 1}. {leaderboard[i].username} - {leaderboard[i].score}\n";
        }

        if (leaderboard.Count == 0)
        {
            Debug.Log("Leaderboard is empty!");
            leaderboardText.text += "No Scores Yet!";
        }
    }

    public void ResetSubmission()
    {
        hasSubmittedScore = false;
        if (usernameInput != null) usernameInput.interactable = true;
        if (submitButton != null) submitButton.interactable = true;
    }
}
