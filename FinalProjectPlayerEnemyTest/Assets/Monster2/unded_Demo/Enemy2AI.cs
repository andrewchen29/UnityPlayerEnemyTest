using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy2AI : MonoBehaviour
{
    public float rotateSpeed = 1.0f;
    public float walkSpeed = 1.0f;
    public float runSpeed = 1.0f;
    public float playerDetectRange = 8.0f;
    public float playerAttackRange = 1.0f;
    public float attackPeriod = 0.5f;
    public Transform playerTransform;
    public int MaxHealth = 2;
    public GameObject HealthBar;

    private bool setWalkPoint = false;
    private Vector3 walkPoint;
    private float walkPointRangeX;
    private float walkPointRangeZ;
    private Vector3 front;

    private Animator animator;
    private NavMeshAgent agent;


    private EnemyState state;
    private Enemy2Animation animate;
    private Collider hitCollider;
    private bool attacking = false;
    // Start is called before the first frame update
    void Start()
    {
        front = new Vector3(0.0f, 0.0f, 1.0f);
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        animate = GetComponent<Enemy2Animation>();
        hitCollider = GetComponentInChildren<CapsuleCollider>();

        walkPointRangeX = 3.0f;
        walkPointRangeZ = 1.0f;
        SetWalkPoint();
        state = EnemyState.Patrolling;
        GetComponent<MonsterHealth>().SetHealth(MaxHealth);
        var hb = Instantiate(HealthBar);
        hb.transform.SetParent(this.transform);
        GetComponentInChildren<MonsterHealthbar>().playerTransform = playerTransform;
        GetComponentInChildren<MonsterHealthbar>().monsterTransform = this.transform;
        GetComponentInChildren<MonsterHealthbar>().SetMaxHealth(MaxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        var newFront = (walkPoint - transform.position).normalized;
        if (newFront.magnitude > 0)
            front = newFront;
        // transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(front, Vector3.up), rotateSpeed);

        float distanceToPlayer = (playerTransform.position - transform.position).magnitude;
        if (distanceToPlayer > playerDetectRange)
            state = EnemyState.Patrolling;
        else if (distanceToPlayer > playerAttackRange)
            state = EnemyState.Tracing;
        else
            state = EnemyState.Attacking;

        switch (state)
        {
            case EnemyState.Patrolling:
                hitCollider.enabled = false;
                attacking =false;
                Patrolling();
                animate.Walk();
                break;
            case EnemyState.Tracing:
                hitCollider.enabled = false;
                attacking =false;
                animate.Walk();
                agent.speed = runSpeed;
                walkPoint = playerTransform.position;
                setWalkPoint = false;
                break;
            case EnemyState.Attacking:
                hitCollider.enabled = true;
                animate.Idle();
                walkPoint = playerTransform.position;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(walkPoint - transform.position, Vector3.up), 2.0f);
                Attack();
                break;
        }
        
        agent.SetDestination(walkPoint);
        if (GetComponent<MonsterHealth>().IsDeath())
            Destroy(this.gameObject, 0.5f);
        var h = GetComponent<MonsterHealth>().GetHealth();
        GetComponentInChildren<MonsterHealthbar>().SetHealth(h);
    }

    void Patrolling()
    {
        if (!setWalkPoint)
        {
            SetWalkPoint();
            setWalkPoint = true;
        }
        agent.speed = walkSpeed;
        float distanceToWalkPoint = (transform.position - walkPoint).magnitude;
        if (distanceToWalkPoint < 1.0f)
            SetWalkPoint();
    }

    void SetWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRangeZ, walkPointRangeZ);
        float randomX = Random.Range(0.0f, walkPointRangeX);
        
        Vector3 right = Vector3.Cross(front, Vector3.up);
        walkPoint = transform.position + right * randomZ + front * randomX;

        NavMeshHit hit;
        NavMesh.SamplePosition(walkPoint, out hit, 2.0f, 1);
        if ((!hit.hit) || (hit.position - transform.position).magnitude < 0.3f)
            walkPoint = transform.position - right * randomZ - front * randomX;
        else
            walkPoint = hit.position;
    }

    void Attack()
    {
        agent.speed = 0.0f;
        if (!attacking) StartCoroutine(StartShoot());
        attacking = true;
    }

    IEnumerator StartShoot()
    {
        while (state == EnemyState.Attacking)
        {
            yield return new WaitForSeconds(attackPeriod);
            animate.Attack();
        }
    }

    
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PlayerDamage")
        {
            GetComponent<MonsterHealth>().TakeDamage(1);
            GetComponent<HurtEffect>().position = transform.position + new Vector3(0.0f, 3.0f, 0.0f);
            GetComponent<HurtEffect>().Spawn();
            GetComponentInChildren<AudioSource>().Play();
        }
    }

}
