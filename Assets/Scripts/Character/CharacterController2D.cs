using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController2D : MonoBehaviour
{
    // Amount of force added when the player jumps.
    [SerializeField] private float m_JumpForce = 250f;
    // How much to smooth out the movement
    [Range(0, .3f)][SerializeField] private float m_MovementSmoothing = .05f;
    // Whether or not a player can steer while jumping
    [SerializeField] private bool m_AirControl = false;
    // A mask determining what is ground to the character
    [SerializeField] private LayerMask m_WhatIsGround;
    // A position marking where to check if the player is grounded.
    [SerializeField] private Transform m_GroundCheck;
    // A position marking where to check for ceilings
    [SerializeField] private Transform m_CeilingCheck;

    private SpriteRenderer spr;
    public GameObject touchKey;

    [SerializeField] private Camera mainCamera;
    [SerializeField] private Material shockWave;
    [SerializeField] private float waveSpeed = 1;
    [SerializeField] private float minWaveSize = 0.3f;
    [SerializeField] private float maxWaveSize = 0.9f;
    [SerializeField] private float slopeCheckDistance;

    private bool expanding1 = false;
    private bool expanding2 = false;
    private float expansion1 = 0.0f;
    private float expansion2 = 0.0f;

    private bool fading_out1 = false;
    private bool fading_out2 = false;

    // Speed of the character
    private float runSpeed = 15f;

    // Direction of the character
    private float horizontalMove = 0f;
    // Is the character jumping
    private bool jump = false;
    // Is the character crouching
    private bool crouch = false;

    // The stump Karna can push
    private Transform stump;

    // Radius of the overlap circle to determine if grounded
    const float k_GroundedRadius = .1f;
    // Whether or not the player is grounded
    private bool m_Grounded;
    // Is clapping
    private bool m_IsClapping;
    // If the character is flipping
    public bool m_IsFlipping;
    // Radius of the overlap circle to determine if the player can stand up
    const float k_CeilingRadius = .1f;
    // Rigid body of the character gameObject
    private Rigidbody2D m_Rigidbody2D;
    private CapsuleCollider2D cc;
    // For determining which way the player is currently facing.
    private bool m_FacingRight = true;
    // Velocity vector of the of the character
    private Vector3 velocity = Vector3.zero;
    // Animator element
    private Animator m_Animator;

    // Karna's state
    private bool hidden;
    private bool hasTorch = false;
    private bool IsNearBush = false;

    // Sound to play when Karna falls back to ground
    public AudioSource grassSound;
    /// <summary>
    /// Function which is called before the start of the script
    /// </summary>
    private void Awake()
    {
        this.m_Rigidbody2D = GetComponent<Rigidbody2D>();
        this.m_Animator = GetComponent<Animator>();
        this.spr = GetComponent<SpriteRenderer>();
        this.cc = GetComponent<CapsuleCollider2D>();

        this.touchKey.SetActive(false);

        this.expansion1 = minWaveSize;
        this.expansion2 = minWaveSize;

        this.hidden = false;
        this.m_Grounded = true;
    }

    /// <summary>
    /// Function which is called each fps
    /// </summary>
    private void Update()
    {
        if (GameManager.Instance.State != GameState.Death)
        {
            m_IsClapping = false;

            // If the character is jumping
            if (Input.GetButtonDown("Jump"))
            {
                m_Animator.SetBool("KarnaJump", true);
                jump = true;
            }

            // Get the direction of the character
            horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

            // If the character is crouching or not
            if (Input.GetButtonDown("Crouch"))
            {
                m_Animator.SetBool("KarnaCrouch", true);
                crouch = true;
            }
            else if (Input.GetButtonUp("Crouch"))
            {
                m_Animator.SetBool("KarnaCrouch", false);
                crouch = false;
            }

            // Wave
            Vector2 screenPos = mainCamera.WorldToScreenPoint(transform.position);
            shockWave.SetVector(
                "_Position",
                new Vector2(screenPos.x / Screen.width, screenPos.y / Screen.height)
            );

            if (!crouch && !jump && horizontalMove == 0)
            {
                if (Input.GetButtonDown("Fire2"))
                {
                    m_Animator.SetTrigger("KarnaLongClap");
                    m_IsClapping = true;
                }
                else if (Input.GetButtonDown("Fire3"))
                {
                    m_Animator.SetTrigger("KarnaClap");
                    m_IsClapping = true;
                }
            }

            if (m_IsClapping)
            {
                if (!expanding1 && !fading_out1)
                {
                    expanding1 = true;
                }
                else if ((expanding1 || fading_out1) && !expanding2 && !fading_out2)
                {
                    expanding2 = true;
                }
            }

            UpdateWave();

            if (!m_IsClapping && IsNearBush)
            {
                if (Input.GetKeyDown(KeyCode.F))
                {
                    if (!hidden)
                    {
                        this.m_Rigidbody2D.velocity = new Vector2(0f, 0f);
                        gameObject.layer = LayerMask.NameToLayer("KarnaHidden");
                        this.cc.isTrigger = true;
                        this.m_Rigidbody2D.isKinematic = true;
                        this.spr.enabled = false;
                        this.hidden = true;
                    }
                    else
                    {
                        gameObject.layer = LayerMask.NameToLayer("KarnaVisible");
                        this.cc.isTrigger = false;
                        this.m_Rigidbody2D.isKinematic = false;
                        this.spr.enabled = true;
                        this.hidden = false;
                    }
                }
            }

            if (!m_IsClapping && !crouch && !hidden)
            {
                // Move our character
                Move(horizontalMove * Time.fixedDeltaTime, crouch, jump);
                jump = false;
            }
        }
    }

    /// <summary>
    /// Function which is called each amount of set time
    /// </summary>
    private void FixedUpdate()
    {
        bool wasGrounded = m_Grounded;
        m_Grounded = false;

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                m_Grounded = true;
                if (!wasGrounded)
                {
                    m_Animator.SetBool("KarnaJump", false);
                    m_Animator.SetBool("KarnaFalling", false);
                    grassSound.Play();
                }

            }
        }

        if (!m_Grounded && !jump)
        {
            m_Animator.SetBool("KarnaFalling", true);
        }
    }

    public void Move(float move, bool crouch, bool jump)
    {
        // If crouching, check to see if the character can stand up
        if (!crouch)
        {
            // If the character has a ceiling preventing them from standing up, keep them crouching
            if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
            {
                crouch = true;
            }
        }

        //only control the player if grounded or airControl is turned on
        if (m_Grounded || m_AirControl)
        {
            // Can't move if the character is still flipping
            if (!m_IsFlipping)
            {
                m_Animator.SetFloat("KarnaSpeed", Mathf.Abs(move));
                // Move the character by finding the target velocity
                Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
                // And then smoothing it out and applying it to the character
                m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref velocity, m_MovementSmoothing);
            }
            else
            {
                m_Animator.SetFloat("KarnaSpeed", 0f);
            }

            // If the input is moving the player right and the player is facing left...
            if (move > 0 && !m_FacingRight && !m_IsFlipping && !GetComponent<ClimbingLadder>().isClimbing)
            {
                // ... flip the player.
                Flip();
            }
            // Otherwise if the input is moving the player left and the player is facing right...
            else if (move < 0 && m_FacingRight && !m_IsFlipping && !GetComponent<ClimbingLadder>().isClimbing)
            {
                // ... flip the player.
                Flip();
            }
        }
        // If the player should jump...
        if (m_Grounded && jump)
        {
            // Add a vertical force to the player.
            m_Grounded = false;
            m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
        }
    }

    void UpdateWave()
    {
        if (expanding1)
        {
            updateVector(ref expansion1, ref expanding1, "_SizeRing1", Time.deltaTime * waveSpeed);
            if (!expanding1)
                fading_out1 = true;
        }
        else if (fading_out1)
        {
            updateVector(
                ref expansion1,
                ref fading_out1,
                "_SizeRing1",
                -(Time.deltaTime * waveSpeed)
            );
        }
        if (expanding2)
        {
            updateVector(ref expansion2, ref expanding2, "_SizeRing2", Time.deltaTime * waveSpeed);
            if (!expanding2)
                fading_out2 = true;
        }
        else if (fading_out2)
        {
            updateVector(
                ref expansion2,
                ref fading_out2,
                "_SizeRing2",
                -(Time.deltaTime * waveSpeed)
            );
        }
    }

    void updateVector(ref float sizeRing, ref bool inRoutine, string bufferReference, float change)
    {
        if (sizeRing + change > maxWaveSize)
        {
            sizeRing = maxWaveSize;
            inRoutine = false;
        }
        else if (sizeRing + change < minWaveSize)
        {
            sizeRing = minWaveSize;
            inRoutine = false;
        }
        else
        {
            sizeRing += change;
        }
        shockWave.SetFloat(bufferReference, smoothstep(0f, 1f, sizeRing));
    }

    float smoothstep(float edge0, float edge1, float x)
    {
        // Scale, bias and saturate x to 0..1 range
        x = clamp((x - edge0) / (edge1 - edge0), minWaveSize, maxWaveSize);
        // Evaluate polynomial
        return x * x * x * (x * (x * 6 - 15) + 10);
    }

    float clamp(float x, float lowerlimit, float upperlimit)
    {
        if (x < lowerlimit)
            x = lowerlimit;
        if (x > upperlimit)
            x = upperlimit;
        return x;
    }

    public void TakeDamage()
    {
        m_Animator.SetBool("KarnaDeath", true);

        GameManager.Instance.UpdateGameState(GameState.Death);
    }

    public void RespawnPoint(Vector2 vec)
    {
        this.transform.position = vec;
        m_Animator.SetBool("KarnaDeath", false);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Bush"))
        {
            IsNearBush = true;
            this.touchKey.SetActive(IsNearBush);
        }
        else if (col.CompareTag("torch"))
        {
            hasTorch = true;
            col.gameObject.SetActive(false);
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Bush"))
        {
            IsNearBush = false;
            this.touchKey.SetActive(IsNearBush);
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("StumpPush"))
        {
            this.stump = col.gameObject.transform.parent.gameObject.transform;
        }
        else if (col.gameObject.CompareTag("Wine"))
        {
            if (hasTorch)
            {
                col.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Launch the flip animation and flip the character
    /// </summary>
    public void Flip()
    {
        m_IsFlipping = true;
        // Switch the way the player is labelled as facing.
        m_FacingRight = !m_FacingRight;
        m_Animator.SetTrigger("KarnaTurn");
    }

    /// <summary>
    /// Correcting the offset of the turn animation
    /// </summary>
    public void FlipCorrection()
    {
        if (!m_IsFlipping)
        {
            if (m_FacingRight)
            {
                gameObject.transform.Translate(0.005f, 0f, 0f);
            }
            else
            {
                gameObject.transform.Translate(-0.005f, 0f, 0f);
            }
        }
    }
}
