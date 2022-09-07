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

    private void Awake() // Find my buttons
    {
        PlayButton = GameObject.Find("PlayButton").GetComponent<Button>();
        LoadButton = GameObject.Find("LoadButton").GetComponent<Button>();
        ExitButton = GameObject.Find("ExitButton").GetComponent<Button>();
    }

    private void Start() // Set Functions to each button
    {
        PlayButton.onClick.AddListener(() => GameScene());
        LoadButton.onClick.AddListener(() => LoadGame());
        ExitButton.onClick.AddListener(() => ExitGame());
    }
    private void GameScene() // New Game Button
    {
        GameManager.Instance.willLoadData = false; // Data load false
        GameManager.Instance.LoadSceneName = "CutScene"; // Show Cutscene
        SceneManager.LoadScene("LoadingScene"); // Call Loading Scene
    }
    private void LoadGame() // Load Button
    {
        GameManager.Instance.GoToLoad(); // Load Data from local path
    }
    private void ExitGame() // Exit Button
    {
        Application.Quit(); // Go out to window
    }
}
