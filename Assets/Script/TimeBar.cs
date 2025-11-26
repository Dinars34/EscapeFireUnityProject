using UnityEngine;
using UnityEngine.UI;

public class TimeBar : MonoBehaviour
{
    public Slider slider;
    public Text timeText; // Optional: to show time as numbers (MM:SS)

    public void SetMaxTime(int time) 
    {
        slider.maxValue = time;
        slider.value = time;
        UpdateTimeText(time);
    }

    public void SetTime(int time) 
    {
        slider.value = time;
        UpdateTimeText(time);
    }
    
    void UpdateTimeText(int seconds)
    {
        // If you have a Text component to show time
        if (timeText != null)
        {
            int minutes = seconds / 60;
            int secs = seconds % 60;
            timeText.text = string.Format("{0:00}:{1:00}", minutes, secs);
        }
    }
}