using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public enum MoveState
    {
        Idle,
        Running,
        Turning,
        Falling,
        Kicking,
        Dead
    }

    public MoveState moveState;

    // References
    public Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    public Slider thermometer;
    public Animator animator;
    public AudioSource splashAudio;
    public AudioSource walkAudio;
    public AudioSource waterWalkAudio;
    public AudioSource sizzleAudio;
    //public ParticleSystem  sandKickupParticles;
    public Blinker blinker;
    [Range(0, 1)] public float percent;


    [SerializeField] private float baseSpeed = 0.0f;
    private float currentSpeed = 0.0f;
    public float feetHeat = 0.0f;
    public float maxHeat = 10.0f;

    public bool onWater = false;
    private bool stunned = false;
    private float stunTimer = 0.0f;
    [SerializeField] private float stunTime = 0.0f;

    [SerializeField] private bool useDebugInput = false;

    private Vector2 moveInput;
    private Vector2 smoothMoveInput;
    private Vector2 movementInputSmoothVelocity;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //DoParticles();
        blinker.blink = (feetHeat >= percent * maxHeat);

        currentSpeed = rb.velocity.magnitude;
        animator.SetFloat("Heat", feetHeat);
        animator.SetFloat("Speed", currentSpeed);
        animator.SetFloat("MoveInputX", moveInput.x);
        animator.SetFloat("MoveInputY", moveInput.y);

        thermometer.value = Mathf.Lerp(thermometer.value, feetHeat, 0.5f);
        sizzleAudio.volume = Mathf.Clamp((feetHeat-5)*Mathf.Pow(1.5f, (1-(2*7.4f)+feetHeat)), 0, 1);

        if (!stunned)
        {
            if(moveState != MoveState.Dead)
            ReadInput();
            if (moveInput != Vector2.zero && !animator.GetBool("Running"))
            {
                animator.SetTrigger("StartRun");
            }
        }
        else // Stunned!
        {
            moveInput = Vector2.zero;
            if (stunTimer > 0.0f) stunTimer -= Time.deltaTime;
            else
            {
                stunned = false;
                animator.SetBool("Fall", false);
            }
        }

        HeatAndCool();

        if (useDebugInput) ReadDebugInputs();

        if (rb.velocity.x < 0)
        {
            spriteRenderer.flipX = true;
            //sandKickupParticles.transform.rotation = Quaternion.Euler(sandKickupParticles.transform.rotation.x, 90, 0);
        } else
        {
            spriteRenderer.flipX = false;
            //sandKickupParticles.transform.rotation = Quaternion.Euler(sandKickupParticles.transform.rotation.x, -90, 0);
        }

        animator.SetBool("Running", moveInput != Vector2.zero);



        if(animator.GetCurrentAnimatorStateInfo(0).IsName("Run")) 
        {
            if (!onWater)
            {
                walkAudio.enabled = true;
                if (waterWalkAudio.enabled) waterWalkAudio.enabled = false;
            }
            else
            {
                waterWalkAudio.enabled = true;
                if (walkAudio.enabled) walkAudio.enabled = false;
            } 
        }
        else
        {
            walkAudio.enabled = false; waterWalkAudio.enabled = false;
        } 
            

        if (((moveInput.x > 0 && rb.velocity.x < 0) || (moveInput.x < 0 && rb.velocity.x > 0)) || (moveInput == Vector2.zero && Mathf.Abs(rb.velocity.x) < 5f)) { animator.SetBool("Turning", true); }
        else 
        {
            animator.SetBool("Turning", false);
        }

        if (moveState == MoveState.Dead && !(animator.GetCurrentAnimatorStateInfo(0).IsName("Die") || animator.GetCurrentAnimatorStateInfo(0).IsName("Dead")))
        {
            animator.ResetTrigger("Kick");
            animator.ResetTrigger("StartRun");
            animator.ResetTrigger("FallStart");
            animator.SetTrigger("Die");
        }

    }

    private void FixedUpdate()
    {
        float velocityChangeSpeed = 0.3f;

        //if (Mathf.Abs(smoothMoveInput.x) <= 0.02f && moveInput != Vector2.zero) smoothMoveInput.x = moveInput.x;
        //else
        smoothMoveInput.x = Mathf.SmoothDamp(smoothMoveInput.x, moveInput.x, ref movementInputSmoothVelocity.x, velocityChangeSpeed);


        //if (rb.velocity.y <= 0.02f && moveInput != Vector2.zero) smoothMoveInput.y = moveInput.y;
        //else
        smoothMoveInput.y = Mathf.SmoothDamp(smoothMoveInput.y, moveInput.y, ref movementInputSmoothVelocity.y, velocityChangeSpeed);

        rb.velocity = smoothMoveInput * baseSpeed * Mathf.Clamp(feetHeat, 0, maxHeat - 3) * Time.deltaTime;

        
    }

    private void ReadInput()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput = moveInput.normalized;
    }

    private void HeatAndCool()
    {
        if (onWater && feetHeat > 1.0f)
        {
            feetHeat = Mathf.Lerp(feetHeat, 1.0f, 2/feetHeat * Time.deltaTime);
            //feetHeat -= 1.0f * Time.deltaTime;
        }
        else if (!onWater && feetHeat <= maxHeat) // On sand
        {
            if (currentSpeed > 0.1f) feetHeat += 0.4f * Time.deltaTime;
            else feetHeat += 0.8f * Time.deltaTime;
        }

        if (feetHeat > maxHeat && moveState != MoveState.Dead) ChangeState(MoveState.Dead);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
            if (collision.gameObject.tag == "Water")
            {
                if(moveState == MoveState.Dead)
                {
                    StartCoroutine(WaitToEnd());
                }
                else
                {
                    onWater = true;
                    splashAudio.Play();
                }
            }
            else if (collision.tag == "Surf")
        {
            collision.gameObject.GetComponent<IKickable>().OnKicked(this);
            ChangeState(MoveState.Kicking);
        }

        if(moveState != MoveState.Dead){
            if (collision.gameObject.tag == "SpikedCastle")
            {
                //ChangeState(MoveState.Kicking);
                collision.GetComponent<IKickable>().OnKicked(this);
                ChangeState(MoveState.Falling);
            }
            else if (collision.gameObject.tag == "Castle")
            {
                if (collision.GetComponent<IKickable>().OnKicked(this))
                {
                    ChangeState(MoveState.Kicking);
                    StartCoroutine(StopTime());
                }
            }
            else if (collision.gameObject.tag == "Urchin")
            {
                //ChangeState(MoveState.Kicking);
                if(collision.GetComponent<IKickable>().OnKicked(this)) {
                    ChangeState(MoveState.Falling);
                }
            }
        }
        
    }
    

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Water")
        {
            onWater = false;
        } 
    }

    private void DoParticles()
    {
        //if (!sandKickupParticles.isPlaying && currentSpeed > 0.5f) { sandKickupParticles.Play(); }
        //else if (sandKickupParticles.isPlaying && currentSpeed <= 0.5f) { sandKickupParticles.Stop(); }
    }

    private void ReadDebugInputs()
    {
        if (Input.GetKey(KeyCode.U) && feetHeat <= maxHeat)
        {
            feetHeat += 1.0f;
        }
        if (Input.GetKey(KeyCode.J) && feetHeat > 0)
        {
            feetHeat -= 1.0f;
        }
        
    }

    public void ChangeState(MoveState newState)
    {
        moveState = newState;

        if(newState == MoveState.Running)
        {
            animator.SetTrigger("StartRun");
            animator.SetBool("Running", true);
        }
        if(newState == MoveState.Turning)
        {
            animator.SetBool("Turning", true);
            
        }
        else if (newState == MoveState.Idle)
        {
            animator.SetBool("Turning", false);
            animator.SetBool("Running", false);
        }
        else if (newState == MoveState.Dead)
        {
            animator.SetTrigger("Die");
            StartCoroutine(LowerPlayerIntoWater());
            stunned = true;
            rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, 10 * Time.deltaTime);
        }
        else if (newState == MoveState.Falling)
        {
            animator.SetTrigger("FallStart");
            animator.SetBool("Fall", true);
            stunned = true;
            stunTimer = stunTime;
        }
        else if (newState == MoveState.Kicking)
        {
            animator.SetTrigger("Kick");
        }
    }

    public IEnumerator StopTime()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        if (Time.timeScale == 1.0f) Time.timeScale = 0.1f;
        yield return new WaitForSecondsRealtime(0.1f);
        Time.timeScale = 1.0f;
        ChangeState(MoveState.Idle);
    }

    public IEnumerator LowerPlayerIntoWater()
    {
        yield return new WaitForSecondsRealtime(1);
        GetComponent<SpriteRenderer>().sortingOrder = 4;
    }

    public IEnumerator WaitToEnd()
    {
        yield return new WaitForSecondsRealtime(4);
        GameManager.SwitchScene("Kyle Scene");
    }

    public bool FacingRight()
    {
        return spriteRenderer.flipX;
    }



}
