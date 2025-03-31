using UnityEngine;

public class RespawnSetter : MonoBehaviour
{
    private void Start()
    {
        RoomTransitionTrigger trigger = GetComponent<RoomTransitionTrigger>();
        if (trigger != null)
        {
            trigger.OnRespawnChanged += HandleRespawnChange;
        }
    }

    void HandleRespawnChange(Transform newSpawn)
    {
        if (newSpawn != null && RespawnManager.Instance != null)
        {
            RespawnManager.Instance.SetRespawnPoint(newSpawn);
        }
    }
}
