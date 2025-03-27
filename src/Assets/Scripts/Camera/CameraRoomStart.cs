using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraRoomStart : MonoBehaviour
{
    public Transform startRoom;

    void Start()
    {
        if (startRoom != null)
        {
            Vector3 center = GetRoomCenterFromTilemap(startRoom);
            transform.position = new Vector3(center.x, center.y, transform.position.z);
        }
    }

    Vector3 GetRoomCenterFromTilemap(Transform room)
    {
        Tilemap tilemap = room.GetComponentInChildren<Tilemap>();
        if (tilemap != null)
        {
            Bounds bounds = tilemap.localBounds;
            return bounds.center + tilemap.transform.position;
        }

        return room.position;
    }
}
