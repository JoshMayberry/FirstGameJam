using UnityEngine;
using Aarthificial.Typewriter.References;

public class Speaker : MonoBehaviour {
	[Header("Speaker")]
    [Required] [SerializeField] internal Transform chatBubblePosition;
    [Required] [SerializeField] internal ChatBubbleAlignment chatBubbleAlignment = ChatBubbleAlignment.TopMiddle;
    [Required] [SerializeField] internal SpeakerType speakerType = SpeakerType.Skeleton;

	[SerializeField] private EntryReference speakerReference;
    [SerializeField] internal ChatBubble personalChatBubble;

    [SerializeField] internal string displayName;

    internal MyContext typewriterContext = new MyContext();

    void OnEnable() {
		if (this.speakerReference == TypewriterEvents.instance.speaker_nothing) {
			return;
		}
        TypewriterEvents.instance.speakerLookup.Add(this.speakerReference.ID, this);
    }

    void OnDisable() {
		if (this.speakerReference == TypewriterEvents.instance.speaker_nothing) {
			return;
		}
        TypewriterEvents.instance.speakerLookup.Remove(this.speakerReference.ID);
    }
}
