using UnityEngine;

public class PatrollingAI : EnemyAI
{
    [Header("Patrol Settings")]
    public float speed = 2f;
    public float stopDistance = 0.05f;
    public float waitTimeAtPoint = 0.5f;
    public float initialDelayRandomRange = 1.5f; 
    public Transform pointA;
    public Transform pointB;
    private Transform currentTarget;
    private float waitTimer = 0f;

    protected override void Start()
    {
        base.Start();
        currentTarget = pointA;

        waitTimer = -Random.Range(0f, initialDelayRandomRange);
    }

    public override void Act()
    {
        if (pointA == null || pointB == null) return;

        Vector2 currentPos = new Vector2(transform.position.x, transform.position.y);
        Vector2 targetPos = new Vector2(currentTarget.position.x, currentTarget.position.y);
        float distance = Vector2.Distance(currentPos, targetPos);

        if (distance < stopDistance)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTimeAtPoint)
            {
                currentTarget = (currentTarget == pointA) ? pointB : pointA;
                waitTimer = 0f;
            }
            return;
        }

        transform.position = Vector2.MoveTowards(currentPos, targetPos, speed * Time.deltaTime);

        Vector3 dir = currentTarget.position - transform.position;
        if (dir.x != 0)
            transform.localScale = new Vector3(Mathf.Sign(dir.x), 1f, 1f);
    }
}
