using UnityEngine;
using System;

[RequireComponent(typeof(BoxCollider2D))]
public class TwinRoomTransitionTrigger : MonoBehaviour, ITriggerDesactivable
{
    [Header("Cible de la transition")]
    public Transform targetRoom;
    public Transform targetRespawn;

    [Header("Trigger jumeau a activer apres passage")]
    public MonoBehaviour alternateTriggerRaw; 
    private ITriggerDesactivable alternateTrigger;

    [Header("Cooldown")]
    public float cooldown = 0.2f;
    private float lastTriggerTime = -999f;

    public event Action<Transform> OnRoomChanged;
    public event Action<Transform> OnRespawnChanged;

    private CameraRoomSizer cameraSizer;

    private void Start()
    {
        cameraSizer = Camera.main?.GetComponent<CameraRoomSizer>();
        alternateTrigger = alternateTriggerRaw as ITriggerDesactivable;

        if (alternateTriggerRaw != null && alternateTrigger == null)
        {
            Debug.LogWarning($"{alternateTriggerRaw.name} ne contient pas ITriggerDesactivable !");
        }
    }

    public void SetEnabled(bool value) => enabled = value;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (Time.time - lastTriggerTime < cooldown) return;

        UpdateCameraRoom(targetRoom);
        UpdateRespawnPoint(targetRespawn);

        alternateTrigger?.SetEnabled(true);
        SetEnabled(false);

        lastTriggerTime = Time.time;
    }

    private void UpdateCameraRoom(Transform room)
    {
        OnRoomChanged?.Invoke(room);
        if (cameraSizer != null && room != null)
            cameraSizer.SetRoom(room);
    }

    private void UpdateRespawnPoint(Transform respawn)
    {
        if (respawn == null) return;

        OnRespawnChanged?.Invoke(respawn);
        RespawnManager.Instance?.SetRespawnPoint(respawn);
    }
}
