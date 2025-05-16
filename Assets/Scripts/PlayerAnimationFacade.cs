using System.Collections;
using UnityEngine;

public class PlayerAnimationFacade : MonoBehaviour
{
	private Player player;
	private Animator animator;
	private SpriteRenderer spriteRenderer;
	private AudioSource walkAudio;
	private AudioSource waterWalkAudio;
	private Blinker blinker;

	// References to be set in initialization
	public void Initialize(Player playerRef, Animator animatorRef, SpriteRenderer rendererRef,
						  AudioSource walkAudioRef, AudioSource waterWalkAudioRef, Blinker blinkerRef)
	{
		player = playerRef;
		animator = animatorRef;
		spriteRenderer = rendererRef;
		walkAudio = walkAudioRef;
		waterWalkAudio = waterWalkAudioRef;
		blinker = blinkerRef;
	}

	public void UpdateAnimations(Vector2 moveInput, float currentSpeed, float feetHeat, float maxHeat, float percent, bool onWater)
	{
		// Blinker update
		blinker.blink = (feetHeat >= percent * maxHeat);

		// Set animator parameters
		animator.SetFloat("Heat", feetHeat);
		animator.SetFloat("Speed", currentSpeed);
		animator.SetFloat("MoveInputX", moveInput.x);
		animator.SetFloat("MoveInputY", moveInput.y);

		// Handle sprite flipping based on direction
		if (player.rb.velocity.x < 0)
		{
			spriteRenderer.flipX = true;
		}
		else
		{
			spriteRenderer.flipX = false;
		}

		// Set running state
		animator.SetBool("Running", moveInput != Vector2.zero);

		// Handle audio for running
		UpdateFootstepAudio(moveInput, onWater);

		// Handle turning animation
		UpdateTurningAnimation(moveInput, player.rb.velocity);

		// Handle death animation if needed
		if (player.moveState == Player.MoveState.Dead &&
			!(animator.GetCurrentAnimatorStateInfo(0).IsName("Die") || animator.GetCurrentAnimatorStateInfo(0).IsName("Dead")))
		{
			TriggerDeathAnimation();
		}
	}

	private void UpdateFootstepAudio(Vector2 moveInput, bool onWater)
	{
		if (animator.GetCurrentAnimatorStateInfo(0).IsName("Run"))
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
			walkAudio.enabled = false;
			waterWalkAudio.enabled = false;
		}
	}

	private void UpdateTurningAnimation(Vector2 moveInput, Vector2 velocity)
	{
		if (((moveInput.x > 0 && velocity.x < 0) || (moveInput.x < 0 && velocity.x > 0)) ||
			(moveInput == Vector2.zero && Mathf.Abs(velocity.x) < 5f))
		{
			animator.SetBool("Turning", true);
		}
		else
		{
			animator.SetBool("Turning", false);
		}
	}

	public void TriggerRunAnimation()
	{
		animator.SetTrigger("StartRun");
	}

	public void TriggerFallAnimation()
	{
		animator.SetTrigger("FallStart");
		animator.SetBool("Fall", true);
	}

	public void ResetFallAnimation()
	{
		animator.SetBool("Fall", false);
	}

	public void TriggerKickAnimation()
	{
		animator.SetTrigger("Kick");
	}

	public void TriggerDeathAnimation()
	{
		animator.ResetTrigger("Kick");
		animator.ResetTrigger("StartRun");
		animator.ResetTrigger("FallStart");
		animator.SetTrigger("Die");
	}

	public void SetRunningState(bool isRunning)
	{
		animator.SetBool("Running", isRunning);
	}

	public void SetTurningState(bool isTurning)
	{
		animator.SetBool("Turning", isTurning);
	}

	public IEnumerator LowerPlayerIntoWater()
	{
		yield return new WaitForSecondsRealtime(1);
		spriteRenderer.sortingOrder = 4;
	}
}