using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerScript : MonoBehaviour
{
    public Text timerText;
    private float currentTime;
    private bool isCounting = false;

    public void StartCountdown(float time)
    {
        currentTime = time;
        isCounting = true;
    }

    void Update()
    {
        if (isCounting)
        {
            currentTime -= Time.deltaTime;

            currentTime = Mathf.Max(0f, currentTime);

            UpdateTimerText();

            if (currentTime == 0f)
            {
                isCounting = false;
                timerText.text = "Your turn end";
            }
        }
    }

    void UpdateTimerText()
    {
        int seconds = Mathf.FloorToInt(currentTime);
        string timeString = seconds.ToString();
        timerText.text = "Time: " + timeString;
    }
}
