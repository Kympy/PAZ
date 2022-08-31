using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Dolly : MonoBehaviour
{
    private CinemachineDollyCart cart;
    private CinemachineDollyCart cart2;
    private CinemachineSmoothPath track;

    private Animator asianAnim;
    private Animator white2Anim;
    private Animator blackAnim;

    private AsyncOperation op = null;

    private GameObject blood;
    private GameObject Crate;
    private GameObject muzzle;
    private Light cell;
    private Image panel;

    private float timer = 0f;

    private bool first = false;
    private bool second = false;
    private bool third = false;
    private bool doLight = false;
    private bool doBlack = false;
    private bool load = false;

    private TextMeshProUGUI center;
    private TextMeshProUGUI bottom;
    private float a = 0f;

    private void Awake()
    {
        cart = GameObject.Find("Cart1").GetComponent<CinemachineDollyCart>();
        cart2 = GameObject.Find("Cart2").GetComponent<CinemachineDollyCart>();
        track = GameObject.Find("Track1").GetComponent<CinemachineSmoothPath>();

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
    private void Start()
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
        if(doLight)
        {
            cell.intensity = Mathf.PingPong(Time.time * 2f, 3f);
        }
        if(doBlack)
        {
            a += Time.deltaTime;
            panel.color += new Color(0f, 0f, 0f, a * 0.01f);
            if(panel.color.a >= 1f)
            {
                op.allowSceneActivation = true;
            }
        }
    }
    private string[] texts =
    {
        "Zombie Survival",
        "Created By KYM",
        "All Assets By Unity"
    };
    private IEnumerator TextCoroutine()
    {
        int i = 0;
        float timer = 0f;
        while(true)
        {
            timer += Time.deltaTime;
            center.alpha = Mathf.PingPong(Time.time * 0.5f, 1f);
            center.text = texts[i];
            if(timer > 4f)
            {
                timer = 0f;
                i++;
                if (i >= texts.Length) break;
            }

            yield return new WaitForFixedUpdate();
        }
        center.text = "";
        yield return new WaitForSeconds(2f);
        bottom.text = "Classified Island, 2010";
        yield return new WaitForSeconds(4f);
        bottom.text = "";
        yield return new WaitForSeconds(3f);
        bottom.text = "One scientist is involved in an organization's classified project.";
        yield return new WaitForSeconds(5f);
        bottom.text = "";
        StartCoroutine(TalkCoroutine());
    }
    private IEnumerator TalkCoroutine()
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
        muzzle.SetActive(true);
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
    private IEnumerator LoadCut2()
    {
        op = SceneManager.LoadSceneAsync("CutScene2");
        op.allowSceneActivation = false;
        while(true)
        {
            if (Input.GetKeyDown(KeyCode.Escape) || load)
            {
                op.allowSceneActivation = true;
                yield break;
            }

            yield return null;
        }
    }
}
