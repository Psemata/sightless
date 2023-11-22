using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class CharacterController2D2 : MonoBehaviour
{
    // Move player in 2D space
    public float maxSpeed = 3.4f;
    public float jumpHeight = 6.5f;
    public float gravityScale = 1.5f;
    public Camera mainCamera;

    public Material shockWave;
    public float waveSpeed = 1;
    public float minWaveSize = 0.1f;
    public float maxWaveSize = 0.9f;

    bool facingRight = true;
    float moveDirection = 0;
    bool isGrounded = false;
    Rigidbody2D r2d;
    CapsuleCollider2D mainCollider;
    Transform t;
    AudioSource clapAudio;

    private bool expanding1 = false;
    private bool expanding2 = false;
    private float expansion1 = 0.0f;
    private float expansion2 = 0.0f;

    private bool fading_out1 = false;
    private bool fading_out2 = false;

    // Use this for initialization
    void Start()
    {
        expansion1 = minWaveSize;
        expansion2 = minWaveSize;

        t = transform;
        r2d = GetComponent<Rigidbody2D>();
        mainCollider = GetComponent<CapsuleCollider2D>();
        clapAudio = GetComponent<AudioSource>();
        r2d.freezeRotation = true;
        r2d.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        r2d.gravityScale = gravityScale;
        facingRight = t.localScale.x > 0;
    }

    // Update is called once per frame
    void Update()
    {
        // Movement controls
        if (
            (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
            && (isGrounded || Mathf.Abs(r2d.velocity.x) > 0.01f)
        )
        {
            moveDirection = Input.GetKey(KeyCode.A) ? -1 : 1;
        }
        else
        {
            if (isGrounded || r2d.velocity.magnitude < 0.01f)
            {
                moveDirection = 0;
            }
        }

        // Change facing direction
        if (moveDirection != 0)
        {
            if (moveDirection > 0 && !facingRight)
            {
                facingRight = true;
                t.localScale = new Vector3(
                    Mathf.Abs(t.localScale.x),
                    t.localScale.y,
                    transform.localScale.z
                );
            }
            if (moveDirection < 0 && facingRight)
            {
                facingRight = false;
                t.localScale = new Vector3(
                    -Mathf.Abs(t.localScale.x),
                    t.localScale.y,
                    t.localScale.z
                );
            }
        }

        // Jumping
        if (Input.GetKeyDown(KeyCode.W) && isGrounded)
        {
            r2d.velocity = new Vector2(r2d.velocity.x, jumpHeight);
        }

        Vector2 screenPos = mainCamera.WorldToScreenPoint(transform.position);

        shockWave.SetVector(
            "_Position",
            new Vector2(screenPos.x / Screen.width, screenPos.y / Screen.height)
        );

        if (Input.GetKeyDown(KeyCode.Space) && !expanding1 && !fading_out1)
        {
            clapAudio.Play();
            expanding1 = true;
        }
        else if (
            Input.GetKeyDown(KeyCode.Space)
            && (expanding1 || fading_out1)
            && !expanding2
            && !fading_out2
        )
        {
            clapAudio.Play();
            expanding2 = true;
        }

        UpdateWave2();
    }

    void UpdateWave2()
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

    void FixedUpdate()
    {
        Bounds colliderBounds = mainCollider.bounds;
        float colliderRadius = mainCollider.size.x * 0.4f * Mathf.Abs(transform.localScale.x);
        Vector3 groundCheckPos =
            colliderBounds.min
            + new Vector3(colliderBounds.size.x * 0.5f, colliderRadius * 0.9f, 0);
        // Check if player is grounded
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheckPos, colliderRadius);
        //Check if any of the overlapping colliders are not player collider, if so, set isGrounded to true
        isGrounded = false;
        if (colliders.Length > 0)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i] != mainCollider)
                {
                    isGrounded = true;
                    break;
                }
            }
        }

        // Apply movement velocity
        r2d.velocity = new Vector2((moveDirection) * maxSpeed, r2d.velocity.y);

        // Simple debug
        Debug.DrawLine(
            groundCheckPos,
            groundCheckPos - new Vector3(0, colliderRadius, 0),
            isGrounded ? Color.green : Color.red
        );
        Debug.DrawLine(
            groundCheckPos,
            groundCheckPos - new Vector3(colliderRadius, 0, 0),
            isGrounded ? Color.green : Color.red
        );
    }
}
