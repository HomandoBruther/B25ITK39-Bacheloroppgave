using UnityEngine;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText; // Reference to the timer UI
    [SerializeField] private float timeMultiplier = 1.0f; // ✅ Multiplier stays

    private float timeRemaining;
    private bool isCounting = false;

    private void Start()
    {
        timerText.gameObject.SetActive(false); // ✅ Hide timer UI at the start
    }

    public void StartCountdown(float baseDuration)
    {
        timeRemaining = baseDuration * timeMultiplier; // ✅ Apply multiplier
        isCounting = true;
        timerText.gameObject.SetActive(true); // ✅ Show timer UI when countdown starts
    }

    private void Update()
    {
        if (isCounting && timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            timerText.text = Mathf.CeilToInt(timeRemaining).ToString();
        }
        else if (timeRemaining <= 0)
        {
            isCounting = false;
            timerText.gameObject.SetActive(false); // ✅ Hide timer UI when time runs out
        }
    }
}
