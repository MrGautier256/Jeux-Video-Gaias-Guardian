using UnityEngine;

public class CameraRoomFollower : MonoBehaviour
{
    private CameraRoomSizer cameraSizer;

    private void Start()
    {
        cameraSizer = Camera.main.GetComponent<CameraRoomSizer>();
        RoomTransitionTrigger trigger = GetComponent<RoomTransitionTrigger>();
        if (trigger != null)
        {
            trigger.OnRoomChanged += HandleRoomChange;
        }
    }

    void HandleRoomChange(Transform newRoom)
    {
        if (cameraSizer != null && newRoom != null)
        {
            cameraSizer.SetRoom(newRoom);
        }
    }
}
