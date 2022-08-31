using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : Singleton<UIManager>
{
    private TextMeshProUGUI currentBullet = null;

    public override void Awake()
    {
        currentBullet = GameObject.Find("CurrentBullet").GetComponent<TextMeshProUGUI>();
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
}
