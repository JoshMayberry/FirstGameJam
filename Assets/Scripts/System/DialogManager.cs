using System.Collections.Generic;
using UnityEngine;
using Aarthificial.Typewriter;
using Aarthificial.Typewriter.References;
using Aarthificial.Typewriter.Blackboards;

public class DialogManager : MonoBehaviour {
    [Required] [SerializeField] internal Sprite[] chatBubbleSprite;
    [Required] [SerializeField] internal Sprite[] chatButtonSprite;
    [Required] [SerializeField] private ChatBubble chatBubblePrefab;

	[Readonly] [SerializeField] private List<ChatBubble> activeChatBubbles;
	[Readonly] [SerializeField] private List<ChatBubble> inactiveChatBubbles;

	[Readonly] [SerializeField] private ChatBubble currentChat;

	internal IBlackboard globalBlackboard;

    [Readonly] public bool isTalkPressed;
	[Readonly] public bool isPlayerLocked;
	internal bool isChatInProgress => currentChat != null;

	public static DialogManager instance { get; private set; }
	private void Awake() {
		if (instance != null) {
			Debug.LogError("Found more than one Dialog Manager in the scene.");
		}

		instance = this;

		this.activeChatBubbles = new List<ChatBubble>();
		this.inactiveChatBubbles = new List<ChatBubble>();
	}

	private void Start() {
		GameManager.instance.player.typewriterContext.TryGetBlackboard(1388552, out this.globalBlackboard);

		// Wipe the global backboard for this playthrough (will make it so things do not persist)
		// I'm having trouble with the context blackboards- so let's just use the global for everything?
		this.globalBlackboard.Clear();
	}

	private void Update() {
		this.isTalkPressed = Input.GetKeyDown(KeyCode.E);
	}

	public void SetCurrentChat(ChatBubble chatBubble) {
		if (this.currentChat != null) {
			TypewriterDatabase.Instance.RemoveListener(this.currentChat.HandleTypewriterEvent);
		}

		this.currentChat = chatBubble;
		TypewriterDatabase.Instance.AddListener(this.currentChat.HandleTypewriterEvent);
	}

	public void CurrentChatFinished(ChatBubble chatBubble) {
		TypewriterDatabase.Instance.RemoveListener(this.currentChat.HandleTypewriterEvent);
		this.currentChat = null;
		//TypewriterDatabase.Instance.MarkChange(); // Allow icons to reappear
	}

	public void TriggerChat(EntryReference eventReference, Speaker firstSpeaker, bool autoStart = true) {
		ChatBubble chatBubble = this.SpawnChatBubble(firstSpeaker);
		chatBubble.SetEvent(eventReference);

		if (autoStart) {
			chatBubble.TriggerEvent();
		}
	}

	public ChatBubble SpawnChatBubble(Speaker firstSpeaker) {
		ChatBubble chatBubble;

        if (firstSpeaker.personalChatBubble != null) {
            chatBubble = firstSpeaker.personalChatBubble;
        }

        // Try to reuse an inactive chatBubble, or create a new one
        else if (this.inactiveChatBubbles.Count > 0) {
			chatBubble = this.inactiveChatBubbles[this.inactiveChatBubbles.Count - 1];
			this.inactiveChatBubbles.RemoveAt(this.inactiveChatBubbles.Count - 1);

            //chatBubble.DoReset(bubblePosition);
            this.activeChatBubbles.Add(chatBubble);
        }
		else {
			chatBubble = Instantiate(this.chatBubblePrefab, firstSpeaker.chatBubblePosition.position, this.transform.rotation);
		}

		chatBubble.SetInitialState(firstSpeaker);
		chatBubble.gameObject.SetActive(true);

		return chatBubble;
	}

	internal void SetBubbleActive(ChatBubble chatBubble, bool state = true) {
		if (chatBubble.isPersonal) {
			return; // Personal bubbles do not enter the pool of available items
		}

		if (state) {
			this.inactiveChatBubbles.Remove(chatBubble);
			this.activeChatBubbles.Add(chatBubble);
			chatBubble.gameObject.SetActive(true);
			return;
        }

		this.activeChatBubbles.Remove(chatBubble);
		this.inactiveChatBubbles.Add(chatBubble);
		chatBubble.gameObject.SetActive(false);
    }
}
