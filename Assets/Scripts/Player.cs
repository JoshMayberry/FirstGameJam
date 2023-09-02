using Aarthificial.Typewriter.Entries;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

internal static class AnimationState {
	internal const string Idle = "idle";
	internal const string MoveUp = "move_up";
	internal const string MoveDown = "move_down";
	internal const string MoveLeft = "move_left";
	internal const string MoveRight = "move_right";
	internal const string Hurt = "hurt";
	internal const string TwistIn = "twist_in";
	internal const string TwistOut = "twist_out";
	//internal const string DoAction = "do_action";
}

internal enum SlimeFormId {
	Normal, Attack, Defend, Heal
}

internal class SlimeForm {
	internal SlimeFormId id;
	internal string key;
	internal Animator animator;

	internal float maxSpeed;
	internal float frictionSpeed;
	internal float accelerationRate;
	internal float deccelerationRate;

	internal SlimeForm(SlimeFormId id, string key, Animator animator, float maxSpeed, float frictionSpeed, float accelerationRate, float deccelerationRate) {
		this.id = id;
		this.key = key;
		this.animator = animator;

		this.maxSpeed = maxSpeed;
		this.frictionSpeed = frictionSpeed;
		this.accelerationRate = accelerationRate;
		this.deccelerationRate = deccelerationRate;
	}
}

public class Player : Speaker {
	private SlimeForm nextForm;
	private SlimeForm currentForm;

	internal Dictionary<string, SlimeForm> SlimeCatalogue { get; private set; }

	[Header("Animation")]
	[SerializeField] private Animator normalAnimator;
	[SerializeField] private Animator healAnimator;
	[SerializeField] private Animator defendAnimator;
	[SerializeField] private Animator attackAnimator;
	private string currentState = AnimationState.Idle;
	private bool isTwisting;
	private bool isAnimatingOneshot;

	[Header("Physics")]
	[SerializeField] private Transform attackCheckPosition;
	private Rigidbody2D rb;

	[Header("Input")]
	[SerializeField] private float doActionGraceTime = 0.1f;
	private Vector2 inputMove;
	private float lastDoActionPressed;
	public bool isDoActionPressed { get; private set; }
    private void Awake() {
		this.rb = this.GetComponent<Rigidbody2D>();

		this.SlimeCatalogue = new Dictionary<string, SlimeForm> {
			{ "Normal", new SlimeForm(
				id: SlimeFormId.Normal,
				key: "Normal",
				animator: normalAnimator,
				maxSpeed: 11f,
				frictionSpeed: 0.2f,
				accelerationRate: 2.5f,
				deccelerationRate: 5f
			) },
			{ "Heal", new SlimeForm(
				id: SlimeFormId.Heal,
				key: "Heal",
				animator: healAnimator,
				maxSpeed: 11f,
				frictionSpeed: 0.2f,
				accelerationRate: 2.5f,
				deccelerationRate: 5f
			) },
			{ "Defend", new SlimeForm(
				id: SlimeFormId.Defend,
				key: "Defend",
				animator: defendAnimator,
				maxSpeed: 11f,
				frictionSpeed: 0.2f,
				accelerationRate: 2.5f,
				deccelerationRate: 5f
			) },
			{ "Attack", new SlimeForm(
				id: SlimeFormId.Attack,
				key: "Attack",
				animator: attackAnimator,
				maxSpeed: 11f,
				frictionSpeed: 0.2f,
				accelerationRate: 2.5f,
				deccelerationRate: 5f
			) },
		};
		this.currentForm = this.SlimeCatalogue["Normal"];
	}

	private void Start() {
        //this.SpawnChatBubble("Testing...");

        FactEntry fact_moveCount = TypewriterEvents.instance.fact_moveCount.GetEntry<FactEntry>();
		//fact_moveCount.
	}

