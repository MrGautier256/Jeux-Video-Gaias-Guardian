using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(BoxCollider2D))]
public class RoomTransitionTrigger : MonoBehaviour
{
    [Header("Rooms à cibler")]
    public Transform previousRoom;
    public Transform nextRoom;

    private float cooldown = 0.2f;
    private float lastTransitionTime = -999f;

    private CameraRoomSizer cameraSizer;

    private void Awake()
    {
        cameraSizer = Camera.main.GetComponent<CameraRoomSizer>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (Time.time - lastTransitionTime < cooldown) return;

        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
        if (rb == null) return;

        float velocityX = rb.linearVelocity.x;
        if (Mathf.Abs(velocityX) < 0.1f) return;

        Transform targetRoom = velocityX > 0 ? nextRoom : previousRoom;

        if (targetRoom != null && cameraSizer != null)
        {
            cameraSizer.SetRoom(targetRoom);
            lastTransitionTime = Time.time;
        }
    }
}
