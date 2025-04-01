using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    public static RespawnManager Instance;

    [Header("Point de spawn initial")]
    public Transform fallbackSpawn;

    private Transform currentSpawn;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SetRespawnPoint(Transform point)
    {
        currentSpawn = point;
    }

    public Transform GetRespawnPoint()
    {
        return currentSpawn != null ? currentSpawn : fallbackSpawn;
    }
}
