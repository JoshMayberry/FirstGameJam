using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Aarthificial.Typewriter;
using Aarthificial.Typewriter.References;
using Aarthificial.Typewriter.Tools;
using UnityEditor.PackageManager;
using Aarthificial.Typewriter.Entries;
using static UnityEditor.Timeline.TimelinePlaybackControls;
using UnityEngine.InputSystem.LowLevel;

// Use: https://www.youtube.com/watch?v=K13WnNL1OYM
public class ChatBubble : MonoBehaviour {
	private SpriteRenderer iconSpriteRenderer;
	private SpriteRenderer backgroundSpriteRenderer;
	private TextMeshPro dialogText;
	private TextMeshPro speakerText;
	private Collider2D factCollider;
	private GameObject container;

	public Vector2 padding = new Vector2(2f, 2f);

	[SerializeField] private EntryReference currentEvent;
	private MyContext typewriterContext = new MyContext();
	private TypewriterWatcher typewriterWatcher;

	public bool isColliderEnabled;
	public EntryReference factColliderUpdates;

	[SerializeField] private float charsPerSecond = 32f;

	private float startTime;
	private DialogueEntry currentEntry;
	private ITypewriterContext currentContext;
	private Speaker currentSpeaker;
	private bool isActive;
	private bool isTextCompleted;
	private bool isLastDialog;
	private Vector3 initialPosition;


	private void Awake() {
		this.container = this.transform.Find("Contents").gameObject;
		this.backgroundSpriteRenderer = this.container.transform.Find("Background").GetComponent<SpriteRenderer>();
		this.iconSpriteRenderer = this.container.transform.Find("Icon").GetComponent<SpriteRenderer>();
		this.speakerText = this.container.transform.Find("SpeakerName").GetComponent<TextMeshPro>();
		this.dialogText = this.container.transform.Find("Text").GetComponent<TextMeshPro>();
		this.factCollider = this.GetComponent<Collider2D>();

		this.factCollider.enabled = this.isColliderEnabled;
	}

	private void Start() {
		this.container.transform.localPosition = Vector3.zero;
		DialogManager.instance.SetBubbleActive(this);
	}

	private void Update() {
		if (this.isTextCompleted) {
            if (!this.isActive) {
				return;
            }

            if (this.isLastDialog) {
				if (DialogManager.instance.isTalkPressed) {
					DialogManager.instance.isTalkPressed = false;
					this.Finish();
                }

                return;
            }

            if (this.currentEntry == null) {
				return;
			}

			if (DialogManager.instance.isTalkPressed) {
				DialogManager.instance.isTalkPressed = false;
				this.typewriterContext.TryInvoke(this.currentEntry);
				return;
            }

			// Account for canceling
			if (!this.typewriterContext.WouldInvoke(this.currentEntry)) {
				this.Cancel();
            }
			return;
        }

		if (!this.isActive) {
			// Account for another chat being in progress
			if (DialogManager.instance.isChatInProgress) {
				this.UpdateIcon(false);
				return;
			}

			if ((this.currentEvent != null) && this.typewriterWatcher.ShouldUpdate()) {
				this.UpdateIcon(this.typewriterContext.WouldInvoke(this.currentEvent));
			}

			if (this.iconSpriteRenderer.gameObject.activeSelf && DialogManager.instance.isTalkPressed) {
				this.TriggerEvent();
			}
			return;
		}

		if (DialogManager.instance.isTalkPressed) {
			DialogManager.instance.isTalkPressed = false;
			if (!this.isTextCompleted) {
                this.Proceed();
				return;
			}
			this.Finish();
			return;
		}

		var timeElapsed = Time.time - startTime;
		var duration = this.currentEntry.Text.Length / (this.charsPerSecond * this.currentEntry.Speed);
		var progress = Mathf.Clamp01(timeElapsed / duration);

		if (progress >= 1) {
			this.Proceed();
			return;
		}
			
		var textLength = Mathf.Max(0, Mathf.FloorToInt(currentEntry.Text.Length * progress));

		// WARNING: This is a very naive approach to revealing the text. I used it
		// so that you don't have to install `TextMeshPro` for this example.
		// In a real game, consider using something like `maxVisibleCharacters`:
		// https://docs.unity3d.com/Packages/com.unity.textmeshpro@4.0/api/TMPro.TMP_Text.html#TMPro_TMP_Text_maxVisibleCharacters
		this.SetContents(currentEntry.Text[..textLength]);
	}

