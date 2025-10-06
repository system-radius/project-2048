using TMPro;
using UnityEngine;

public class TimeDisplay : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI minutesTens;

    [SerializeField]
    private TextMeshProUGUI secondsTens;

    [SerializeField]
    private TextMeshProUGUI minutesOnes;

    [SerializeField]
    private TextMeshProUGUI secondsOnes;

    private bool running = false;
    private float currentTime = -1;

    private void Update()
    {
        if (!running) return;
        currentTime -= Time.deltaTime;

        if (currentTime < 0)
        {
            currentTime = 0;
            running = false;
        }
        UpdateTime();
    }

    private void UpdateTime()
    {
        float minutes = currentTime / 60;
        minutesTens.text = Mathf.FloorToInt(minutes / 10f).ToString();
        minutesOnes.text = Mathf.FloorToInt(minutes % 10f).ToString();

        float seconds = currentTime % 60;
        secondsTens.text = Mathf.FloorToInt(seconds / 10f).ToString();
        secondsOnes.text = Mathf.FloorToInt(seconds % 10f).ToString();
        //Debug.LogError("Current time: " + currentTime + ", " + minutesTens.text + "" + minutesOnes.text + ":" + secondsTens.text + "" + secondsOnes.text);
    }

    public void SetValue(float time)
    {
        running = true;
        currentTime = time;
    }
}