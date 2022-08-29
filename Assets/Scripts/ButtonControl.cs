using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonControl : MonoBehaviour
{
    private Button PlayButton;
    private Button ExitButton;

    private void Awake()
    {
        PlayButton = GameObject.Find("PlayButton").GetComponent<Button>();
        ExitButton = GameObject.Find("ExitButton").GetComponent<Button>();
    }

    private void Start()
    {
        PlayButton.onClick.AddListener(() => GameScene());
        ExitButton.onClick.AddListener(() => ExitGame());
    }
    private void GameScene()
    {
        GameManager.Instance.LoadSceneName = "CutScene";
        SceneManager.LoadScene("LoadingScene");
    }
    private void ExitGame()
    {
        Application.Quit();
    }
}
