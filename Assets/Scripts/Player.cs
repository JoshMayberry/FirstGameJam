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
	internal const string ActionStarting = "action_starting";
	internal const string ActionLooping = "action_looping";
	internal const string ActionEnding = "action_ending";
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
    [Required] [SerializeField] private Animator normalAnimator;
	[Required] [SerializeField] private Animator healAnimator;
	[Required] [SerializeField] private Animator defendAnimator;
	[Required] [SerializeField] private Animator attackAnimator;
	[Readonly] public string currentState = AnimationState.Idle;
	[Readonly] public bool isTwisting;
	[Readonly] public bool isActing;
    [Readonly] public bool isAnimatingOneshot;

	[Header("Physics")]
	[SerializeField] private Transform attackCheckPosition;
	private Rigidbody2D rb;

	[Header("Input")]
	[SerializeField] private float doActionGraceTime = 0.1f;
	private Vector2 inputMove;
	private float lastDoActionPressed;
    [Readonly] public bool isDoActionPressed;
    [Readonly] public bool isActingLoopOver;

    [Header("Actions")]
    [SerializeField] private float applyActionGraceTime = 0.25f;
    private float lastActionSlam;
    [Readonly] public bool isSkinny;
    [Readonly] public bool isCutting;
    [Readonly] public bool isSlamming;
    [Readonly] public bool isSlamActive;

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

    void Update() {
		if (GameManager.instance.isGameOver || DialogManager.instance.isPlayerLocked) {
			return;
		}

		// Update Timers
		this.lastDoActionPressed -= Time.deltaTime;
		this.lastActionSlam -= Time.deltaTime;

        // Update States
        this.isDoActionPressed = (this.lastDoActionPressed > 0.01f);
        this.isSlamActive = (this.lastActionSlam > 0.01f);

        this.UpdateAnimations();
	}

	private void FixedUpdate() {
		if (GameManager.instance.isGameOver || DialogManager.instance.isPlayerLocked || this.isSlamming) {
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

    void OnDoActionPress() {
		this.isActingLoopOver = false;

		if (!this.isActing) {
			this.lastDoActionPressed = this.doActionGraceTime;
		}
    }

    void OnDoActionRelease() {
		this.isActingLoopOver = true;
    }

    void UpdateAnimations() {
        if (this.isActing) {
			return;
		}

		if (this.isDoActionPressed) {
            this.isDoActionPressed = false;
			this.lastDoActionPressed = 0f;

			SetAnimation(AnimationState.ActionStarting);
            this.isAnimatingOneshot = true;
			this.isActing = true;
        }
        else if ((this.inputMove.x == 0) && (this.inputMove.y == 0)) {
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

	void UpdateMovement() {
		Vector2 speed_target = new Vector2(this.inputMove.x * this.currentForm.maxSpeed, this.inputMove.y * this.currentForm.maxSpeed);
		Vector2 speed_delta = speed_target - this.rb.velocity;
		
		bool is_accelerating = ((Mathf.Abs(speed_target.x) > 0.01f) || (Mathf.Abs(speed_target.y) > 0.01f));
		float accelerationRate = (is_accelerating ? this.currentForm.accelerationRate : this.currentForm.deccelerationRate);
		Vector2 velocity_force = speed_delta * accelerationRate;

		this.rb.AddForce(velocity_force, ForceMode2D.Force);
	}

	void SetForm(string newForm) {
		if ((GameManager.instance.isGameOver) || DialogManager.instance.isPlayerLocked || this.isTwisting || this.isActing || (newForm == this.currentForm.key)) {
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

    internal void AnimationStart(string animationName) {
        switch (animationName) {
            case AnimationState.MoveUp:
            case AnimationState.MoveDown:
            case AnimationState.MoveLeft:
            case AnimationState.MoveRight:
                AudioManager.instance.PlayOneShot("moveSound", FmodEvents.instance.moveSound, this.gameObject.transform.position);
                break;

            case AnimationState.TwistIn:
                AudioManager.instance.PlayOneShot("twistSound", FmodEvents.instance.twistSound, this.gameObject.transform.position);
                break;

            case AnimationState.Hurt:
                AudioManager.instance.PlayOneShot("hurtSound", FmodEvents.instance.hurtSound, this.gameObject.transform.position);
                break;

            case AnimationState.ActionStarting:
				switch (this.currentForm.id) {
					case SlimeFormId.Normal:
						this.isActing = false;
						this.isAnimatingOneshot = false;
						break;

					case SlimeFormId.Defend:
						this.isSlamming = true;
                        break;
				}

                break;

			case AnimationState.ActionEnding:
				switch (this.currentForm.id) {
					case SlimeFormId.Heal:
						GameManager.instance.ToggleGates(false);
						this.isSkinny = false;
						break;

                    case SlimeFormId.Defend:
                        this.rb.AddForce(-this.rb.velocity, ForceMode2D.Impulse); // Remove all momentum
                        break;
                }
                break;

            default:
                throw new System.Exception("Unknown animation start event for '" + animationName + "'");

        }
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

            case AnimationState.ActionStarting:
				switch (this.currentForm.id) {
					case SlimeFormId.Heal:
						this.isSkinny = true;
						GameManager.instance.ToggleGates(true);
                        SetAnimation(AnimationState.ActionLooping, true);
                        break;

					case SlimeFormId.Defend:
                        this.lastActionSlam = this.applyActionGraceTime;
                        SetAnimation(AnimationState.ActionEnding, true);
                        break;

					case SlimeFormId.Attack:
						this.isCutting = true;
                        SetAnimation(AnimationState.ActionLooping, true);
                        break;

					default:
						throw new System.Exception("Unknown slime form '" + this.currentForm.id + "'");
				}
				break;

            case AnimationState.ActionEnding:
				switch (this.currentForm.id) {
                    case SlimeFormId.Defend:
                        this.isSlamming = false;
                        break;

                    case SlimeFormId.Attack:
                        this.isCutting = false;
                        break;
                }

                this.isAnimatingOneshot = false;
                this.isActing = false;
                TypewriterEvents.instance.IncrementFact(this.typewriterContext, TypewriterEvents.instance.fact_actCount, 1);
                break;

			case AnimationState.ActionLooping:
                switch (this.currentForm.id) {
                    case SlimeFormId.Heal:
                    case SlimeFormId.Attack:
						if (this.isActingLoopOver) {
                            SetAnimation(AnimationState.ActionEnding, true);
							break;
                        }

						// Play the animation again
                        //this.currentForm.animator.CrossFade(AnimationState.ActionLooping, 0, 0);
                        break;

                    default:
                        throw new System.Exception("Unknown slime form '" + this.currentForm.id + "'");
                }
				break;

            default:
				throw new System.Exception("Unknown animation finish event for '" + animationName + "'");

		}
    }
}
