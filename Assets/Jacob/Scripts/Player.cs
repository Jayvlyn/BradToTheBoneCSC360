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

	// Animation Facade
	private PlayerAnimationFacade animationFacade;

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

		// Create and initialize animation facade
		animationFacade = gameObject.AddComponent<PlayerAnimationFacade>();
		animationFacade.Initialize(this, animator, spriteRenderer, walkAudio, waterWalkAudio, blinker);
	}

	// Update is called once per frame
	void Update()
	{
		//DoParticles();
		currentSpeed = rb.velocity.magnitude;

		// Update thermometer and sizzle sound
		thermometer.value = Mathf.Lerp(thermometer.value, feetHeat, 0.5f);
		sizzleAudio.volume = Mathf.Clamp((feetHeat - 5) * Mathf.Pow(1.5f, (1 - (2 * 7.4f) + feetHeat)), 0, 1);

		if (!stunned)
		{
			if (moveState != MoveState.Dead)
				ReadInput();

			if (moveInput != Vector2.zero && !animator.GetBool("Running"))
			{
				animationFacade.TriggerRunAnimation();
			}
		}
		else // Stunned!
		{
			moveInput = Vector2.zero;
			if (stunTimer > 0.0f) stunTimer -= Time.deltaTime;
			else
			{
				stunned = false;
				animationFacade.ResetFallAnimation();
			}
		}

		HeatAndCool();

		if (useDebugInput) ReadDebugInputs();

		// Update animations through facade
		animationFacade.UpdateAnimations(moveInput, currentSpeed, feetHeat, maxHeat, percent, onWater);
	}

	private void FixedUpdate()
	{
		float velocityChangeSpeed = 0.3f;

		smoothMoveInput.x = Mathf.SmoothDamp(smoothMoveInput.x, moveInput.x, ref movementInputSmoothVelocity.x, velocityChangeSpeed);
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
			feetHeat = Mathf.Lerp(feetHeat, 1.0f, 2 / feetHeat * Time.deltaTime);
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
			if (moveState == MoveState.Dead)
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

		if (moveState != MoveState.Dead)
		{
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
				if (collision.GetComponent<IKickable>().OnKicked(this))
				{
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

		if (newState == MoveState.Running)
		{
			animationFacade.TriggerRunAnimation();
			animationFacade.SetRunningState(true);
		}
		else if (newState == MoveState.Turning)
		{
			animationFacade.SetTurningState(true);
		}
		else if (newState == MoveState.Idle)
		{
			animationFacade.SetTurningState(false);
			animationFacade.SetRunningState(false);
		}
		else if (newState == MoveState.Dead)
		{
			animationFacade.TriggerDeathAnimation();
			StartCoroutine(animationFacade.LowerPlayerIntoWater());
			stunned = true;
			rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, 10 * Time.deltaTime);
		}
		else if (newState == MoveState.Falling)
		{
			animationFacade.TriggerFallAnimation();
			stunned = true;
			stunTimer = stunTime;
		}
		else if (newState == MoveState.Kicking)
		{
			animationFacade.TriggerKickAnimation();
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