using UnityEngine;
using UnityEngine.AI;

public class GuardAI : MonoBehaviour
{
    public enum GuardState
    {
        Patrol,
        Chase,
        Return
    }

    public GuardState currentState;

    public Transform[] patrolPoints;
    public Transform player;

    public float detectionRange = 8f;
    public float losePlayerRange = 12f;

    private NavMeshAgent agent;
    private int currentPoint;

    private Vector3 lastKnownPosition;

    private Renderer rend;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        rend = GetComponent<Renderer>();

        currentState = GuardState.Patrol;

        GoToNextPoint();
    }

    void Update()
    {
        float distanceToPlayer =
            Vector3.Distance(transform.position, player.position);

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

                if (distanceToPlayer > losePlayerRange)
                {
                    lastKnownPosition = player.position;
                    currentState = GuardState.Return;
                }

                break;

            case GuardState.Return:
                Return();

                break;
        }

        UpdateColor();
    }

    void Patrol()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            GoToNextPoint();
        }
    }

    void Chase()
    {
        agent.SetDestination(player.position);
    }

    void Return()
    {
        agent.SetDestination(lastKnownPosition);

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            currentState = GuardState.Patrol;
            GoToNextPoint();
        }
    }

    void GoToNextPoint()
    {
        if (patrolPoints.Length == 0)
            return;

        agent.destination = patrolPoints[currentPoint].position;

        currentPoint = (currentPoint + 1) % patrolPoints.Length;
    }

    bool CanSeePlayer()
    {
        float distance =
            Vector3.Distance(transform.position, player.position);

        if (distance > detectionRange)
            return false;

        RaycastHit hit;

        Vector3 direction =
            (player.position - transform.position).normalized;

        if (Physics.Raycast(transform.position, direction, out hit, detectionRange))
        {
            if (hit.transform.CompareTag("Player"))
            {
                return true;
            }
        }

        return false;
    }

    void UpdateColor()
    {
        switch (currentState)
        {
            case GuardState.Patrol:
                rend.material.color = Color.green;
                break;

            case GuardState.Chase:
                rend.material.color = Color.red;
                break;

            case GuardState.Return:
                rend.material.color = Color.yellow;
                break;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