	private void UpdateIcon(bool showIcon) {
		this.iconSpriteRenderer.gameObject.SetActive(showIcon);
	}

	private void UpdateSpeaker(Speaker newSpeaker = null) {
		string speakerName = "";
		if (newSpeaker == null) {
			speakerName = currentEntry.Speaker.DisplayName;
			newSpeaker = TypewriterEvents.instance.LookupSpeaker(currentEntry);
		}

		if (speakerName == "") {
			speakerName = newSpeaker.displayName;

			if ((speakerName == "") || (speakerName == null)) {
				speakerName = "Unknown";
			}
		}

		this.speakerText.text = speakerName;
		this.speakerText.ForceMeshUpdate(); // Ensure text renders this frame so we can get the size

		if (this.currentSpeaker?.speakerType != newSpeaker.speakerType) {
			this.backgroundSpriteRenderer.sprite = DialogManager.instance.chatBubbleSprite[(int)newSpeaker.speakerType];
			this.iconSpriteRenderer.sprite = DialogManager.instance.chatButtonSprite[(int)newSpeaker.speakerType];
		}

		this.currentSpeaker = newSpeaker;
	}

	internal void UpdatePosition() {
		Vector2 upscaledSize = this.backgroundSpriteRenderer.size * this.backgroundSpriteRenderer.transform.localScale;

		Vector2 newPosition = this.currentSpeaker.chatBubblePosition.position;
		switch (this.currentSpeaker.chatBubbleAlignment) {
			case ChatBubbleAlignment.TopLeft:
				newPosition += new Vector2(-upscaledSize.x / 2, upscaledSize.y / 2);
				break;
			case ChatBubbleAlignment.TopMiddle:
				newPosition += new Vector2(0, upscaledSize.y / 2);
				break;
			case ChatBubbleAlignment.TopRight:
				newPosition += new Vector2(upscaledSize.x / 2, upscaledSize.y / 2);
				break;
			case ChatBubbleAlignment.BottomLeft:
				newPosition += new Vector2(-upscaledSize.x / 2, -upscaledSize.y / 2);
				break;
			case ChatBubbleAlignment.BottomMiddle:
				newPosition += new Vector2(0, -upscaledSize.y / 2);
				break;
			case ChatBubbleAlignment.BottomRight:
				newPosition += new Vector2(upscaledSize.x / 2, -upscaledSize.y / 2);
				break;
			case ChatBubbleAlignment.Left:
				newPosition += new Vector2(-upscaledSize.x / 2, 0);
				break;
			case ChatBubbleAlignment.Right:
				newPosition += new Vector2(upscaledSize.x / 2, 0);
				break;
			case ChatBubbleAlignment.Center:
				// no need to modify the position
				break;
		}

		this.container.transform.position = newPosition;

		// Move icon to bottom right corner
		Vector2 iconSize = this.iconSpriteRenderer.size;
		Vector2 iconPosition = (Vector2)this.container.transform.position + new Vector2(upscaledSize.x / 2 - iconSize.x / 2, -upscaledSize.y / 2 + iconSize.y / 2);
		this.iconSpriteRenderer.transform.position = iconPosition;

		// Move name to top left corner
		Vector2 namePosition = (Vector2)this.container.transform.position + new Vector2(-upscaledSize.x / 2 + 1, upscaledSize.y / 2);
		this.speakerText.gameObject.transform.position = namePosition;
	}

	private void SetContents(string text) {
		this.dialogText.text = text;
		this.dialogText.ForceMeshUpdate(); // Ensure text renders this frame so we can get the size
		Vector2 textSize = this.dialogText.GetRenderedValues(false);
		Vector2 targetSize = (textSize + this.padding) / this.backgroundSpriteRenderer.transform.localScale;
		Vector2 clampedSize = new Vector2(Mathf.Max(0.36f, targetSize.x), Mathf.Max(0.32f, targetSize.y));

		// Ensure the sprite does not get squished
		this.backgroundSpriteRenderer.size = clampedSize;

		this.UpdatePosition();
	}

	public void SetInitialState(Speaker initialSpeaker) {
		this.UpdateSpeaker(initialSpeaker);
		this.transform.position = initialSpeaker.chatBubblePosition.transform.position;
	}

