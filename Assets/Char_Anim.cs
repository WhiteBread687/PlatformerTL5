using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class Char_Anim : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5f;

    public float mult = 2f;
    public float dur = 4f;

    [Header("Dash")]
    public float dashSpeed = 14f;
    public float dashTime = 0.15f;
    public float dashCooldown = 0.5f;
    public bool disableGravityDuringDash = true;

    private bool isDashing = false;
    private float nextDashTime = 0f;

    [Header("Shooting")] //Nastia: added shooting variables
    public Transform firePoint;
    public GameObject bulletPrefab;
    public float bulletSpeed = 12f;


    private bool moving = false;
    private bool facingLeft = false;
    private bool isGrounded = false;

    [Header("Audio")] //Nastia: added header just for organization in the inspector
    public AudioSource sfxSource;
    public AudioClip jumpClip;
    public AudioClip powerupClip;
    public AudioClip coinClip;
    public AudioClip enemyKillClip;
    public AudioClip levelFinishClip;
    public AudioClip shootClip; //Nastia: added shooting sound effect clip

    Rigidbody2D rb;
    Animator animator;

    private float defaultGravityScale;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        defaultGravityScale = rb.gravityScale;

        if (sfxSource == null)
            sfxSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!isDashing)
        {
            HandleMovement();
            HandleJump();
        }

        HandleDash();
        HandleShoot(); //Nastia: added shooting handler
        HandleAnimation();
    }

    void HandleMovement()
    {
        float horizontal = 0f;

        if (Keyboard.current.aKey.isPressed)
        {
            horizontal = -1f;
            facingLeft = true;
            moving = true;
        }
        else if (Keyboard.current.dKey.isPressed)
        {
            horizontal = 1f;
            facingLeft = false;
            moving = true;
        }
        else
        {
            moving = false;
        }

        rb.linearVelocity = new Vector2(horizontal * moveSpeed, rb.linearVelocity.y);
    }

    void HandleJump()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            if (jumpClip != null)
            {
                sfxSource.PlayOneShot(jumpClip);
            }
        }
    }

    void HandleDash()
    {
        // Left Shift to dash
        if (Keyboard.current.leftShiftKey.wasPressedThisFrame && Time.time >= nextDashTime && !isDashing)
        {
            StartCoroutine(DashCoroutine());
        }
    }

    void HandleShoot() //Nastia: added shooting function
{
    if (Keyboard.current.cKey.wasPressedThisFrame)
    {
        Vector2 dir = facingLeft ? Vector2.left : Vector2.right;

        GameObject b = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        sfxSource.PlayOneShot(shootClip);

        Rigidbody2D brb = b.GetComponent<Rigidbody2D>();
        if (brb != null) //checks that the bullet prefab has a Rigidbody2D component
            brb.linearVelocity = dir * bulletSpeed;
    }
}

    IEnumerator DashCoroutine()
    {
        isDashing = true;
        nextDashTime = Time.time + dashCooldown;

        float dir = 0f;
        if (Keyboard.current.aKey.isPressed) dir = -1f;
        else if (Keyboard.current.dKey.isPressed) dir = 1f;
        else dir = facingLeft ? -1f : 1f;

        float originalGravity = rb.gravityScale;
        if (disableGravityDuringDash)
            rb.gravityScale = 0f;

        rb.linearVelocity = new Vector2(dir * dashSpeed, 0f);

        yield return new WaitForSeconds(dashTime);

        if (disableGravityDuringDash)
            rb.gravityScale = originalGravity;

        isDashing = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("powerup"))
        {
            StartCoroutine(Pickup(other));
            if (powerupClip != null)
            {
                sfxSource.PlayOneShot(powerupClip);
            }
        }
    }

    IEnumerator Pickup(Collider2D powerup)
    {
        Destroy(powerup.gameObject);
        moveSpeed *= mult;
        yield return new WaitForSeconds(dur);
        moveSpeed /= mult;
    }

    public void PlayCoinSFX()
    {
        if (coinClip != null)
        {
            sfxSource.PlayOneShot(coinClip);
        }
    }

    public void PlayEnemyKillSFX()
    {
        sfxSource.PlayOneShot(enemyKillClip);
    }

    public void PlayLevelFinishSfx()
    {
        sfxSource.PlayOneShot(levelFinishClip);
    }

    void HandleAnimation()
    {
        animator.SetBool("Moving", moving);
        animator.SetBool("FacingLeft", facingLeft);

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        isGrounded = true;
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
    }
}