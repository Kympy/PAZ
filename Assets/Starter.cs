using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Starter : MonoBehaviour
{
    private void Start()
    {
        GameManager.Instance.StartGame();
    }
}
