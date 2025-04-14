using UnityEngine;

public class OctopusBossAI : EnemyAI
{
    [Header("Movement Settings")]
    public float floatSpeed = 2f;
    public float swoopSpeed = 5f;
    public float curveAmplitude = 0.5f;
    public float curveFrequency = 2f;

    [Header("Float Area")]
    public Transform floatLeft;
    public Transform floatRight;
    public float floatY = 0f;

    [Header("Swoop Settings")]
    public float detectionRadius = 7f;
    public float swoopCooldown = 5f;
    public float swoopDuration = 10f;

    private Vector2 swoopTarget;
    private float swoopTimer = 0f;
    private float swoopProgress = 0f;
    private bool isSwooping = false;
    private bool goingRight = true;
    private Vector2 startSwoopPos;

    private bool isReturning = false;
    private Vector2 returnStartPos;
    private float returnProgress = 0f;


    private float floatTimer = 0f;
    private float verticalWobbleOffset;

    protected override void Start()
    {
        base.Start();
        floatY = transform.position.y;
        verticalWobbleOffset = Random.Range(0f, 2f * Mathf.PI);
    }

    public override void Act()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer > detectionRadius) return;

        swoopTimer -= Time.deltaTime;

        if (isSwooping)
        {
            HandleSwoop();
        }
        else if (isReturning)
        {
            HandleReturnToFloat();
        }
        else
        {
            HandleFloating();

            if (swoopTimer <= 0f && distanceToPlayer < detectionRadius)
            {
                StartSwoop();
            }
        }
    }



    private void HandleFloating()
    {
        floatTimer += Time.deltaTime;

        // Mouvement horizontal entre les deux bornes
        float direction = goingRight ? 1f : -1f;
        float targetX = transform.position.x + direction * floatSpeed * Time.deltaTime;

        if ((goingRight && targetX > floatRight.position.x) || (!goingRight && targetX < floatLeft.position.x))
        {
            goingRight = !goingRight;
            return;
        }

        // Mouvement vertical sinusoïdal
        float offsetY = Mathf.Sin(floatTimer * curveFrequency + verticalWobbleOffset) * curveAmplitude;

        transform.position = new Vector3(targetX, floatY + offsetY, transform.position.z);

        // Flip visuel
        Vector3 scale = transform.localScale;
        scale.x = goingRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    private void StartSwoop()
    {
        isSwooping = true;
        swoopTimer = swoopCooldown;
        startSwoopPos = transform.position;
        swoopTarget = player.position;
        swoopProgress = 0f;
    }

    private void HandleSwoop()
    {
        swoopProgress += Time.deltaTime / swoopDuration;
        if (swoopProgress >= 1f)
        {
            isSwooping = false;
            StartReturnToFloat();
            return;
        }

        Vector2 lerped = Vector2.Lerp(startSwoopPos, swoopTarget, swoopProgress);
        float arc = Mathf.Sin(Mathf.PI * swoopProgress) * curveAmplitude;
        Vector2 final = new Vector2(lerped.x, lerped.y + arc);
        transform.position = final;
    }

    private void StartReturnToFloat()
    {
        isReturning = true;
        returnStartPos = transform.position;
        returnProgress = 0f;
    }

    private void HandleReturnToFloat()
    {
        returnProgress += Time.deltaTime / 1f; 

        if (returnProgress >= 1f)
        {
            isReturning = false;
            return;
        }

        float targetX = goingRight ? floatRight.position.x : floatLeft.position.x;
        Vector2 floatPos = new Vector2(targetX, floatY);
        Vector2 lerped = Vector2.Lerp(returnStartPos, floatPos, returnProgress);
        float arc = Mathf.Sin(Mathf.PI * returnProgress) * curveAmplitude;
        Vector2 final = new Vector2(lerped.x, lerped.y + arc);
        transform.position = final;
    }

    private void OnDrawGizmosSelected()
    {
        if (floatLeft && floatRight)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(new Vector2(floatLeft.position.x, floatY), new Vector2(floatRight.position.x, floatY));
        }

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}