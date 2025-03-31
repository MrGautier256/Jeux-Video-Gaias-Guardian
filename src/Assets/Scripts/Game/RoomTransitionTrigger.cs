using UnityEngine;
using System;

[RequireComponent(typeof(BoxCollider2D))]
public class RoomTransitionTrigger : MonoBehaviour
{
    [Header("Room References")]
    public Transform previousRoom;
    public Transform nextRoom;

    [Header("Respawn Points")]
    public Transform previousRespawn;
    public Transform nextRespawn;

    [Header("Cooldown")]
    public float cooldown = 0.2f;
    private float lastTriggerTime = -999f;

    public event Action<Transform> OnRoomChanged;
    public event Action<Transform> OnRespawnChanged;

    private CameraRoomSizer cameraSizer;

    private void Start()
    {
        cameraSizer = Camera.main?.GetComponent<CameraRoomSizer>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (Time.time - lastTriggerTime < cooldown) return;

        Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
        if (rb == null) return;

        float dir = rb.linearVelocity.x;
        if (Mathf.Abs(dir) < 0.1f) return;

        bool goingRight = dir > 0;

        Transform targetRoom = goingRight ? nextRoom : previousRoom;
        Transform targetRespawn = goingRight ? nextRespawn : previousRespawn;

        UpdateCameraRoom(targetRoom);
        UpdateRespawnPoint(targetRespawn);

        lastTriggerTime = Time.time;
    }

    private void UpdateCameraRoom(Transform room)
    {
        OnRoomChanged?.Invoke(room);

        if (cameraSizer != null && room != null)
        {
            cameraSizer.SetRoom(room);
        }
    }

    private void UpdateRespawnPoint(Transform respawn)
    {
        OnRespawnChanged?.Invoke(respawn);

        if (respawn != null && RespawnManager.Instance != null)
        {
            RespawnManager.Instance.SetRespawnPoint(respawn);
        }
    }
}
