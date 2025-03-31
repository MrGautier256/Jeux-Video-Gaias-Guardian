using UnityEngine;
using System;

[RequireComponent(typeof(BoxCollider2D))]
public class RoomTransitionTrigger : MonoBehaviour
{
    public Transform previousRoom;
    public Transform nextRoom;

    public Transform previousRespawn;
    public Transform nextRespawn;

    public float cooldown = 0.2f;
    private float lastTriggerTime = -999f;

    public event Action<Transform> OnRoomChanged;
    public event Action<Transform> OnRespawnChanged;

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

        OnRoomChanged?.Invoke(targetRoom);
        OnRespawnChanged?.Invoke(targetRespawn);

        lastTriggerTime = Time.time;
    }
}
