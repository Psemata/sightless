using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingLadder : MonoBehaviour
{
    private float vertical;
    private float horizontal;
    private float speedV = 4f;
    private float speedH = 2f;
    private bool isLadder;
    public bool isClimbing;
    private float gravityScale;

    private Rigidbody2D rb;
    // Animator element
    private Animator m_Animator;

    void Awake()
    {
        this.rb = GetComponent<Rigidbody2D>();
        this.m_Animator = GetComponent<Animator>();
        this.gravityScale = rb.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        this.vertical = Input.GetAxisRaw("Vertical");
        this.horizontal = Input.GetAxisRaw("Horizontal");

        if (this.isLadder && Mathf.Abs(this.vertical) > 0f)
        {
            this.isClimbing = true;
            m_Animator.SetBool("KarnaClimbing", true);
        }
    }

    void FixedUpdate()
    {
        if (this.isClimbing)
        {
            this.rb.gravityScale = 0f;
            this.rb.velocity = new Vector2(this.horizontal * this.speedH, this.vertical * this.speedV);
            m_Animator.SetFloat("KarnaVSpeed", Mathf.Abs(this.vertical));
        }
        else
        {
            this.rb.gravityScale = this.gravityScale;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("ladder"))
        {
            this.isLadder = true;
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        this.isLadder = false;
        this.isClimbing = false;
        m_Animator.SetBool("KarnaClimbing", false);
    }
}
