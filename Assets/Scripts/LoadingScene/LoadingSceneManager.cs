using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingSceneManager : MonoBehaviour
{
    private Image loadingBar = null;
    private string sceneName; // Desired scene name

    private void Awake()
    {
        loadingBar = GameObject.Find("LoadingBar").GetComponent<Image>();
        loadingBar.fillAmount = 0f;
    }
    private void Start()
    {
        StartCoroutine(Loading());
    }
    private  IEnumerator Loading()
    {
        sceneName = GameManager.Instance.LoadSceneName;

        if(sceneName == "")
        {
            Debug.LogError("There's no scene name to load.");
            yield break;
        }

        float timer = 0f;
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        while(op.isDone == false)
        {
            timer += Time.deltaTime;
            if(op.progress < 0.9f)
            {
                loadingBar.fillAmount = Mathf.Lerp(loadingBar.fillAmount, op.progress, timer);
                if(loadingBar.fillAmount > op.progress)
                {
                    timer = 0f;
                }
            }
            else
            {
                loadingBar.fillAmount = Mathf.Lerp(loadingBar.fillAmount, 1.0f, timer);
                if(loadingBar.fillAmount >= 1.0f)
                {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
            yield return null;
        }
    }
}
