using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapIconActive : MonoBehaviour
{
    private Collider playerEnterTrigger = null;
    private Renderer iconRenderer = null;
    private void Awake()
    {
        iconRenderer = GetComponent<Renderer>();
        playerEnterTrigger = GetComponent<Collider>();

        iconRenderer.enabled = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            iconRenderer.enabled = true;
            playerEnterTrigger.enabled = false;
            StartCoroutine(UIManager.Instance.FindArea(this.name));
        }
    }
}
