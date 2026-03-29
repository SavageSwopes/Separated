using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Unity.Cinemachine;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;


public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Variables")]
    [SerializeField] private float sprintMultiplier = 1.5f;
    [SerializeField] private float speed = 1.0f;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] float jumpForce;
    [SerializeField] float sprintingJumpForce;
    private bool isSprinting;
    private float horizontal;
    private float facingDirection = 1f;


    [Header("Grounding")]
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask platformLayer;
    [SerializeField] private Transform groundCheck;
    private BoxCollider2D groundCheckCollider;

    [Header("Animation Variables")]
    [SerializeField] private Animator animator;

    [Header("Wall Check")]
    [SerializeField] private float wallCheckRadius = 0.2f;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private Transform wallCheck;

    [Header("Wall Movement")]
    [SerializeField] private float wallSlideSpeed = 2;
    //Wall Jumping
    [SerializeField] private Vector2 wallJumpPower = new Vector2(5f, 10f);
    [SerializeField] private float wallJumpTime = 0.2f;
    private BoxCollider2D wallBoxCollider;
    private bool isWallSliding;
    private bool isWallJumping;
    private bool isFacingRight = true;
    private float wallJumpDirection;
    private float wallJumpTimer;
    private bool grounded;
    private AudioManager audioManager;


    [Header("Echo Location")]
    private GameObject currentEcho;
    [SerializeField] private GameObject echoLocation;
    [SerializeField] private float cooldownTimer = 35f;
    [SerializeField] private float echoChargeTime = 3f;
    [SerializeField] private float echoChargeTimer = 0f;
    [SerializeField] private float echoOffset = 1.5f;
    private bool isOnCooldown = false;
    private bool isCharging = false;
    private Vector2 direction = Vector2.right;


    [Header("Camera Zoom Settings")]
    [SerializeField] CinemachineCamera cam;
    [SerializeField] float defaultSize;
    [SerializeField] private float zoomedSize = 4f;
    [SerializeField] private float zoomSpeed = 5f;
    private float targetSize;
    private bool isZooming = false;


    [Header("Gravity")]
    [SerializeField] private float baseGravity = 2f;
    [SerializeField] private float maxFallSpeed = 18f;
    [SerializeField] private float fallSpeed = 2f;


    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        rb = GetComponent<Rigidbody2D>();
        defaultSize = cam.Lens.OrthographicSize;
    }


    private void Update()
    {
        ProcessWallSlide();
        ProccessWallJump();
        Gravity();
        if (!isWallJumping)
        {
            Flip();
        }
        //Checks to see if charging and if so, starts a counter that once equals echoTime Launches the echo 
        if (isCharging && currentEcho != null)
        {
            Vector2 offset = new Vector2(facingDirection * echoOffset, 0f);
            currentEcho.transform.position = rb.position + offset;

            if (!isZooming && !isOnCooldown)
            {
                float t = Mathf.Clamp01(echoChargeTimer / echoChargeTime);

                targetSize = Mathf.Lerp(defaultSize, zoomedSize, t);

                cam.Lens.OrthographicSize = Mathf.Lerp(cam.Lens.OrthographicSize, targetSize, Time.deltaTime * zoomSpeed);
            }


            echoChargeTimer += Time.deltaTime;

            if(echoChargeTimer > echoChargeTime)
            {
                Destroy(currentEcho);
                Launch();
                
            }
        }

        //Camera zoom code for when charging

            bool actuallySprinting = isSprinting && horizontal != 0;
            grounded = isGrounded();
            animator.SetFloat("yVelocity", rb.linearVelocity.y);
            animator.SetFloat("magnitude", Mathf.Abs(rb.linearVelocity.x));
            animator.SetBool("isWallSliding", isWallSliding);
            animator.SetBool("isGrounded", grounded);
            animator.SetBool("isRunning", actuallySprinting);
            animator.SetBool("isWallJumping", isWallJumping);
    }
     

    private void Gravity()
    {
        if (rb.linearVelocity.y < 0)
        {
            rb.gravityScale = baseGravity * fallSpeed; //Falls increasingly faster
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -maxFallSpeed));
        }
        else
        {
            rb.gravityScale = baseGravity;
        }
    }


    //Code for doing echo location
    public void Attack(InputAction.CallbackContext context)
    {
        if (context.performed && !isOnCooldown)
        {
            isCharging = true;
            echoChargeTimer = 0f;
            currentEcho = Instantiate(echoLocation, rb.position, Quaternion.identity);
            Animator animate = currentEcho.GetComponent<Animator>();

            animate.SetBool("isCharging", true);
        }
        if(context.canceled && !isOnCooldown)
        {
            isCharging = false;
            cam.Lens.OrthographicSize = defaultSize;
            if(currentEcho != null)
            {
                Destroy(currentEcho);
                currentEcho = null;
            }
        }
    }

   

    public void Move(InputAction.CallbackContext context)
    {
           horizontal = context.ReadValue<Vector2>().x;
        if (horizontal > 0)
        {
            direction = Vector2.right;
            facingDirection = 1f;
        }
        else if (horizontal < 0)
        {
            direction= Vector2.left;
            facingDirection = -1f;
        }
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isSprinting = true;
        }
        else
        {
            isSprinting = false;
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (isGrounded())
        {
            if (!isCharging)
            {
                if (context.performed)
                {
                    if (isSprinting)
                    {
                        rb.linearVelocity = new Vector2(rb.linearVelocity.x, sprintingJumpForce);
                        animator.SetTrigger("jump");
                        audioManager.PlaySFX(audioManager.jump);
                    }
                    else
                    {
                        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                        animator.SetTrigger("jump");
                        audioManager.PlaySFX(audioManager.jump);
                    }


                }
                else if (context.canceled && rb.linearVelocity.y > 0f)
                {
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
                    animator.SetTrigger("jump");
                    audioManager.PlaySFX(audioManager.jump);
                }
            }
        }


        if(context.performed && wallJumpTimer > 0f)
        {
            isWallJumping = true;
            rb.linearVelocity = new Vector2(wallJumpDirection * wallJumpPower.x, wallJumpPower.y);
            wallJumpTimer = 0;

            audioManager.PlaySFX(audioManager.jump);

            //Force spite flip
            if (transform.localScale.x != wallJumpDirection)
            {
                isFacingRight = !isFacingRight;
                Vector3 ls = transform.localScale;
                ls.x *= -1f;
                transform.localScale = ls;
            }

            Invoke(nameof(CancelWallJump), wallJumpTime + 0.1f); //Wall Jump = 0.5f -- Jump again = 0.6f
        }

    }

    private bool WallCheck()
    {
        if (Physics2D.OverlapCircle(wallCheck.position, wallCheckRadius, wallLayer))
        {
            return true;
        }
        return false;
    }

    private bool isGrounded()
    {
        
        if (Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer) || Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, platformLayer))
        {
            isWallJumping = false;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Checks to see if not grounded and on a wall, then slows down fall.
    /// </summary>
    private void ProcessWallSlide()
    {
        if(!isGrounded() && WallCheck() && horizontal != 0)
        {
            isWallSliding = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -wallSlideSpeed)); //caps fall rate
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void ProccessWallJump()
    {
        if (isWallSliding)
        {
            wallJumpDirection = -transform.localScale.x;
            wallJumpTimer = wallJumpTime;

            CancelInvoke(nameof(CancelWallJump));
        }   
        else if (wallJumpTimer > 0f)
        {
            wallJumpTimer -= Time.deltaTime;
        }
    }

    private void CancelWallJump()
    {
        isWallJumping =  false;
    }

    /// <summary>
    /// Method for flipping spire direction.
    /// </summary>
    private void Flip()
    {
        
        if(isFacingRight && horizontal < 0 || !isFacingRight && horizontal > 0)
        {
            isFacingRight = !isFacingRight; 
            Vector3 ls = transform.localScale;
            ls.x *= -1f; //reverses character
            transform.localScale = ls;
        }
    }

    //Instantiation of launch method
    private void Launch()
    {
        isCharging = false;
        GameObject currentEcho = Instantiate(echoLocation, rb.position, Quaternion.identity);
        Location echo = currentEcho.GetComponent<Location>();
        StartCoroutine(Zoom());

        echo.Launch(direction);
        StartCoroutine(Cooldown());
    }



    private IEnumerator Cooldown()
    {
        isOnCooldown = true;

        yield return new WaitForSeconds(cooldownTimer);

        isOnCooldown = false;
    }

    private IEnumerator Zoom()
    {
        isZooming = true;

        float duration = 0.3f;
        float time = 0f;

        while (time < duration) {
            time += Time.deltaTime;
            float t = time / duration;

            //This is what creates the ripple effect
            float bounce = Mathf.Sin(t * Mathf.PI * 3) * (1 - t);

            cam.Lens.OrthographicSize = defaultSize - (bounce * 0.5f);
            yield return null;
        }
        cam.Lens.OrthographicSize = defaultSize;
        isZooming = false;
    }

 
    public void PlayFootstep()
    {
        if (grounded && horizontal != 0)
        {
            audioManager.PlaySFX(audioManager.walking);
        }
    }
    
    private void FixedUpdate()
    {
        float currentSpeed = isSprinting ? speed * sprintMultiplier : speed;
        if (!isWallJumping) //Checks to see if wall jumping, so that the wall jump doesn't get overriden
        {
            rb.linearVelocity = new Vector2(horizontal * currentSpeed, rb.linearVelocity.y);
        }
    }



}
