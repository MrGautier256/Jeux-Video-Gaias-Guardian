using UnityEngine;

public class CameraRoomSizer : MonoBehaviour
{
    private Transform currentRoom;

    public void SetRoom(Transform room)
    {
        currentRoom = room;
        if (room != null)
        {
            Vector3 newPosition = new Vector3(
                room.position.x,
                room.position.y,
                transform.position.z // garde la profondeur caméra
            );

            transform.position = newPosition;
        }
    }

    private void Start()
    {
        if (currentRoom != null)
        {
            SetRoom(currentRoom);
        }
    }
}
