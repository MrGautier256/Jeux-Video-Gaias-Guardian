using UnityEngine;

public class BossRoomTrigger : MonoBehaviour
{
    public BossRoomManager bossRoomManager;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            bossRoomManager?.ActivateBossRoom();
            gameObject.SetActive(false); 
        }
    }
}
