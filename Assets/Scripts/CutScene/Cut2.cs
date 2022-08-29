using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Cut2 : MonoBehaviour
{
    private Image panel;
    private TextMeshProUGUI txt;
    [HideInInspector] private Animator _Animator;

    private float timer = 0f;
    private void Awake()
    {
        txt = GameObject.Find("Text").GetComponent<TextMeshProUGUI>();
        panel = GameObject.Find("Panel").GetComponent<Image>();
        panel.color = new Color(0f, 0f, 0f, 255f);
        txt.alpha = 0f;
        _Animator = GameObject.Find("Ghoul").GetComponent<Animator>();
        _Animator.speed = 0f;
    }
    private void Start()
    {
        StartCoroutine(TextUp());
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.Instance.LoadSceneName = "GameScene01";
            SceneManager.LoadScene("LoadingScene");
        }
    }
    private IEnumerator TextUp()
    {
        timer = 0f;
        while(true)
        {
            timer += Time.deltaTime * 0.01f;
            txt.alpha += timer;
            if (txt.alpha >= 1f)
            {
                break;
            }
            yield return null;
        }
        yield return new WaitForSeconds(3f);
        StartCoroutine(TextDown());
    }
    private IEnumerator TextDown()
    {
        timer = 0f;
        while (true)
        {
            timer += Time.deltaTime * 0.01f;
            txt.alpha -= timer;
            if (txt.alpha <= 0f) break;
            yield return null;
        }
        StartCoroutine(PanelDown());
    }
    private IEnumerator PanelDown()
    {
        timer = 0f;
        while(true)
        {
            timer += Time.deltaTime * 0.01f;
            panel.color -= new Color(0f, 0f, 0f, timer);

            if (panel.color.a <= 0f)
            {
                _Animator.speed = 1f;
                break;
            }
            yield return null;
        }
        yield return new WaitForSeconds(10f);
        StartCoroutine(PanelUp());
    }
    private IEnumerator PanelUp()
    {
        timer = 0f;
        while (true)
        {
            timer += Time.deltaTime * 0.01f;
            panel.color += new Color(0f, 0f, 0f, timer);

            if (panel.color.a >= 1f)
            {
                GameManager.Instance.LoadSceneName = "GameScene01";
                SceneManager.LoadScene("LoadingScene");
                yield break;
            }
            yield return null;
        }
    }
}
