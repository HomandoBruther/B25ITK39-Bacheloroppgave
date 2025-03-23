using UnityEngine;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    [Header("Timer Settings")]
    public float timeMultiplier = 1.0f; // Multiplier to adjust difficulty in Inspector
    private float countdownTime;
    private bool isCountingDown = false;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI timerText; // Assign in Inspector

    private void Update()
    {
        if (isCountingDown)
        {
            countdownTime -= Time.deltaTime;
            UpdateTimerUI();

            if (countdownTime <= 0)
            {
                countdownTime = 0;
                isCountingDown = false;
                TimerExpired();
            }
        }
    }

    public void StartCountdown(float distance)
    {
        countdownTime = distance * timeMultiplier; // Adjust time based on difficulty
        isCountingDown = true;
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            timerText.text = Mathf.Ceil(countdownTime).ToString();
        }
    }

    private void TimerExpired()
    {
        Debug.Log("⏳ Timer expired! Ending game.");

        if (EndZone.instance != null)
        {
            EndZone.instance.EndGame(); // Triggers the same end-game sequence
        }
    }
}
