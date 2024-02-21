using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class scr_Player_Control : MonoBehaviour
{
    [HideInInspector] public scr_BaseEntityStats stats;
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Animator anim;

    public bool Testing;

    [Header("Button Settings")]
    public ButtonsClass bt;

    [Header("Sprite Settings")]
    public GameObject spriteHolder;
    public GameObject colliderHolder;
    public Sprite defaultSprite;
    public bool facingRight;
    private Vector3 spriteScaleStand;
    private Vector3 spriteScaleCrouch;
    private Vector3 boxColliderStand;
    private Vector3 boxColliderCrouch;
    private GameObject boxColliderStanding;
    private GameObject boxColliderCrouching;
    private GameObject boxColliderDashing;

    [Header("Movement Settings")]
    public float moveSpeedTest;//15
    private float moveInput;

    [Header("Jump Settings")]
    public float jumpForce; //30
    public float jumpTime; //0.1
    public bool isJumping;
    private float jumpTimeCounter;

    [Header("Walljump Settings")]
    public Transform wallJumpRayPos1;
    public Transform wallJumpRayPos2;
    public Transform wallJumpRayPos3;
    public Transform wallJumpRayPos4;
    public float wallRayDistance; //1.25
    public float wallJumpRayDistanceMultiplier; //1.25
    public bool isJumping2;
    public float wallJumpTime; //0.2
    public LayerMask wallLayer;

    [Header("Wall Slide")]
    public float slideSpeed; //5
    public bool isSliding;

    [Header("Fall Settings")]
    public float fallMaxSpeed; //35

    [Header("Dash Settings")]
    public float dashSpeedTest; //26
    public float dashTime; //0.7
    public bool isDashing;
    private bool isDashingOnAir;
    private float dashCurrentTime;

    [Header("Crouch Settings")]
    public bool isCrouching;
    private bool forceCrouch;

    [Header("Attack Settings")]
    public data_Weapon equipedWeapon;
    public bool isAttacking;

    [Header("Ground Check")]
    public Transform feetPosition;
    public Transform headPosition;
    public float checkRadius; //0.38
    public LayerMask whatIsGround;
    public LayerMask whatIsCeiling;
    public bool isGrounded;
    public bool isCeiling;

    [Header("Smooth Move")]
    public float smoothCoeficient; //6
    public float smoothTimer; //0.1
    private float smoothCurrentTimer;
    private bool activeSmoothMove;
    private Vector3 vecSmooth;

    void Awake()
    {
        stats = GetComponent<scr_BaseEntityStats>();
        rb = GetComponent<Rigidbody2D>();
        anim = transform.Find("SpriteHolder").gameObject.GetComponentInChildren<Animator>();
    }
    void Start()
    {
        boxColliderStand = colliderHolder.transform.localScale;
        boxColliderCrouch = new Vector3(boxColliderStand.x, boxColliderStand.y / 2, boxColliderStand.z);

        boxColliderStanding = colliderHolder.transform.Find("Self-Hitbox-Standing").gameObject;
        boxColliderCrouching = colliderHolder.transform.Find("Self-Hitbox-Crouching").gameObject;
        boxColliderDashing = colliderHolder.transform.Find("Self-Hitbox-Dashing").gameObject;

        boxColliderStanding.SetActive(true);
        boxColliderCrouching.SetActive(false);
        boxColliderDashing.SetActive(false);

        spriteScaleStand = spriteHolder.transform.localScale;
        spriteScaleCrouch = new Vector3(spriteScaleStand.x, spriteScaleStand.y / 2, spriteScaleStand.z);
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(feetPosition.position, checkRadius, whatIsGround);
        isCeiling = Physics2D.OverlapCircle(headPosition.position, checkRadius, whatIsCeiling);

        if (Testing)
        {
            stats.speed.Move = moveSpeedTest;
            stats.speed.Dash = dashSpeedTest;
        }

        Jump();
        WallJump();
        WallSlide();
        FallControl();
        Dash();
        Crouch();

        ColliderSpriteSizeControl();
        AnimationControl();
        SmoothMovement();
    }

    void FixedUpdate()
    {
        Move();
    }

    void LateUpdate()
    {

    }

    #region OnCollision
    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other != null)
        {
            if(other.collider.gameObject.layer == 7)
            {
                if(isDashing && rb.velocity.y < 0) // parar o dash quando desce do ar
                {
                    isDashing = false;
                }
            }
        }
    }
    private void OnCollisionStay2D(Collision2D other)
    {
        if(other != null)
        {
            //if(other.gameObject.layer == 7)
            //{
            //    isGrounded = Physics2D.OverlapCircle(feetPosition.position, checkRadius, whatIsGround);
            //}
        }
    }
    private void OnCollisionExit2D(Collision2D other)
    {

    }
    #endregion

    #region OnTrigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        
    }
    private void OnTriggerStay2D(Collider2D other)
    {

    }
    private void OnTriggerExit2D(Collider2D other)
    {
        
    }
    #endregion

    void AnimationControl()
    {
        //Debug.Log(rb.velocity.x);
        if( moveInput != 0 && isGrounded && !isCrouching && !isDashing && rb.velocity.x > 0.1f ||
            moveInput != 0 && isGrounded && !isCrouching && !isDashing && rb.velocity.x < -0.1f)
        {
            anim.SetBool("Running", true);
        }
        else
        {
            anim.SetBool("Running", false);
        }

        if(isDashing && isGrounded)
        {
            anim.SetBool("Dashing", true);
        }
        else
        {
            anim.SetBool("Dashing", false);
        }

        if(isJumping)
        {
            anim.SetTrigger("Jump");
        }
        if(rb.velocity.y > 0)
        {
            anim.SetBool("Jumping", true);
        }
        else
        {
            anim.SetBool("Jumping", false);
        }

        if (isCrouching)
        {
            anim.SetBool("Crouching", true);
        }
        else
        {
            anim.SetBool("Crouching", false);
        }

        if(!isGrounded && !isSliding)
        {
            anim.SetBool("Falling", true);
        }
        else
        {
            anim.SetBool("Falling", false);
        }
    }

    void Move()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        
        if(moveInput != 0)
        {
            if(moveInput > 0)
            {
                facingRight = true;
            }
            else if(moveInput < 0)
            {
                facingRight = false;
            }

            ChangeDirection(); 
        }

        if (isDashing && isGrounded)
        {
            if (facingRight)
            {
                moveInput = 1;
            }
            else
            {
                moveInput = -1;
            }
        }

        if (isAttacking || isCrouching)
        {
            moveInput = 0;
        }

        if (moveInput > 0)
        {
            if(!isDashing)
            {
                stats.speed.Current = stats.speed.Move;
            }
            else
            {
                stats.speed.Current = stats.speed.Dash;
            }
        }
        else if (moveInput < 0)
        {
            if (!isDashing)
            {
                stats.speed.Current = -stats.speed.Move;
            }
            else
            {
                stats.speed.Current = -stats.speed.Dash;
            }
        }
        else
        {
            stats.speed.Current = 0;
        }

        if(!isAttacking)
        {
            rb.velocity = new Vector2((stats.speed.Current), rb.velocity.y);
        }
    }

    void Jump()
    {
        if (isGrounded && Input.GetKeyDown(bt.jump1) || isGrounded && Input.GetKeyDown(bt.jump2))
        {
            isJumping = true;
            jumpTimeCounter = jumpTime;
            rb.velocity = Vector2.up;
        }

        if (Input.GetKey(bt.jump1) && isJumping || Input.GetKey(bt.jump2) && isJumping)
        {
            if(jumpTimeCounter > 0)
            {
                rb.velocity = Vector2.up * jumpForce;
                jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
        }

        if(Input.GetKeyUp(bt.jump1) || Input.GetKeyUp(bt.jump2))
        {
            isJumping = false;
        }
    }

    void WallJump()
    {
        //RaycastHit2D hit1, hit2, hit3, hit4;
        bool test = Physics2D.OverlapArea(wallJumpRayPos1.position, wallJumpRayPos2.position, wallLayer);
        bool test1 = Physics2D.OverlapArea(wallJumpRayPos3.position, wallJumpRayPos4.position, wallLayer);

        //hit1 = Physics2D.Raycast(wallJumpRayPos1.position, Vector2.right, wallRayDistance * wallJumpRayDistanceMultiplier, wallLayer);
        //hit2 = Physics2D.Raycast(wallJumpRayPos1.position, Vector2.left,  wallRayDistance * wallJumpRayDistanceMultiplier, wallLayer);
        //hit3 = Physics2D.Raycast(wallJumpRayPos2.position, Vector2.right, wallRayDistance * wallJumpRayDistanceMultiplier, wallLayer);
        //hit4 = Physics2D.Raycast(wallJumpRayPos2.position, Vector2.left,  wallRayDistance * wallJumpRayDistanceMultiplier, wallLayer);

        if (/*hit1.collider != null && !isGrounded && facingRight || hit2.collider != null && !isGrounded && !facingRight ||
            hit3.collider != null && !isGrounded && facingRight || hit4.collider != null && !isGrounded && !facingRight*/true)
        {
            //Debug.Log("Wall");
            if (Input.GetKeyDown(bt.jump1) || Input.GetKeyDown(bt.jump2))
            {
                isJumping2 = true;
                jumpTimeCounter = wallJumpTime;

                if(Input.GetKey(bt.dash))
                {
                    dashCurrentTime = 0;
                    isDashing = true;
                }

                if(/*hit1.collider != null*/test)
                {
                    Vector3 v = new Vector3();
                    if (isDashing)
                    {
                        v = new Vector3(transform.position.x - Mathf.Abs(stats.speed.Dash / 3), transform.position.y + 4, transform.position.z);
                    }
                    else
                    {
                        v = new Vector3(transform.position.x - Mathf.Abs(stats.speed.Move / 3), transform.position.y + 4, transform.position.z);
                    }
                    vecSmooth = v;
                    smoothTimer = 0.1f;
                    smoothCoeficient = 6;
                    activeSmoothMove = true;
                }
                else if (/*hit2.collider != null*/test1)
                {
                    Vector3 v = new Vector3();
                    if (isDashing)
                    {
                        v = new Vector3(transform.position.x + Mathf.Abs(stats.speed.Dash / 3), transform.position.y + 4, transform.position.z);
                    }
                    else
                    {
                        v = new Vector3(transform.position.x + Mathf.Abs(stats.speed.Move / 3), transform.position.y + 4, transform.position.z);
                    }
                    vecSmooth = v;
                    smoothTimer = 0.1f;
                    smoothCoeficient = 6;
                    activeSmoothMove = true;
                }
            }
        }

        if (Input.GetKey(bt.jump1) && isJumping2 || Input.GetKey(bt.jump2) && isJumping2)
        {
            if (jumpTimeCounter > 0)
            {
                rb.velocity = Vector2.up * jumpForce;
                jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                isJumping2 = false;
            }
        }

        if (Input.GetKeyUp(bt.jump1) || Input.GetKeyUp(bt.jump2))
        {
            isJumping2 = false;
        }
    }

    void WallSlide()
    {
        //RaycastHit2D hit1, hit2, hit3, hit4;
        //hit1 = Physics2D.Raycast(wallJumpRayPos1.position, Vector2.right, wallRayDistance, wallLayer);
        //hit2 = Physics2D.Raycast(wallJumpRayPos1.position, Vector2.left,  wallRayDistance, wallLayer);
        //hit3 = Physics2D.Raycast(wallJumpRayPos2.position, Vector2.right, wallRayDistance, wallLayer);
        //hit4 = Physics2D.Raycast(wallJumpRayPos2.position, Vector2.left,  wallRayDistance, wallLayer);

        //if (hit1.collider != null && !isGrounded && facingRight && moveInput > 0 || hit2.collider != null && !isGrounded && !facingRight && moveInput < 0 ||
        //    hit3.collider != null && !isGrounded && facingRight && moveInput > 0 || hit4.collider != null && !isGrounded && !facingRight && moveInput < 0)
        //{
        //    isSliding = true;
        //}
        //else
        //{
        //    isSliding = false;
        //}
    }

    void FallControl()
    {
        if(!isGrounded && !isSliding)
        {
            if(rb.velocity.y < -fallMaxSpeed)
            {
                rb.velocity = new Vector2(rb.velocity.x, -fallMaxSpeed);
            }
        }
        else if(!isGrounded && isSliding)
        {
            if (rb.velocity.y < -slideSpeed)
            {
                rb.velocity = new Vector2(rb.velocity.x, -slideSpeed);
            }
        }
    }

    void Dash()
    {
        if(Input.GetKeyDown(bt.dash) && isGrounded)
        {
            dashCurrentTime = 0;
            isAttacking = false;
            isDashing = true;
        }

        if (Input.GetKey(bt.dash) && isDashing)
        {
            if(dashCurrentTime < dashTime || !isGrounded)
            {
                dashCurrentTime += Time.deltaTime;
            }

            if(dashCurrentTime >= dashTime && isGrounded)
            {
                isDashing = false;
            }
        }

        if(Input.GetKeyUp(bt.dash) && isGrounded)
        {
            isDashing = false;
        }

        if(!isGrounded && isDashing)
        {
            isDashingOnAir = true;
        }
        
        if(isGrounded && isDashingOnAir)
        {
            isDashingOnAir = false;
            isDashing = false;
        }
    }

    void Crouch()
    {
        if(isGrounded && Input.GetAxisRaw("Vertical") < 0 && !isDashing)
        {
            isCrouching = true;
        }
        else
        {
            isCrouching = false;
        }

        if(isCeiling && !isDashing)
        {
            isCrouching = true;
        }
    }

    void ColliderSpriteSizeControl()
    {
        if(isCrouching && !isDashing)
        {
            boxColliderStanding.SetActive(false);
            boxColliderCrouching.SetActive(true);
            boxColliderDashing.SetActive(false);
            //colliderHolder.transform.localScale = boxColliderCrouch;
            //spriteHolder.transform.localScale = spriteScaleCrouch;
        }
        else if(!isCrouching && isDashing && isGrounded)
        {
            boxColliderStanding.SetActive(false);
            boxColliderCrouching.SetActive(false);
            boxColliderDashing.SetActive(true);
            //colliderHolder.transform.localScale = boxColliderCrouch;
            //spriteHolder.transform.localScale = spriteScaleCrouch;
        }
        else
        {
            boxColliderStanding.SetActive(true);
            boxColliderCrouching.SetActive(false);
            boxColliderDashing.SetActive(false);
            //colliderHolder.transform.localScale = boxColliderStand;
            //spriteHolder.transform.localScale = spriteScaleStand;
        }
    }

    void ChangeDirection()
    {
        if(facingRight && transform.localScale.x < 0 || !facingRight && transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    }

    void SmoothMovement()
    {
        if (activeSmoothMove)
        {
            transform.position = Vector3.Lerp(transform.position, vecSmooth, Time.deltaTime * smoothCoeficient);

            smoothCurrentTimer += Time.deltaTime;
            if(smoothCurrentTimer > smoothTimer)
            {
                activeSmoothMove = false;
                smoothCurrentTimer = 0;
            }
        }
    }

    #region Gizmos
    [Header("Gizmos")]
    public bool onDrawActive = false;
    public bool onDrawSelectActive = false;
    private void OnDrawGizmos()
    {
        if(onDrawActive)
        {
            Gizmos.DrawWireSphere(feetPosition.position, checkRadius);
            Gizmos.DrawWireSphere(headPosition.position, checkRadius);

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(new Vector3(
                                            wallJumpRayPos1.position.x - wallJumpRayPos2.position.x/2,
                                            wallJumpRayPos1.position.y - wallJumpRayPos2.position.y/2,
                                            wallJumpRayPos1.position.z - wallJumpRayPos2.position.z/2),
                                new Vector3(1,1,1));

            //Gizmos.DrawRay(wallJumpRayPos1.position, Vector2.right * wallRayDistance * wallJumpRayDistanceMultiplier);
            //Gizmos.DrawRay(wallJumpRayPos1.position, Vector2.left  * wallRayDistance * wallJumpRayDistanceMultiplier);
            //Gizmos.DrawRay(wallJumpRayPos2.position, Vector2.right * wallRayDistance * wallJumpRayDistanceMultiplier);
            //Gizmos.DrawRay(wallJumpRayPos2.position, Vector2.left  * wallRayDistance * wallJumpRayDistanceMultiplier);

            //Gizmos.color = Color.yellow;
            //Gizmos.DrawRay(wallJumpRayPos1.position, Vector2.right * wallRayDistance);
            //Gizmos.DrawRay(wallJumpRayPos1.position, Vector2.left  * wallRayDistance);
            //Gizmos.DrawRay(wallJumpRayPos2.position, Vector2.right * wallRayDistance);
            //Gizmos.DrawRay(wallJumpRayPos2.position, Vector2.left  * wallRayDistance);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (onDrawSelectActive)
        {
            Gizmos.DrawWireSphere(feetPosition.position, checkRadius);
            Gizmos.DrawWireSphere(headPosition.position, checkRadius);

            Gizmos.color = Color.red;
            //Gizmos.DrawRay(wallJumpRayPos1.position, Vector2.right * wallRayDistance * wallJumpRayDistanceMultiplier);
            //Gizmos.DrawRay(wallJumpRayPos1.position, Vector2.left  * wallRayDistance * wallJumpRayDistanceMultiplier);
            //Gizmos.DrawRay(wallJumpRayPos2.position, Vector2.right * wallRayDistance * wallJumpRayDistanceMultiplier);
            //Gizmos.DrawRay(wallJumpRayPos2.position, Vector2.left  * wallRayDistance * wallJumpRayDistanceMultiplier);

            //Gizmos.color = Color.yellow;
            //Gizmos.DrawRay(wallJumpRayPos1.position, Vector2.right * wallRayDistance);
            //Gizmos.DrawRay(wallJumpRayPos1.position, Vector2.left  * wallRayDistance);
            //Gizmos.DrawRay(wallJumpRayPos2.position, Vector2.right * wallRayDistance);
            //Gizmos.DrawRay(wallJumpRayPos2.position, Vector2.left  * wallRayDistance);
        }
    }
    #endregion
}

[System.Serializable]
public class ButtonsClass
{
    public KeyCode attack1;
    public KeyCode attack2;
    public KeyCode jump1;
    public KeyCode jump2;
    public KeyCode dash;
}