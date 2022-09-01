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
    private Camera MapCamera = null;
    private TextMeshProUGUI areaDiscover = null;

    private WaitForSeconds delayTime = new WaitForSeconds(2f);

    public override void Awake() // Find components
    {
        currentBullet = GameObject.Find("CurrentBullet").GetComponent<TextMeshProUGUI>();
        hpBar = GameObject.Find("HPBar").GetComponent<Image>();
        MapCamera = GameObject.FindGameObjectWithTag("MapCamera").GetComponent<Camera>();
        MapCamera.enabled = false;
        areaDiscover = GameObject.Find("AreaDiscover").GetComponent<TextMeshProUGUI>();
        areaDiscover.alpha = 0f;
    }
    public void SetBulletUI(int current, int have) // Show current bullet
    {
        currentBullet.text = current.ToString() + " / " + have.ToString();
        if (current == 0)
        {
            currentBullet.color = Color.red;
        }
        else currentBullet.color = Color.white;
    }
    public void UpdateBar(float current, float max) // Update coroutine start
    {
        UICoroutine = StartCoroutine(UpdateHPBar(current, max));
    }
    public void StopUpdateBar() // Stop already executing coroutine
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
    public void ShowMap(bool show)
    {
        MapCamera.enabled = show;
    }
    public IEnumerator FindArea(string name)
    {
        areaDiscover.text = "Discovered New Area : " + name;
        while (true)
        {
            areaDiscover.alpha += 0.05f;
            if (areaDiscover.alpha >= 1f) break;
            yield return null;
        }

        yield return delayTime;

        while(true)
        {
            areaDiscover.alpha -= 0.05f;
            if (areaDiscover.alpha <= 0f)
            {
                areaDiscover.text = "";
                break;
            }
            yield return null;
        }
    }
}