	public void SetEvent(EntryReference myEvent, Transform newPosition = null) {
		this.DoReset(newPosition);
		this.currentEvent = myEvent;
	}

	public void DoReset(Transform newPosition = null) {
		if (newPosition != null) {
			this.transform.position = newPosition.position;
		}

		this.currentEntry = null;
		this.currentContext = null;
		this.currentSpeaker = null;
		this.isTextCompleted = false;
		this.isLastDialog = false;
		this.container.transform.localPosition = Vector3.zero;
		this.iconSpriteRenderer.gameObject.transform.localPosition = Vector3.zero;
		this.iconSpriteRenderer.gameObject.SetActive(true);
		this.backgroundSpriteRenderer.gameObject.SetActive(false);
		this.speakerText.gameObject.SetActive(false);
		this.dialogText.gameObject.SetActive(false);
		this.factCollider.enabled = this.isColliderEnabled;
	}

	internal void HandleTypewriterEvent(BaseEntry entry, ITypewriterContext context) {
		//if (this.isActive) {
		//	return;
		//}

		if (entry is DialogueEntry textEntry) {
			this.currentEntry = textEntry;
			this.currentContext = context;
			this.Begin();
			return;
		}

		if (entry is EventEntry eventEntry) {
			context.TryInvoke(eventEntry);
			return;
		}

		if (entry is DecisionEntry decisionEntry) {
			context.TryInvoke(decisionEntry);
			return;
		}

		if (entry is TriggerEntry triggerEntry) {
			context.TryInvoke(triggerEntry);
			return;
		}

		throw new System.Exception("Unknown enrty type for '" + entry + "'");
	}

	private void Begin() {
		DialogManager.instance.isPlayerLocked = this.currentEntry.IsPlayerLocked;
		this.isActive = true;
		this.startTime = Time.time;
		this.isTextCompleted = false;
		this.isLastDialog = !currentContext.HasMatchingRule(currentEntry.ID);

        this.backgroundSpriteRenderer.gameObject.SetActive(true);
		this.speakerText.gameObject.SetActive(true);
		this.dialogText.gameObject.SetActive(true);

		this.UpdateSpeaker();
	}

	private void Proceed() {
		this.isTextCompleted = true;
		this.SetContents(this.currentEntry.Text);
	}

	private void Finish(DialogueEntry next = null) {
        DialogManager.instance.isPlayerLocked = false;
		var entry = this.currentEntry;
		var context = this.currentContext;

		if (next != null) {
			entry.Apply(context);
			next.Invoke(context);
			entry = next;
		}

		this.currentEntry = null;
		this.currentContext = null;
		context.Process(entry);

		if (this.currentEntry == null) {
			this.FinishForGood();
        }
	}

	private void Cancel() {
		DialogManager.instance.isPlayerLocked = false;
		DialogManager.instance.CurrentChatFinished(this);
		this.isActive = false;
		this.isTextCompleted = false;
        this.isLastDialog = false;
		this.container.transform.localPosition = Vector3.zero;
        this.iconSpriteRenderer.gameObject.transform.localPosition = Vector3.zero;
		this.iconSpriteRenderer.gameObject.SetActive(true);

		this.backgroundSpriteRenderer.gameObject.SetActive(false);
		this.speakerText.gameObject.SetActive(false);
		this.dialogText.gameObject.SetActive(false);
	}

	private void FinishForGood() {
		DialogManager.instance.CurrentChatFinished(this);
        this.isActive = false;
		this.isTextCompleted = true;
        this.isLastDialog = false;
		this.backgroundSpriteRenderer.gameObject.SetActive(false);
        this.iconSpriteRenderer.gameObject.SetActive(false);
		this.speakerText.gameObject.SetActive(false);
		this.dialogText.gameObject.SetActive(false);
		DialogManager.instance.SetBubbleActive(this, false);
	}

	public void TriggerEvent() {
		DialogManager.instance.SetCurrentChat(this);
		this.typewriterContext.TryInvoke(this.currentEvent);
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.tag == "Player") {
			TypewriterEvents.instance.SetFact(this.typewriterContext, this.factColliderUpdates, 1);
		}
	}

	void OnTriggerExit2D(Collider2D other) {
		if (other.tag == "Player") {
			TypewriterEvents.instance.SetFact(this.typewriterContext, this.factColliderUpdates, 0);
		}
	}
}
