using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingSceneManager : MonoBehaviour
{
    private Image loadingBar = null;

    private void Awake()
    {
        loadingBar = GameObject.Find("LoadingBar").GetComponent<Image>();
    }
    private  IEnumerator Loading<T>(T SceneIndex) where T : struct
    {
        float timer = 0f;

        AsyncOperation op = SceneManager.LoadSceneAsync(0);
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
