using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

enum ScifiSolderState
{
    Patrolling = 0,
    Tracing,
    Attacking
}

public class ScifiSolderAI : MonoBehaviour
{
    public Transform playerTransform;
    public float speed;
    public float playerDetectRange = 7.0f;
    public float playerAttackRange = 4.0f;
    public GameObject bullet;

    private float walkPointRangeX = 3.0f;
    private float walkPointRangeZ = 1.0f;
    private Vector3 front;
    private ScifiSolderState state;
    private Actions solderAction;
    private NavMeshAgent agent;
    private Vector3 walkPoint;
    private bool setWalkPoint = false;
    private bool isShooting = false;
    private PlayerController controller;
    // Start is called before the first frame update
    void Start()
    {
        solderAction = GetComponent<Actions>();
        agent = GetComponent<NavMeshAgent>();
        controller = GetComponent<PlayerController>();
        state = ScifiSolderState.Patrolling;
        front = new Vector3(0.0f, 0.0f, 1.0f);
        GetComponent<MonsterHealth>().SetHealth(2);
    }

    // Update is called once per frame
    void Update()
    {
        var newFront = (walkPoint - transform.position).normalized;
        if (newFront.magnitude > 0)
            front = newFront;
        float distanceToPlayer = (playerTransform.position - transform.position).magnitude;
        if (distanceToPlayer > playerDetectRange)
            state = ScifiSolderState.Patrolling;
        else if (distanceToPlayer > playerAttackRange)
            state = ScifiSolderState.Tracing;
        else
            state = ScifiSolderState.Attacking;
        switch (state)
        {
            case ScifiSolderState.Patrolling:
                isShooting = false;
                Patrolling();
                solderAction.Walk();
                break;    
            case ScifiSolderState.Tracing:
                isShooting = false;
                walkPoint = playerTransform.position;
                agent.speed = 3.0f;
                solderAction.Run();
                break;    
            case ScifiSolderState.Attacking:
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(walkPoint - transform.position, Vector3.up), 2.0f);
                walkPoint = playerTransform.position;
                agent.speed = 0.0f;
                Shoot();
                break;    
        }
        agent.SetDestination(walkPoint);
        if (GetComponent<MonsterHealth>().IsDeath())
            Destroy(this.gameObject);
    }
    void Patrolling()
    {
        if (!setWalkPoint)
        {
            SetWalkPoint();
            setWalkPoint = true;
        }
        agent.speed = speed;
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
            // walkPoint = transform.position - (hit.position - transform.position) * walkPointRangeX;
            walkPoint = transform.position - right * randomZ - front * randomX * 0.5f;
        else
            walkPoint = hit.position;
    }

    void Shoot()
    {
        solderAction.Aiming();
        if (!isShooting) 
            StartCoroutine(StartShoot());
        isShooting = true;
    }

    IEnumerator StartShoot()
    {
        while (state == ScifiSolderState.Attacking)
        {
            solderAction.Attack();
            ShootBullet();
            yield return new WaitForSeconds(1.5f);
        }
    }

    void ShootBullet()
    {
        GameObject newBullet = Instantiate(bullet);
        newBullet.transform.position = controller.GetRightGunPosition();
        newBullet.GetComponent<Bullet>().Direction = (playerTransform.position - newBullet.transform.position).normalized;
    }

    
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PlayerDamage")
        {
            GetComponent<MonsterHealth>().TakeDamage(1);
            GetComponent<HurtEffect>().position = transform.position + new Vector3(0.0f, 1.0f, 0.0f);
            GetComponent<HurtEffect>().Spawn();
        }
    }

}
