using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonControl : MonoBehaviour
{
    private Button PlayButton;
    private Button ExitButton;
    private Button LoadButton;

    private void Awake()
    {
        PlayButton = GameObject.Find("PlayButton").GetComponent<Button>();
        LoadButton = GameObject.Find("LoadButton").GetComponent<Button>();
        ExitButton = GameObject.Find("ExitButton").GetComponent<Button>();
    }

    private void Start()
    {
        PlayButton.onClick.AddListener(() => GameScene());
        LoadButton.onClick.AddListener(() => LoadGame());
        ExitButton.onClick.AddListener(() => ExitGame());
    }
    private void GameScene()
    {
        GameManager.Instance.willLoadData = false;
        GameManager.Instance.LoadSceneName = "CutScene";
        SceneManager.LoadScene("LoadingScene");
    }
    private void LoadGame()
    {
        GameManager.Instance.GoToLoad();
    }
    private void ExitGame()
    {
        Application.Quit();
    }
}
