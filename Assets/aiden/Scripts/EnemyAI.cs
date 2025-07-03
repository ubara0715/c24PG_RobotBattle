using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    public GameObject projectilePrefab;
    public Transform shootPoint;
    public float shootForce = 15f;
    public float shootInterval = 2f;

    private float shootTimer;
    private NavMeshAgent agent;

    void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
            else
                Debug.LogError("Player not found in scene! Please tag the player object with 'Player'.");
        }

        agent = GetComponent<NavMeshAgent>();
        shootTimer = shootInterval;
    }

    void Update()
    {
        if (player == null || agent == null) return;

        if (agent.isOnNavMesh)
        {
            agent.SetDestination(player.position);
        }

        shootTimer -= Time.deltaTime;
        if (shootTimer <= 0f && HasLineOfSight())
        {
            ShootProjectile();
            shootTimer = shootInterval;
        }
    }

    bool HasLineOfSight()
    {
        Vector3 dir = (player.position - shootPoint.position).normalized;
        if (Physics.Raycast(shootPoint.position, dir, out RaycastHit hit))
        {
            return hit.transform == player;
        }
        return false;
    }

    void ShootProjectile()
    {
        Vector3 direction = (player.position - shootPoint.position).normalized;
        Vector3 spawnPosition = shootPoint.position + direction * 1f;

        GameObject projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);
        projectile.transform.forward = direction;

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = direction * shootForce;
        }
    }
}
