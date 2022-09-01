using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : Singleton<UIManager>
{
    private TextMeshProUGUI currentBullet = null;
    private Image hpBar = null;
    public Coroutine UICoroutine = null;

    public override void Awake()
    {
        currentBullet = GameObject.Find("CurrentBullet").GetComponent<TextMeshProUGUI>();
        hpBar = GameObject.Find("HPBar").GetComponent<Image>();
    }
    public void SetBulletUI(int current, int have)
    {
        currentBullet.text = current.ToString() + " / " + have.ToString();
        if (current == 0)
        {
            currentBullet.color = Color.red;
        }
        else currentBullet.color = Color.white;
    }
    public void UpdateBar(float current, float max)
    {
        UICoroutine = StartCoroutine(UpdateHPBar(current, max));
    }
    public void StopUpdateBar()
    {
        if(UICoroutine != null)
        StopCoroutine(UICoroutine);
    }
    private IEnumerator UpdateHPBar(float current, float max)
    {
        while(true)
        {
            hpBar.fillAmount = Mathf.Lerp(hpBar.fillAmount, current / max, Time.deltaTime * 3f);
            if (Mathf.Approximately(hpBar.fillAmount, current / max)) yield break;
            yield return null;
        }
    }
}
