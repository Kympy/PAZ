using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockPick : MonoBehaviour
{
    private GameObject keyBody = null;
    private GameObject key = null;

    private GameObject smoke = null;

    private float answer = 0f;
    private float mouseX = 0f;
    private float rotationY = 0f;
    private float keyRotY = 0f;

    private float offset = 5f; // Allowance offset

    private float breakTimer = 0f;
    private float brokenTime = 0.3f;

    private const float answerMax = 89f;
    private const float keyRotMax = 90f;

    private Coroutine effectCoroutine = null;

    private bool waitReset = false;

    private Vector3 originKeyPos;

    private void Awake()
    {
        keyBody = GameObject.Find("KeyBody");
        key = GameObject.Find("Pivot");
        smoke = GameObject.Find("KeySmoke");
        smoke.SetActive(false);

        answer = Random.Range(-answerMax, answerMax + 1);

        rotationY = keyBody.transform.localEulerAngles.y;
        rotationY = keyBody.transform.localEulerAngles.y;
        rotationY = rotationY > 180 ? rotationY - 360 : rotationY;

        originKeyPos = key.transform.localPosition;

        key.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
        keyBody.transform.localEulerAngles = new Vector3(0f, -90f, 0f);
    }
    private void OnEnable()
    {
        smoke.SetActive(false);
        answer = Random.Range(-answerMax, answerMax + 1);

        rotationY = keyBody.transform.localEulerAngles.y;
        rotationY = keyBody.transform.localEulerAngles.y;
        rotationY = rotationY > 180 ? rotationY - 360 : rotationY;

        originKeyPos = key.transform.localPosition;

        key.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
        keyBody.transform.localEulerAngles = new Vector3(0f, -90f, 0f);
        waitReset = false;
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)) // Cancel lock sys
        {
            UIManager.Instance.ToggleLockUI(false);
            DoorManager.Instance.ClearCurrentDoor(); // Player doesn't tring unlock door
        }
        if (waitReset == false) // Can't rotate when reset time
        {
            // Get Mouse X
            mouseX = Input.GetAxis("Mouse X");
            // Set Key's rotation
            keyRotY = key.transform.localEulerAngles.y;
            keyRotY = keyRotY > 180 ? keyRotY - 360 : keyRotY;

            if (keyRotY > keyRotMax)
            {
                key.transform.localEulerAngles = new Vector3(0f, keyRotMax, 0);
            }
            else if (keyRotY < -keyRotMax)
            {
                key.transform.localEulerAngles = new Vector3(0f, -keyRotMax, 0);
            }
            //else key.transform.RotateAround(pivot, Vector3.forward, -mouseX * 100 * Time.deltaTime);
            else key.transform.Rotate(0f, mouseX * 100f * Time.deltaTime, 0f);

            // Calculate key body's Y rotation value
            rotationY = keyBody.transform.localEulerAngles.y;
            rotationY = rotationY > 180 ? rotationY - 360 : rotationY;

            if (Input.GetKey(KeyCode.D))
            {
                // Check Answer - current key degree - > 180
                float realAnswer = answer + answerMax; // --> Answer Range (-89 ~ 89) convert to (0 ~ 178)
                float myAnswer = keyRotY + keyRotMax; // --> My Answer Range (-90 ~ 90) convert to (0 ~ 180)
                                                      // Change 90 degree -> Answer / 2  - current / 2
                float canRotate = Mathf.Abs(realAnswer / 2f - myAnswer / 2f); // Degree between answer and my answer (Convert to 180 degree to 90 degree) 
                                                                              // I can rotate 90 - (Answer - current) degree
                canRotate = 90f - canRotate;
                Debug.Log("Answer : " + answer);
                Debug.Log("RealAnswer : " + realAnswer);
                Debug.Log("My Answer : " + myAnswer);

                // Start Rotate At -90f degree
                if (rotationY > 0f) // Max Right
                {
                    StartCoroutine(Success());
                    Debug.LogError("Success!");
                }
                else
                {
                    if (Mathf.Abs(-90f - rotationY) > canRotate + offset) // If current rotation is less than canRotate value
                    {
                        breakTimer += Time.deltaTime;
                        if (breakTimer > brokenTime)
                        {
                            effectCoroutine = StartCoroutine(FailEffect());
                            breakTimer = 0f;
                            waitReset = true;
                            Debug.LogError("Broken!");
                        }
                        Debug.Log("Can Rotate : " + canRotate);
                    }
                    else
                    {
                        keyBody.transform.Rotate(0f, 100f * Time.deltaTime, 0f);
                    }
                }
            }
            else
            {
                breakTimer = 0f;

                if (rotationY < -90f)
                {
                    keyBody.transform.localEulerAngles = new Vector3(0f, -keyRotMax, 0f);
                }
                else if(rotationY > -90f)
                {
                    keyBody.transform.Rotate(0f, -100f * Time.deltaTime, 0f);
                }
            }
        }

        
    }

    private IEnumerator FailEffect()
    {
        if (effectCoroutine != null)
        {
            yield break;
        }
        smoke.SetActive(true);
        smoke.GetComponent<ParticleSystem>().Play();

        yield return new WaitForSecondsRealtime(2f);

        smoke.SetActive(false);

        key.transform.localPosition = originKeyPos;
        key.transform.localEulerAngles = Vector3.zero;
        keyBody.transform.localEulerAngles = new Vector3(0f, -keyRotMax, 0f);

        waitReset = false;
        effectCoroutine = null;
    }

    private IEnumerator Success()
    {
        waitReset = true;
        yield return new WaitForSecondsRealtime(1f);
        UIManager.Instance.ToggleLockUI(false);
        DoorManager.Instance.RemoveOpenedDoor();
    }
}
