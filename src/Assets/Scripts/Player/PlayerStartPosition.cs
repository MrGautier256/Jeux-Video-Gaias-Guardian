using UnityEngine;

public class PlayerStartPosition : MonoBehaviour
{
    void Start()
    {
        if (RespawnManager.Instance == null)
        {
            Debug.LogError("[PlayerStartPosition] RespawnManager.Instance est null au demarrage !");
            return;
        }

        Transform spawn = RespawnManager.Instance.GetRespawnPoint();

        if (spawn != null)
        {
            transform.position = spawn.position;
        }
        else
        {
            Debug.LogWarning("[PlayerStartPosition] Aucun point de spawn trouve. Position non modifiee.");
        }
    }
}
