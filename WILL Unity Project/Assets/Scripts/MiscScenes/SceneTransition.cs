using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    static Dictionary<string, SceneTransition> SceneTransitions = new Dictionary<string, SceneTransition>();
    static List<string> AdditiveSceneNames = new List<string>()
    {
        "SettingsScene", "SaveLoadScene", "OutcomeAnimatedTextScene", "StoryHistoryScene"
    };

    public static SceneTransition Instance(string sceneName)
    {
        return SceneTransitions[sceneName];
    }

    RectTransform rectTransform;

    string sceneName;
    bool isAdditive;

    void Awake()
    {
        SceneTransitions.Add(gameObject.scene.name, this);
    }

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        FadeIn();
    }

    public void FadeIn()
    {
        GetComponent<Image>().color = Color.black;
        LeanTween.alpha(rectTransform, 0f, 0.25f).setEaseInQuad();
    }

    public void FadeOut(string sceneName, bool isAdditive)
    {
        this.sceneName = sceneName;
        this.isAdditive = isAdditive;

        LeanTween.alpha(rectTransform, 1f, 0.25f).setEaseOutQuad().setOnComplete(LoadNextScene);
    }

    void LoadNextScene()
    {
        // remove from dictionary if the new scene is a single or returned from additive scene
        if (!isAdditive || sceneName == "") 
        {
            SceneTransitions.Remove(gameObject.scene.name);
        }

        // if the current scene is additive
        if (AdditiveSceneNames.Contains(gameObject.scene.name))
        {
            SceneManager.UnloadSceneAsync(gameObject.scene);
            SceneTransition.Instance(SceneManager.GetSceneAt(SceneManager.sceneCount - 2).name).FadeIn();
        }

        // load new scene
        if (sceneName != "")
        {
            SceneManager.LoadSceneAsync(sceneName, isAdditive ? LoadSceneMode.Additive : LoadSceneMode.Single);
        }
    }
}
