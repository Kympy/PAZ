using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class BlinkTextColor : MonoBehaviour
{
    private TextMeshProUGUI loadingText;
    private bool up = true;

    private void Awake()
    {
        loadingText = GetComponent<TextMeshProUGUI>();
        loadingText.alpha = 0f;
    }
    private void Start()
    {
        StartCoroutine(BlinkText());
    }
    private IEnumerator BlinkText()
    {
        float alpha = loadingText.alpha;
        while(true)
        {
            if(up)
            {
                loadingText.alpha += 0.01f;
                if (loadingText.alpha > 1f)
                {
                    loadingText.alpha = 1f;
                    up = false;
                }
            }
            else
            {
                loadingText.alpha -= 0.01f;
                if (loadingText.alpha < 0f)
                {
                    loadingText.alpha = 0f;
                    up = true;
                }
            }
            yield return null;
        }
    }
}
