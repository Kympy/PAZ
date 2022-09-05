using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorLock : MonoBehaviour
{
    private Renderer render;
    private Material outline;
    private List<Material> materials = new List<Material>();

    private bool Enter = false;

    private void Awake()
    {
        render = GetComponent<Renderer>();
        outline = new Material(Shader.Find("Custom/Outline"));
        materials.Clear();
        materials.Add(render.sharedMaterial);
    }
    private void OnMouseEnter()
    {
        materials.Add(outline);
        render.materials = materials.ToArray();
    }
    private void OnMouseOver()
    {
        if(Enter)
        {
            UIManager.Instance.ShowDoorText();

            if (InputManager.Instance.E)
            {
                UIManager.Instance.HideDoorText();
                UIManager.Instance.ToggleLockUI(true);
                DoorManager.Instance.SetCurrentDoor(this.gameObject); // Invoke this door's transform to door manager
                Time.timeScale = 0f;
            }
        }
    }
    private void OnMouseExit()
    {
        UIManager.Instance.HideDoorText();
        materials.Remove(outline);
        render.materials = materials.ToArray();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            Enter = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Enter = false;
        }
    }
}
