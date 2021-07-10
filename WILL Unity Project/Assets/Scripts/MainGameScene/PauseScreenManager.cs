using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScreenManager : MonoBehaviour
{

    public static bool GameIsPaused = false;
    
    public GameObject fullScreenPanel; 

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            } else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        fullScreenPanel.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void Pause()
    {
        fullScreenPanel.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void OpenSettings()
    {
        SceneManager.LoadSceneAsync("SettingsScene", LoadSceneMode.Additive);
    }

    public void ReturnToTitleScreen()
    {
        SceneManager.LoadSceneAsync("MenuScene");
        Resume();
        // TODO: Add confirmation window
    }
}
