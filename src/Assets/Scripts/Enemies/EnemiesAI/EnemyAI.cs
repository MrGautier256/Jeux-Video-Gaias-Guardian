using UnityEngine;

public abstract class EnemyAI : MonoBehaviour
{
    protected Transform player;

    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public abstract void Act();
}


