using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    
    public void NewGame() {
        SceneManager.LoadSceneAsync("MainGameScene");
    }

    public void LoadGame() {
        SaveLoadManager.isSaving = false;
        SceneManager.LoadSceneAsync("SaveLoadScene", LoadSceneMode.Additive);
    }

    public void Settings() {
        SceneManager.LoadSceneAsync("SettingsScene", LoadSceneMode.Additive);        
    }

    public void Exit() {
        // TODO: Add confirmation window
        Application.Quit();
    }

}
