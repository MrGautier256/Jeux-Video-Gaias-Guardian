using UnityEngine;

public class CameraRoomSizer : MonoBehaviour
{
    [Header("Taille fixe de la caméra")]
    public float targetSize = 5.715f;

    private Transform currentRoom;

    public void SetRoom(Transform room)
    {
        currentRoom = room;
        if (room != null)
        {
            Vector3 newPosition = new Vector3(
                room.position.x,
                room.position.y,
                transform.position.z
            );

            transform.position = newPosition;
        }
    }

    private void Start()
    {
        // Forcer la taille de la caméra orthographique
        Camera cam = GetComponent<Camera>();
        if (cam != null)
        {
            cam.orthographicSize = targetSize;
        }

        if (currentRoom != null)
        {
            SetRoom(currentRoom);
        }
    }
}
