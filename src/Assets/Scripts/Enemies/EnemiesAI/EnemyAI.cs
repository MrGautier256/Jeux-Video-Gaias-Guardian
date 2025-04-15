using UnityEngine;

public abstract class EnemyAI : MonoBehaviour
{
    protected Transform player;
    public bool IsDead { get; private set; } = false;

    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public abstract void Act();

    public void MarkAsDead()
    {
        IsDead = true;
    }
}


