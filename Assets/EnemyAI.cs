using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    Rigidbody2D rb;

    Animator animator; // trigger animation transitions etc.

    GameObject player; // track the main player 
   // Transform 
   bool isDead = false;

    bool isPlayerDetected = false; // not detected yet
    bool isAttacking = false;

    float detectionRange = 1.2f;

    public CoinManager cm;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindWithTag("Character"); //make sure this is set in Unity
        animator = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {

        DetectPlayer();
    }
    void Attack()
    {
        if (!isAttacking)
        { 
            isAttacking = true;
            animator.SetTrigger("Attack");
        }
    }

    void DetectPlayer()
    {
        float distace = Vector2.Distance(transform.position, player.transform.position);
        if(!isPlayerDetected)
        {
            if (distace <= detectionRange)
            {
                //player detected
                isPlayerDetected = true;
                Attack(); // start attacking

            }
            else 
            {
                isPlayerDetected = false;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;
        else if (collision.gameObject.CompareTag("Character"))
        {
            //Debug.Log("Enemy collided with player");
            // Damage player logic here
            float playerBottom = collision.collider.bounds.min.y;
            float enemyTop = this.GetComponent<Collider2D>().bounds.max.y;

            if (playerBottom > enemyTop - 0.1f)
            {
                Char_Anim p = collision.gameObject.GetComponent<Char_Anim>();
                if (p != null)
                {
                    p.PlayEnemyKillSFX();
                }
                cm.diecount++;
                Die();
            }

        }

    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;
        rb.linearVelocity  = Vector2.zero;
        rb.simulated = false;
        GetComponent<Collider2D>().enabled = false;

        animator.SetTrigger("Death");
        Destroy(gameObject, 1.0f); // destroy after 1 second to allow death animation to play

    }

}
