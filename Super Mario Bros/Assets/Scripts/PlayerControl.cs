using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Mario
{
    public class PlayerControl : MonoBehaviour
    {
        [HideInInspector]
        public bool facingRight = true;
        [HideInInspector]
        public bool jump = false;
        public float moveForce = 365f;
        public float maxSpeed = 5f;
        // public float jumpForce = 1000f;
        [Range(1, 100)]
        public float jumpVelocity;
        public float fallMultiplier = 2.5f;
        public float lowJumpMultiplier = 2f;
        public Transform spawnPoint;
        public LayerMask groundMask;
        public LayerMask enemyMask;
        private Vector3[] groundRayCheck = new Vector3[8];
        private bool[] top = { false, false, false };
        private bool died = false;
        private bool[] stackForwardBackward = { false, false, false, false, false, false };
        private Animator anim;
        private Rigidbody2D rb2d;
        RaycastHit2D groundLeft;
        RaycastHit2D groundMid;
        RaycastHit2D groundRight;
        // Use this for initialization
        void Awake()
        {
            anim = GetComponent<Animator>();
            rb2d = GetComponent<Rigidbody2D>();

        }

        // Update is called once per frame
        void Update()
        {
            //down
            groundRayCheck[0] = new Vector3(transform.position.x + 0.0f, transform.position.y - 0.5f, transform.position.z);
            //up
            groundRayCheck[1] = new Vector3(transform.position.x - 0.0f, transform.position.y + 0.5f, transform.position.z);
            //left
            groundRayCheck[2] = new Vector3(transform.position.x - 0.5f, transform.position.y + 0.0f, transform.position.z);
            //right
            groundRayCheck[3] = new Vector3(transform.position.x + 0.4f, transform.position.y + 0.0f, transform.position.z);
            //right up
            groundRayCheck[4] = new Vector3(transform.position.x + 0.5f, transform.position.y + 0.5f, transform.position.z);
            //right down
            groundRayCheck[5] = new Vector3(transform.position.x + 0.5f, transform.position.y - 0.5f, transform.position.z);
            //left up
            groundRayCheck[6] = new Vector3(transform.position.x - 0.5f, transform.position.y + 0.5f, transform.position.z);
            //left down
            groundRayCheck[7] = new Vector3(transform.position.x - 0.5f, transform.position.y - 0.5f, transform.position.z);


            groundLeft = Physics2D.Raycast(groundRayCheck[2], Vector2.down, 0.6f, groundMask);
            groundMid = Physics2D.Raycast(transform.position, Vector2.down, 0.6f, groundMask);
            groundRight = Physics2D.Raycast(groundRayCheck[3], Vector2.down, 0.6f, groundMask);


            /*  grounded[0] = Physics2D.Linecast(transform.position, groundRayCheck[0], 1 << LayerMask.NameToLayer("Ground"));
              grounded[1] = Physics2D.Linecast(transform.position, groundRayCheck[5], 1 << LayerMask.NameToLayer("Ground"));
              grounded[2] = Physics2D.Linecast(transform.position, groundRayCheck[7], 1 << LayerMask.NameToLayer("Ground"));
              */
            RaycastHit2D tophit = Physics2D.Raycast(transform.position, Vector2.up, 0.6f, 1 << LayerMask.NameToLayer("Ground"));

            RaycastHit2D enemyHitL = Physics2D.Raycast(groundRayCheck[2], Vector2.down, 0.5f, enemyMask);
            RaycastHit2D enemyHitM = Physics2D.Raycast(transform.position, Vector2.down, 0.5f, enemyMask);
            RaycastHit2D enemyHitR = Physics2D.Raycast(groundRayCheck[3], Vector2.down, 0.5f, enemyMask);
            Debug.DrawLine(groundRayCheck[3], new Vector3(groundRayCheck[3].x, groundRayCheck[3].y-0.5f, groundRayCheck[3].z),Color.red);

            RaycastHit2D deadhit = Physics2D.Raycast(transform.position, Vector2.down, 0.6f, 1 << LayerMask.NameToLayer("DestroyArea"));

            if (deadhit.collider != null) { Death(); }

            if (!died)
            {
                if (tophit.collider != null)
                {
                    if (tophit.collider.tag == "QuestionBlock")
                    {
                        tophit.collider.GetComponent<QuestionBrickHit>().QuestionBlockBounce();
                    }

                    if (tophit.collider.tag == "BrickBlock")
                    {
                        tophit.collider.GetComponent<BrickHit>().QuestionBlockBounce();
                    }
                }
                if (enemyHitL.collider != null || enemyHitM.collider != null || enemyHitR.collider != null)
                {
                    RaycastHit2D hitRay = enemyHitL;

                    if (enemyHitL)
                    {
                        hitRay = enemyHitL;
                    }
                    else if (enemyHitM)
                    {
                        hitRay = enemyHitM;
                    }
                    else if (enemyHitR)
                    {
                        hitRay = enemyHitR;
                    }

                    if (hitRay.collider.tag == "Enemy")
                    {
                        hitRay.collider.GetComponent<EnemyAI>().Crush();
                    }
                }

                /* if (stackForwardBackward[0])
                 {
                     transform.position = new Vector3(transform.position.x - 0.001f, transform.position.y, transform.position.z);
                 }*/

                if (Input.GetButtonDown("Jump") && (groundLeft.collider != null || groundMid.collider != null || groundRight.collider != null))
                {
                    jump = true;
                }

                if (rb2d.velocity.y < 0)
                {
                    rb2d.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
                }

                else if (rb2d.velocity.y > 0 && !jump)
                {
                    rb2d.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
                }
            }

        }

        void FixedUpdate()
        {
            if (!died)
            {
                float h = Input.GetAxis("Horizontal");
                if (h > 0 || h < 0)
                {
                    anim.SetBool("Run", true);
                }
                else
                {
                    anim.SetBool("Run", false);
                }
                if (h * rb2d.velocity.x < maxSpeed)
                    rb2d.AddForce(Vector2.right * h * moveForce);

                if (Mathf.Abs(rb2d.velocity.x) > maxSpeed)
                    rb2d.velocity = new Vector2(Mathf.Sign(rb2d.velocity.x) * maxSpeed, rb2d.velocity.y);

                if (h > 0 && !facingRight) { Flip(); }

                else if (h < 0 && facingRight) { Flip(); }

                if (jump)
                {
                    anim.SetBool("Jump", true);
                    rb2d.velocity = Vector2.up * jumpVelocity;
                    // rb2d.AddForce(new Vector2(0f, jumpForce));
                }
                else
                {
                    if (groundLeft.collider != null || groundMid.collider != null || groundRight.collider != null)
                        anim.SetBool("Jump", false);
                }

                jump = false;
            }
        }

        public void Death()
        {

            anim.SetBool("Death", true);
            rb2d.gravityScale = 0;
            rb2d.velocity = new Vector2(0f, 0f);
            GetComponent<Collider2D>().enabled = false;
            died = true;

        }

        public void respawn()
        {
            transform.position = spawnPoint.position;
            CameraFollow2D.cameraFollow.ResetPosition();
        }

        void Flip()
        {
            facingRight = !facingRight;
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }
    }
}