using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool IsGameOver { get; private set; }
    public int Days { get; private set; }

    public int lives = 5;

    bool isInBreak = false;

    int sceneBuildIndex;

    float totalTime = 24;           // in seconds
    float accumulatedTime = 0f;

    float breakTime = 5;            // in seconds
    float breakModifier = 1;
    float breakElapsedTime = 0;

    SpawnManager spawnManager;
    PlayerController playerController;
    UIManager uiManager;
    Material backgroundMaterial;
    
    Vector3 originalBgTextureOffset;

    // Start is called before the first frame update
    void Start()
    {
        IsGameOver = true;
        Days = 0;

        sceneBuildIndex = SceneManager.GetActiveScene().buildIndex;

        spawnManager = GameObject.Find("Spawn Manager").GetComponent<SpawnManager>();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        uiManager = GameObject.Find("UI Manager").GetComponent<UIManager>();
        backgroundMaterial = GameObject.Find("Background").GetComponent<Renderer>().material;

        originalBgTextureOffset = backgroundMaterial.mainTextureOffset;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsGameOver)
        {
            if (!isInBreak || breakElapsedTime <= 0)
            {
                if (!spawnManager.IsSpawning)
                {
                    spawnManager.StartSpawning();
                }
                SimulateTimePassing(Time.deltaTime);
            }
            else
            {
                SimulateBreakTime(Time.deltaTime);
            }
        }
    }

    public void StartGame()
    {
        IsGameOver = false;

        // Update ui
        uiManager.ToggleTitleScreen(false);
        uiManager.ToggleInGameScreen(true);
        uiManager.UpdateDayText(Days);
        uiManager.UpdateLiveText(lives);

        // Update player's visibility
        playerController.ToggleOnOff(true);
        playerController.IsActive = true;

        // Spawn enemies
        spawnManager.StartSpawning();
    }

    public void EndGame()
    {
        IsGameOver = true;

        spawnManager.StopSpawning();
        uiManager.ToggleGameOverScreen(true);
    }

    public void RestartGame()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        SceneManager.LoadScene(sceneBuildIndex);
    }

    // Change background's texture offset overtime to cycle day/night section
    void ChangeBackground(float offsetX)
    {
        backgroundMaterial.mainTextureOffset = new Vector2(offsetX, originalBgTextureOffset.y);
    }

    // Simulate the passing of time: day -> night -> day -> ...
    void SimulateTimePassing(float deltaTime)
    {
        accumulatedTime += deltaTime;
        if (accumulatedTime > totalTime)
        {
            uiManager.UpdateDayText(++Days);
            accumulatedTime -= totalTime;

            // Break
            if (Days % breakModifier == 0)
            {
                isInBreak = true;
                breakElapsedTime = breakTime;

                spawnManager.StopSpawning();
                uiManager.ToggleBreakTime(true);
            }
        }

        ChangeBackground(Mathf.Lerp(originalBgTextureOffset.x, originalBgTextureOffset.x + 1, accumulatedTime / totalTime));
    }

    // Simulate break time in between
    void SimulateBreakTime(float deltaTime)
    {
        var newBreakElapsedTime = breakElapsedTime - deltaTime;
        if (newBreakElapsedTime < 0 || (int)breakElapsedTime != (int)newBreakElapsedTime)
        {
            if (newBreakElapsedTime <= 0)
            {
                // End break time
                isInBreak = false;
                newBreakElapsedTime = 0;

                uiManager.UpdateBreakCountdownText(0);
                StartCoroutine(HideBreakTimeCountdownRoutine());
            }
            else
            {
                uiManager.UpdateBreakCountdownText((int)breakElapsedTime);
            }
        }
        breakElapsedTime = newBreakElapsedTime;
    }

    // Routine to hide break time countdown
    IEnumerator HideBreakTimeCountdownRoutine()
    {
        yield return new WaitForSeconds(1);
        uiManager.ToggleBreakTime(false);
    }
}
