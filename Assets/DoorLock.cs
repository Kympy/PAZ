using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorLock : MonoBehaviour
{
    private void OnMouseOver()
    {
        UIManager.Instance.ShowDoorText();
        
        if(InputManager.Instance.E)
        {
            UIManager.Instance.HideDoorText();
            UIManager.Instance.ToggleLockUI(true);
            DoorManager.Instance.SetCurrentDoor(this.gameObject); // Invoke this door's transform to door manager
        }
    }
    private void OnMouseExit()
    {
        UIManager.Instance.HideDoorText();
    }
}
