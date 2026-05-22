using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class GuardAI : MonoBehaviour
{
    public enum GuardState
    {
        Patrol,
        Chase,
        Investigate,
        Return
    }

    public GameObject alertIcon;

    private float lastSeenTime;
    public float memoryTime = 2f;

    [Header("State")]
    public GuardState currentState = GuardState.Patrol;

    [Header("References")]
    public Transform[] patrolPoints;
    public Transform player;

    [Header("Detection")]
    public float detectionRange = 8f;
    public float losePlayerRange = 12f;
    public float catchRange = 1.5f;
    public LayerMask lineOfSightMask = ~0;

    [Header("Speeds")]
    public float patrolSpeed = 2.5f;
    public float chaseSpeed = 4.5f;

    [Header("Scene")]
    public bool restartWhenCaught = true;
    public float restartDelay = 1f;

    private NavMeshAgent agent;
    private Renderer rend;
    private int currentPoint = 0;
    private Vector3 lastKnownPosition;
    private Vector3 returnPosition;
    private bool playerCaught = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        rend = GetComponent<Renderer>();

        currentState = GuardState.Patrol;
        agent.speed = patrolSpeed;

        GoToNextPoint();
    }

    void Update()
    {
        if (player == null || playerCaught) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        switch (currentState)
        {
            case GuardState.Patrol:
                Patrol();

                if (CanSeePlayer())
                {
                    currentState = GuardState.Chase;
                }
                break;

            case GuardState.Chase:
            Chase();

            if (CanSeePlayer())
            {
                lastSeenTime = Time.time;
            }

            if (distanceToPlayer <= catchRange)
            {
                CatchPlayer();
            }
            else if (Time.time - lastSeenTime > memoryTime)
            {
                lastKnownPosition = player.position;
                returnPosition = lastKnownPosition;
                currentState = GuardState.Investigate;
            }
            break;

            case GuardState.Investigate:
                Investigate();

                if (CanSeePlayer())
                {
                    currentState = GuardState.Chase;
                }
                break;

            case GuardState.Return:
                ReturnToPatrol();

                if (CanSeePlayer())
                {
                    currentState = GuardState.Chase;
                }
                break;
        }

        UpdateColor();
    }

    void Patrol()
    {
        agent.speed = patrolSpeed;

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            GoToNextPoint();
        }
    }

    void Chase()
    {
        agent.speed = chaseSpeed;
        agent.SetDestination(player.position);
    }

    void Investigate()
    {
        agent.speed = patrolSpeed;
        agent.SetDestination(lastKnownPosition);

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            currentState = GuardState.Return;
        }
    }

    void ReturnToPatrol()
    {
        agent.speed = patrolSpeed;

        if (patrolPoints.Length == 0) return;

        agent.SetDestination(patrolPoints[currentPoint].position);

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            currentState = GuardState.Patrol;
            GoToNextPoint();
        }
    }

    void GoToNextPoint()
    {
        if (patrolPoints.Length == 0) return;

        agent.SetDestination(patrolPoints[currentPoint].position);
        currentPoint = (currentPoint + 1) % patrolPoints.Length;
    }

    bool CanSeePlayer()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > detectionRange)
            return false;

        PlayerHide playerHide = player.GetComponent<PlayerHide>();

        if (playerHide != null && playerHide.isHidden)
            return false;

        Vector3 rayStart = transform.position + Vector3.up * 1.5f;
        Vector3 rayEnd = player.position + Vector3.up * 1f;
        Vector3 direction = (rayEnd - rayStart).normalized;

        RaycastHit hit;

        if (Physics.Raycast(rayStart, direction, out hit, detectionRange, lineOfSightMask))
        {
            if (hit.transform.CompareTag("Player"))
            {
                return true;
            }
        }

        return false;
    }

    void CatchPlayer()
    {
        playerCaught = true;
        Debug.Log("Player caught!");

        if (restartWhenCaught)
        {
            Invoke(nameof(RestartScene), restartDelay);
        }
    }

    void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void UpdateColor()
    {
        if (rend == null) return;

        switch (currentState)
        {
            case GuardState.Patrol:
                rend.material.color = Color.green;
                break;

            case GuardState.Chase:
                rend.material.color = Color.red;
                break;

            case GuardState.Investigate:
                rend.material.color = Color.yellow;
                break;

            case GuardState.Return:
                rend.material.color = Color.blue;
                break;
        }

        if (currentState == GuardState.Chase)
        {
            alertIcon.SetActive(true);
        }
        else
        {
            alertIcon.SetActive(false);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Vector3 forward = transform.forward * detectionRange;

        Gizmos.DrawLine(transform.position, transform.position + forward);
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
