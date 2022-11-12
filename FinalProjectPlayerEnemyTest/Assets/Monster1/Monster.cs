using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
enum EnemyState
{
    Patrolling = 0,
    Tracing,
    Attacking
}

public class Monster : MonoBehaviour
{
    public float rotateSpeed = 1.0f;
    public float walkSpeed = 1.0f;
    public float runSpeed = 1.0f;
    public Transform playerTransform;
    public float playerDetectRange = 8.0f;
    public float playerAttackRange = 1.0f;

    private float walkPointRangeX;
    private float walkPointRangeZ;
    private Vector3 front;
    private bool setWalkPoint = false;
    private Vector3 walkPoint;

    private Animator animator;
    private NavMeshAgent navMeshAgent;

    private int walkForwardHash;
    private int runForwardHash;
    private int jumpHash;
    private int attack01Hash;
    private int attack02Hash;
    private EnemyState state;
    private Collider biteCollider;
    // Start is called before the first frame update
    void Start()
    {
        front = new Vector3(1.0f, 0.0f, 0.0f);
        animator = GetComponent<Animator>();
        biteCollider = GetComponentInChildren<SphereCollider>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        walkForwardHash = Animator.StringToHash("Walk Forward");
        runForwardHash = Animator.StringToHash("Run Forward");
        jumpHash = Animator.StringToHash("Jump");
        attack01Hash = Animator.StringToHash("Attack 01");
        attack02Hash = Animator.StringToHash("Attack 02");

        walkPointRangeX = 3.0f;
        walkPointRangeZ = 1.0f;
        SetWalkPoint();
        state = EnemyState.Patrolling;
        GetComponent<MonsterHealth>().SetHealth(1);
    }

    // Update is called once per frame
    void Update()
    {
        var newFront = (walkPoint - transform.position).normalized;
        if (newFront.magnitude > 0)
            front = newFront;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(front, Vector3.up), rotateSpeed);

        float distanceToPlayer = (playerTransform.position - transform.position).magnitude;
        if (distanceToPlayer > playerDetectRange)
            state = EnemyState.Patrolling;
        else if (distanceToPlayer > playerAttackRange)
            state = EnemyState.Tracing;
        else
            state = EnemyState.Attacking;
        Animate();
        Move();
        navMeshAgent.SetDestination(walkPoint);
        if (GetComponent<MonsterHealth>().IsDeath())
            Destroy(this.gameObject, 0.5f);
    }

    void Animate()
    {
        animator.SetBool(walkForwardHash, false);
        animator.SetBool(runForwardHash, false);
        switch (state)
        {
            case EnemyState.Patrolling:
                biteCollider.enabled = false;
                animator.SetBool(walkForwardHash, true);
                break;
            case EnemyState.Tracing:
                animator.SetBool(runForwardHash, true);
                break;
            case EnemyState.Attacking:
                biteCollider.enabled = true;
                animator.SetTrigger(attack02Hash);
                break; 
        }
    }
    void Move()
    {
        switch (state)
        {
            case EnemyState.Patrolling:
                Patrolling();
                break;
            case EnemyState.Tracing:
                TracingPlayer();
                break;
            case EnemyState.Attacking:
                walkPoint = playerTransform.position;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(walkPoint - transform.position, Vector3.up), 2.0f);
                Attacking();
                break;
        }
    }
    void TracingPlayer()
    {
        walkPoint = playerTransform.position;
        navMeshAgent.speed = runSpeed;
        setWalkPoint = false;
    }
    void Patrolling()
    {
        if (!setWalkPoint)
        {
            SetWalkPoint();
            setWalkPoint = true;
        }
        navMeshAgent.speed = walkSpeed;
        float distanceToWalkPoint = (transform.position - walkPoint).magnitude;
        if (distanceToWalkPoint < 1.0f)
            SetWalkPoint();
    }

    void SetWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRangeZ, walkPointRangeZ);
        float randomX = Random.Range(0.0f, walkPointRangeX);

        // walkPoint = new Vector3(transform.position.x + randomX, 
        //                         transform.position.y,
        //                         transform.position.z + randomZ);
        
        Vector3 right = Vector3.Cross(front, Vector3.up);
        walkPoint = transform.position + right * randomZ + front * randomX;

        NavMeshHit hit;
        NavMesh.SamplePosition(walkPoint, out hit, 2.0f, 1);
        if ((!hit.hit) || (hit.position - transform.position).magnitude < 0.3f)
            // walkPoint = transform.position - (hit.position - transform.position) * walkPointRangeX;
            walkPoint = transform.position - right * randomZ - front * randomX;
        else
            walkPoint = hit.position;
        
        // if (Physics.Raycast(walkPoint, -transform.up, 2.0f, Ground))
    }

    void Attacking()
    {
        setWalkPoint = false;
        walkPoint = transform.position;
    }

    [System.Serializable]
    public struct EnemyAISetting
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PlayerDamage")
        {
            GetComponent<MonsterHealth>().TakeDamage(1);
            GetComponent<HurtEffect>().position = transform.position + new Vector3(0.0f, 1.0f, 0.0f);
            GetComponent<HurtEffect>().Spawn();
            GetComponentInChildren<AudioSource>().Play();
        }
    }

}
