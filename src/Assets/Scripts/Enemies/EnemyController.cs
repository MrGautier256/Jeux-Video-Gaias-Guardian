using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private EnemyAI ai;

    void Start()
    {
        ai = GetComponent<EnemyAI>();
    }

    void Update()
    {
        if (ai != null)
            ai.Act();
    }
}
