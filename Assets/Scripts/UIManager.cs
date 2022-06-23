using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    GameObject titleScreen;

    [SerializeField]
    GameObject inGameScreen;

    [SerializeField]
    GameObject gameOverScreen;

    [SerializeField]
    TextMeshProUGUI dayText;

    [SerializeField]
    TextMeshProUGUI liveText;

    [SerializeField]
    TextMeshProUGUI breakCountdownText;

    [SerializeField]
    TextMeshProUGUI breakTimeText;

    // Toggle title screen
    public void ToggleTitleScreen(bool isOn)
    {
        titleScreen.SetActive(isOn);
    }

    // Toggle in-game screen
    public void ToggleInGameScreen(bool isOn)
    {
        inGameScreen.SetActive(isOn);
    }

    // Toggle game over screen
    public void ToggleGameOverScreen(bool isOn)
    {
        gameOverScreen.SetActive(isOn);
    }

    // Update day text
    public void UpdateDayText(int days)
    {
        dayText.SetText("Day {0}", days);
    }

    // Update live text
    public void UpdateLiveText(int lives)
    {
        liveText.SetText("Live x{0}", lives);
    }

    // Update countdown of break time
    public void UpdateBreakCountdownText(int currentCount)
    {
        breakCountdownText.SetText("" + currentCount);
    }

    // Toggle break time notification
    public void ToggleBreakTime(bool isOn)
    {
        breakTimeText.gameObject.SetActive(isOn);
        breakCountdownText.gameObject.SetActive(isOn);
    }
}
