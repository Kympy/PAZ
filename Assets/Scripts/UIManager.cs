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
    private GameObject OpenText = null; // When mouse over on locked door.
    private TextMeshProUGUI itemCount = null;

    // Game UI
    private CanvasGroup UICanvas = null;
    // ESC
    private CanvasGroup ESCCanvas;
    private Button SaveButton;
    private Button LoadButton;
    private Button MainButton;
    // Lock system UI
    private GameObject LockUICam = null;

    private WaitForSeconds delayTime = new WaitForSeconds(2f);

    public override void Awake() // Find components
    {
        currentBullet = GameObject.Find("CurrentBullet").GetComponent<TextMeshProUGUI>();
        hpBar = GameObject.Find("HPBar").GetComponent<Image>();
        itemCount = GameObject.Find("ItemCount").GetComponent<TextMeshProUGUI>();

        MapCamera = GameObject.FindGameObjectWithTag("MapCamera").GetComponent<Camera>();
        MapCamera.enabled = false;
        MapCamera.depth = 1;

        areaDiscover = GameObject.Find("AreaDiscover").GetComponent<TextMeshProUGUI>();
        areaDiscover.alpha = 0f;

        OpenText = GameObject.Find("OpenText");
        OpenText.SetActive(false);

        UICanvas = GameObject.Find("UICanvas").GetComponent<CanvasGroup>();
        ESCCanvas = GameObject.Find("ESCCanvas").GetComponent<CanvasGroup>();

        SaveButton = ESCCanvas.transform.GetChild(0).GetComponent<Button>();
        LoadButton = ESCCanvas.transform.GetChild(1).GetComponent<Button>();
        MainButton = ESCCanvas.transform.GetChild(2).GetComponent<Button>();

        SaveButton.onClick.AddListener();
        LoadButton.onClick.AddListener();
        MainButton.onClick.AddListener();

        ESCCanvas.alpha = 0f;
        ESCCanvas.interactable = false;

        LockUICam = GameObject.Find("LockUICam");
        LockUICam.SetActive(false);
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
    public void UpdateItemCount(int current)
    {
        itemCount.text = " : " + current.ToString();
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
    public void ToggleLockUI(bool value)
    {
        LockUICam.SetActive(value);
        if (UICanvas.alpha == 1f)
            UICanvas.alpha = 0f;
        else if (UICanvas.alpha == 0f)
            UICanvas.alpha = 1f;
    }
    public void ShowDoorText()
    {
        OpenText.SetActive(true);
        OpenText.transform.position = Input.mousePosition;
    }
    public void HideDoorText()
    {
        OpenText.SetActive(false);
    }
    public void ShowESC(bool boolean)
    {
        if(boolean)
        {
            ESCCanvas.alpha = 1f;
            ESCCanvas.interactable = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0f;
        }
        else
        {
            ESCCanvas.alpha = 0f;
            ESCCanvas.interactable = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1f;
        }
    }
}
