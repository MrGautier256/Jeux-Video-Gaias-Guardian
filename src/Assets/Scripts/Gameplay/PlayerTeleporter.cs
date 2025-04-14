using UnityEngine;
using System;

[RequireComponent(typeof(BoxCollider2D))]
public class PlayerTeleporter : MonoBehaviour, ITriggerDesactivable
{
    [Header("Changement de Room et de Spawn")]
    public Transform nextRoom;            // Pour la caméra
    public Transform newRespawnPoint;     // Point de respawn à sauvegarder
    public Transform teleportTarget;      // Point exact où téléporter le joueur

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

    public void SetEnabled(bool value) => enabled = value;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (Time.time - lastTriggerTime < cooldown) return;

        if (teleportTarget != null)
        {
            // Déplacement immédiat du joueur
            other.transform.position = teleportTarget.position;

            // Réinitialisation de la vitesse
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }
        }

        UpdateCameraRoom(nextRoom);
        UpdateRespawnPoint(newRespawnPoint);

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
