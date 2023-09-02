using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aarthificial.Typewriter;
using Aarthificial.Typewriter.References;
using Aarthificial.Typewriter.Tools;
using System.Xml;
using Aarthificial.Typewriter.Entries;

public class DialogManager : MonoBehaviour {
    [SerializeField] internal Sprite[] chatBubbleSprite;
    [SerializeField] internal Sprite[] chatButtonSprite;
    //[SerializeField] private ChatBubble chatBubblePrefab;

    //private List<ChatBubble> activeChatBubbles;
    //private List<ChatBubble> inactiveChatBubbles;

    private Dictionary<int, GameObject> speakerDictionary;

	internal bool isTalkPressed { get; private set; }

    public static DialogManager instance { get; private set; }
    private void Awake() {
        if (instance != null) {
            Debug.LogError("Found more than one Dialog Manager in the scene.");
        }

        instance = this;

        //this.BuildSpeakerDictionary();

        //this.activeChatBubbles = new List<ChatBubble>();
        //this.inactiveChatBubbles = new List<ChatBubble>();
    }

    private void Update() {
        this.isTalkPressed = Input.GetKeyDown(KeyCode.E);
    }

    //void BuildSpeakerDictionary() {
    //    speakerDictionary = new Dictionary<int, GameObject>();
    //    foreach (var entry in speakerLookup.entries) {
    //        speakerDictionary.Add(entry.key, entry.value);
    //    }
    //}

    //public GameObject GetSpeaker(EntryReference speakerReference) {
    //    GameObject speaker;
    //    if (speakerDictionary.TryGetValue(speakerReference.ID, out speaker)) {
    //        return speaker;
    //    }

    //    throw new System.Exception("Unknown speaker id '" + speakerReference.ID + "'");
    //}

    //public ChatBubble SpawnChatBubble() {
    //    // Try to reuse an inactive chatBubble, or create a new one
    //    ChatBubble chatBubble;
    //    if (this.inactiveChatBubbles.Count > 0) {
    //        chatBubble = this.inactiveChatBubbles[this.inactiveChatBubbles.Count - 1];
    //        this.inactiveChatBubbles.RemoveAt(this.inactiveChatBubbles.Count - 1);
    //    }
    //    else {
    //        chatBubble = Instantiate(this.chatBubblePrefab, this.transform.position, this.transform.rotation);
    //    }

    //    chatBubble.gameObject.SetActive(true);
    //    this.activeChatBubbles.Add(chatBubble);

    //    return chatBubble;
    //}

    //public void TriggerEvent(EntryReference myEvent, ChatBubble chatBubble = null) {
    //    if (chatBubble == null) {
    //        chatBubble = this.SpawnChatBubble();
    //    }

    //    //chatBubble.TriggerEvent(myEvent);
    //}
}
