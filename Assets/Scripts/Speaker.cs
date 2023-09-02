using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Aarthificial.Typewriter.References;

public class Speaker : MonoBehaviour {
	[SerializeField] internal Transform chatBubblePosition;
	[SerializeField] internal ChatBubbleAlignment chatBubbleAlignment = ChatBubbleAlignment.TopMiddle;
	[SerializeField] internal SpeakerType speakerType = SpeakerType.Skeleton;

	[SerializeField] private EntryReference speakerReference;

	internal MyContext typewriterContext = new MyContext();

	internal void SpawnChatBubble(string text) {
		//DialogManager.instance.SpawnChatBubble(this, text);
	}
    void OnEnable() {
		if (this.speakerReference == null) {
			return;
		}
        TypewriterEvents.instance.speakerLookup.Speakers.Add(this.speakerReference.ID, this);
    }

    void OnDisable() {
		if (this.speakerReference == null) {
			return;
		}
        TypewriterEvents.instance.speakerLookup.Speakers.Remove(this.speakerReference.ID);
    }
}
