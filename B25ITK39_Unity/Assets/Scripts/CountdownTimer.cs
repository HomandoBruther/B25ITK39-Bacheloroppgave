using UnityEngine;
using TMPro;
using System.Collections;

public class CountdownTimer : MonoBehaviour
{
    [Header("Timer Settings")]
    public float timeMultiplier = 1.0f; // Multiplier to adjust difficulty in Inspector
    private float countdownTime;
    private bool isCountingDown = false;
    private bool hasStarted = false; // Tracks if the timer has started at least once

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI timerText; // Assign in Inspector

    // Initialize the timer UI state
    private void Start()
    {
        if (timerText != null)
        {
            timerText.gameObject.SetActive(false);
        }
    }

    // Updates countdown logic every frame
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

    // Starts the countdown based on distance and difficulty multiplier
    public void StartCountdown(float distance)
    {
        StartCoroutine(FlashTimer());
        countdownTime = distance * timeMultiplier;
        isCountingDown = true;

        if (!hasStarted)
        {
            hasStarted = true;
            if (timerText != null)
            {
                timerText.gameObject.SetActive(true);
            }
        }
    }

    // Updates the UI to display the remaining time
    private void UpdateTimerUI()
    {
        if (timerText != null)
        {            
            timerText.text = "Time: " + Mathf.Ceil(countdownTime).ToString();
            if ((Mathf.Ceil(countdownTime) % 2) == 0) {
                if (Mathf.Ceil(countdownTime) > 30 )
                {
                timerText.color = Color.green;
                }
                else timerText.color = Color.red;
                }
            else timerText.color = Color.yellow;
        }
    }

    // Handles logic when the timer reaches zero
    private void TimerExpired()
    {
        if (EndZone.instance != null)
        {
            EndZone.instance.EndGame();
        }
    }

    private IEnumerator FlashTimer(int flashes = 10, float flashDuration = 0.1f)
{
    Color originalColor = timerText.color;

    for (int i = 0; i < flashes; i++)
    {
        timerText.color = Color.yellow;
        yield return new WaitForSeconds(flashDuration);
        timerText.color = Color.red;
        yield return new WaitForSeconds(flashDuration);
    }

    timerText.color = originalColor; // Reset to default
}
}
