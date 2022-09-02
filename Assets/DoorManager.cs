using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorManager : Singleton<DoorManager>
{
    private List<GameObject> doorList = new List<GameObject>();
    private GameObject currentDoor = null;
    public override void Awake()
    {
        // Get All locked doors
        doorList.AddRange(GameObject.FindGameObjectsWithTag("LockedDoor"));

        for(int i = 0; i < doorList.Count; i++)
        {
            Debug.LogWarning(doorList[i].name);
        }
    }
    public void RemoveOpenedDoor()
    {
        if (currentDoor != null)
        {
            if (doorList.Contains(currentDoor))
            {
                doorList.Remove(currentDoor);
                currentDoor.SetActive(false);
                currentDoor = null;
            }
            else Debug.LogWarning("Door not contained in list.");
        }
        else Debug.LogError("Door is not available");
    }
    public void SetCurrentDoor(GameObject door)
    {
        currentDoor = door;
        Debug.LogWarning(currentDoor);
    }
    public void ClearCurrentDoor()
    {
        currentDoor = null;
    }
}
