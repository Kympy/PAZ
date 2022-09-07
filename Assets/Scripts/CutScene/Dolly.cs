using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Dolly : MonoBehaviour
{
    // My Carts
    private CinemachineDollyCart cart;
    private CinemachineDollyCart cart2;
    // Actor Character
    private Animator asianAnim;
    private Animator white2Anim;
    private Animator blackAnim;
    // Loading operation
    private AsyncOperation op = null;
    // Effect Object
    private GameObject blood;
    private GameObject Crate;
    private GameObject muzzle;
    private Light cell;
    private Image panel;
    // Boolean Flag
    private bool doLight = false;
    private bool doBlack = false;
    private bool load = false;
    // Text UI
    private TextMeshProUGUI center;
    private TextMeshProUGUI bottom;
    private float a = 0f;

    private void Awake() // Finds
    {
        cart = GameObject.Find("Cart1").GetComponent<CinemachineDollyCart>();
        cart2 = GameObject.Find("Cart2").GetComponent<CinemachineDollyCart>();

        asianAnim = GameObject.Find("Asian").GetComponent<Animator>();
        white2Anim = GameObject.Find("White2").GetComponent<Animator>();
        blackAnim = GameObject.Find("Black").GetComponent<Animator>();
        muzzle = GameObject.FindGameObjectWithTag("Muzzle");
        muzzle.SetActive(false);
        center = GameObject.Find("CenterText").GetComponent<TextMeshProUGUI>();
        bottom = GameObject.Find("BottomText").GetComponent<TextMeshProUGUI>();

        blood = ResourceDataObj.Instance.Blood;
        Crate = GameObject.Find("Crate");
        cell = GameObject.Find("Cell").GetComponentInChildren<Light>();
        panel = GameObject.Find("Panel").GetComponent<Image>();
    }
    private void Start() // Set Cart Speed And Start Coroutine
    {
        cart.m_Speed = 9f;
        center.text = "";
        center.alpha = 0f;
        bottom.text = "";
        StartCoroutine(TextCoroutine());
        StartCoroutine(LoadCut2());
    }
    private void FixedUpdate()
    {
        if(doLight) // Cell object's light effect
        {
            cell.intensity = Mathf.PingPong(Time.time * 2f, 3f);
        }
        if(doBlack) // UI alpha turn to black
        {
            a += Time.deltaTime;
            panel.color += new Color(0f, 0f, 0f, a * 0.01f);
            if(panel.color.a >= 1f)
            {
                op.allowSceneActivation = true;
            }
        }
    }
    // My texts
    private string[] texts =
    {
        "Zombie Survival",
        "Created By KYM",
        "All Assets By Unity"
    };
    private IEnumerator TextCoroutine() // First subtitle's showing coroutine
    {
        int i = 0;
        float timer = 0f; // Init
    
        while(true)
        {
            timer += Time.deltaTime; // Check time for next subtitles
            center.alpha = Mathf.PingPong(timer * 0.5f, 1f); // Text Blink effect
            center.text = texts[i]; // Enter new text from 'my texts'
            if(timer > 4f) // For 4 seconds
            {
                timer = 0f;
                i++;
                if (i >= texts.Length) break; // No more texts,
            }

            yield return new WaitForFixedUpdate();
        }
        center.text = ""; // Start displaying subtitles
        yield return new WaitForSeconds(2f);
        bottom.text = "Classified Island, 2010";
        yield return new WaitForSeconds(4f);
        bottom.text = "";
        yield return new WaitForSeconds(3f);
        bottom.text = "One scientist is involved in an organization's classified project.";
        yield return new WaitForSeconds(5f);
        bottom.text = "";
        StartCoroutine(TalkCoroutine()); // Start Next Dialog coroutine
    }
    private IEnumerator TalkCoroutine() // Dialog
    {
        yield return new WaitForSeconds(3f);
        asianAnim.SetTrigger("IsTalk");
        bottom.text = "We promised, I did everything you told me!";
        yield return new WaitForSeconds(4f);
        bottom.text = "Why are you doing this?";
        yield return new WaitForSeconds(3f);
        white2Anim.SetTrigger("IsTalk");
        bottom.text = "Sir,";
        yield return new WaitForSeconds(2f);
        bottom.text = "(Tell a secret)";
        yield return new WaitForSeconds(2f);
        bottom.text = "";
        yield return new WaitForSeconds(3f);
        blackAnim.SetTrigger("IsShoot");
        bottom.text = "- Hmm..\n- Oh, Please..";
        yield return new WaitForSeconds(1f);
        asianAnim.SetTrigger("IsDead");
        muzzle.SetActive(true); // ================> Effect Time
        yield return new WaitForSeconds(0.5f);
        muzzle.SetActive(false);
        yield return new WaitForSeconds(2.5f);
        bottom.text = "";
        yield return new WaitForSeconds(1f);
        cart2.m_Speed = 0.2f;
        bottom.text = "Take it.";
        yield return new WaitForSeconds(4.5f);
        bottom.text = "";
        Instantiate(blood, new Vector3(220.298767f, -8.92500019f, 67.552002f), blood.transform.rotation);
        Crate.SetActive(false);
        yield return new WaitForSeconds(6f);
        doLight = true;
        yield return new WaitForSeconds(3f);
        doBlack = true;
    }
    private IEnumerator LoadCut2() // Load Cut Scene 2 already
    {
        op = SceneManager.LoadSceneAsync("CutScene2"); // Load Start
        op.allowSceneActivation = false; // Not allow load
        while(true)
        {
            if (Input.GetKeyDown(KeyCode.Escape) || load) // ESC to Skip
            {
                op.allowSceneActivation = true; // Allow load
                yield break;
            }

            yield return null;
        }
    }
}
