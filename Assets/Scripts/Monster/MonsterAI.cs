using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class MonsterAI : MonoBehaviour
{
    // Monster and its target
    private Rigidbody2D rb;
    public bool facingRight;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private Transform playerTarget;

    // Pathfinding
    [SerializeField]
    private float nextWaypointDistance;
    private int currentWayPoint;

    private Path path;
    private Seeker seeker;
    private bool reachedEndOfPath;

    [SerializeField]
    private float monsterSpeed;

    private Vector3 targetPosition;

    // Player detection
    [SerializeField]
    private LayerMask whatIsGround, whatIsPlayer;

    // Patroling
    [SerializeField]
    private float walkPointRange; // Distance max to the next patrolling point
    [SerializeField]
    private Transform patrollingDirection;

    // Attacking
    public bool alreadyAttacked;
    [SerializeField]
    private Transform hitPoint;

    // States
    [SerializeField]
    private float sightRange, attackRange;
    private bool playerInSightRange, playerInAttackRange;

    void Awake()
    {
        this.rb = GetComponent<Rigidbody2D>();
        this.facingRight = true;

        this.seeker = GetComponent<Seeker>();
        this.targetPosition = Vector3.zero;

        this.currentWayPoint = 0;
        this.reachedEndOfPath = true;
        this.alreadyAttacked = false;
    }

    void Start()
    {
        // Make the monster face the left side
        MonsterFlip();
        // Update the path each .5 second
        InvokeRepeating("ChasingPath", 0f, .5f);
        InvokeRepeating("PatrollingPath", 0f, 3f);
    }

    void ChasingPath()
    {
        if (this.playerInSightRange)
        {
            // If the path is completed, start again
            if (this.seeker.IsDone())
            {
                targetPosition = playerTarget.position;
                targetPosition.y = this.transform.position.y;
                seeker.StartPath(rb.position, targetPosition, OnPathComplete);
            }
        }
    }

    void PatrollingPath()
    {
        // If the path is completed, start again
        if (!this.playerInSightRange)
        {
            if (this.seeker.IsDone())
            {
                if (this.reachedEndOfPath)
                {
                    targetPosition = this.patrollingDirection.position + (Vector3)Random.insideUnitCircle * walkPointRange;
                    targetPosition.y = this.transform.position.y;
                    seeker.StartPath(rb.position, targetPosition, OnPathComplete);
                }
            }
        }
    }

    void OnPathComplete(Path path)
    {
        if (!path.error)
        {
            this.path = path;
            currentWayPoint = 0;
        }
    }

    void FixedUpdate()
    {
        // If karna is in sight
        this.playerInSightRange = Physics2D.OverlapCircle(this.transform.position, sightRange, whatIsPlayer) != null;
        this.playerInAttackRange = Physics2D.OverlapCircle(this.transform.position, attackRange, whatIsPlayer) != null;

        if (!this.playerInSightRange && !this.playerInAttackRange) MonsterMove();
        if (this.playerInSightRange && !this.playerInAttackRange) MonsterMove();
        if (this.playerInAttackRange && this.playerInSightRange && !this.alreadyAttacked) AttackPlayer();
    }

    /// <summary>
    /// Attacking the player
    /// </summary>
    private void AttackPlayer()
    {
        animator.SetBool("MonsterWalking", false);
        animator.SetTrigger("MonsterAttacking");
        this.alreadyAttacked = true;

        Collider2D karnaHit = Physics2D.OverlapCircle(hitPoint.position, attackRange, whatIsPlayer);
        if (karnaHit != null)
        {
            karnaHit.GetComponent<CharacterController2D>().TakeDamage();
        }
    }

    public void AttackMode()
    {
        this.sightRange = 1000f;
    }

    /// <summary>
    /// Move the monster to the end of the generated path
    /// </summary>
    private void MonsterMove()
    {
        if (this.path == null)
            return;

        if (this.currentWayPoint >= this.path.vectorPath.Count)
        {
            this.reachedEndOfPath = true;
            animator.SetBool("MonsterWalking", false);
            return;
        }
        else
        {
            this.reachedEndOfPath = false;
        }

        Vector2 direction = ((Vector2)this.path.vectorPath[currentWayPoint] - this.rb.position).normalized;
        //direction.y = this.rb.position.y;
        transform.Translate(direction * monsterSpeed * Time.fixedDeltaTime);
        animator.SetBool("MonsterWalking", true);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWayPoint]);

        if (distance < nextWaypointDistance)
            currentWayPoint++;

        if ((facingRight && direction.x < 0) || (!facingRight && direction.x > 0))
            MonsterFlip();
    }

    private void MonsterFlip()
    {
        this.facingRight = !facingRight;
        transform.localScale = new Vector3(transform.localScale.x * -1f, transform.localScale.y * 1f, transform.localScale.z * 1f);
    }
}