	void Update() {
		if (GameManager.instance.isGameOver || DialogManager.instance.isPlayerLocked) {
			return;
		}

		// Update Timers
		this.lastDoActionPressed -= Time.deltaTime;

		// Update States
		this.isDoActionPressed = (this.lastDoActionPressed > 0.01f);

		if ((this.inputMove.x == 0) && (this.inputMove.y == 0)) {
			SetAnimation(AnimationState.Idle);
		}
		else if (this.inputMove.x < 0) {
			SetAnimation(AnimationState.MoveRight);
            TypewriterEvents.instance.IncrementFact(this.typewriterContext, TypewriterEvents.instance.fact_moveCount, 1);
        }
        else if (this.inputMove.x > 0) {
			SetAnimation(AnimationState.MoveLeft);
            TypewriterEvents.instance.IncrementFact(this.typewriterContext, TypewriterEvents.instance.fact_moveCount, 1);
        }
        else if (this.inputMove.y > 0) {
			SetAnimation(AnimationState.MoveUp);
            TypewriterEvents.instance.IncrementFact(this.typewriterContext, TypewriterEvents.instance.fact_moveCount, 1);
        }
        else if (this.inputMove.y < 0) {
			SetAnimation(AnimationState.MoveDown);
            TypewriterEvents.instance.IncrementFact(this.typewriterContext, TypewriterEvents.instance.fact_moveCount, 1);
        }
    }

	private void FixedUpdate() {
		if (GameManager.instance.isGameOver || DialogManager.instance.isPlayerLocked) {
			return;
		}

		this.UpdateMovement();
	}

	void OnMove(InputValue context) {
		this.inputMove = context.Get<Vector2>();
	}

	void OnTwistNormal() {
		this.SetForm("Normal");
	}

	void OnTwistAttack() {
		this.SetForm("Attack");
	}

	void OnTwistDefend() {
		this.SetForm("Defend");
	}

	void OnTwistHeal() {
		this.SetForm("Heal");
	}

	void OnDoAction() {
		this.lastDoActionPressed = this.doActionGraceTime;
	}

	void UpdateMovement() {
		Vector2 speed_target = new Vector2(this.inputMove.x * this.currentForm.maxSpeed, this.inputMove.y * this.currentForm.maxSpeed);
		Vector2 speed_delta = speed_target - this.rb.velocity;
		
		bool is_accelerating = ((Mathf.Abs(speed_target.x) > 0.01f) || (Mathf.Abs(speed_target.y) > 0.01f));
		float accelerationRate = (is_accelerating ? this.currentForm.accelerationRate : this.currentForm.deccelerationRate);
		Vector2 velocity_force = speed_delta * accelerationRate;

		this.rb.AddForce(velocity_force, ForceMode2D.Force);
	}

	void SetForm(string newForm) {
		if ((GameManager.instance.isGameOver) || DialogManager.instance.isPlayerLocked || this.isTwisting || (newForm == this.currentForm.key)) {
			return;
		}

		this.nextForm = this.SlimeCatalogue[newForm];
		if (this.nextForm == null) {
			throw new System.Exception("Unknown form '" + newForm + "'");
		}

		this.SetAnimation(AnimationState.TwistIn);
		this.isTwisting = true;
		this.isAnimatingOneshot = true;
	}

	void SetAnimation(string animationName, bool ignoreOneshot = false) {
		if ((!ignoreOneshot && this.isAnimatingOneshot) || (animationName == this.currentState)) {
			return;
		}

		this.currentForm.animator.CrossFade(animationName, 0, 0);
		this.currentState = animationName;
	}

	internal void AnimationFinished(string animationName) {
		switch (animationName) {
			case AnimationState.TwistIn:
				this.currentForm.animator.gameObject.SetActive(false);
				this.currentForm = this.nextForm;
				this.currentForm.animator.gameObject.SetActive(true);
				this.SetAnimation(AnimationState.TwistOut, true);
				break;

			case AnimationState.TwistOut:
				this.isAnimatingOneshot = false;
				this.isTwisting = false;
				TypewriterEvents.instance.IncrementFact(this.typewriterContext, TypewriterEvents.instance.fact_twistCount, 1);
                break;

			default:
				throw new System.Exception("Unknown animation event for '" + animationName + "'");

		}
	}

	internal void AnimationStart(string animationName) {
		switch (animationName) {
			case AnimationState.MoveUp:
			case AnimationState.MoveDown:
			case AnimationState.MoveLeft:
			case AnimationState.MoveRight:
				AudioManager.instance.PlayOneShot(FmodEvents.instance.moveSound, this.gameObject.transform.position);
				break;

			case AnimationState.TwistIn:
				AudioManager.instance.PlayOneShot(FmodEvents.instance.twistSound, this.gameObject.transform.position);
				break;

			case AnimationState.Hurt:
				AudioManager.instance.PlayOneShot(FmodEvents.instance.hurtSound, this.gameObject.transform.position);
				break;

			default:
				throw new System.Exception("Unknown animation event for '" + animationName + "'");

		}
	}
}
