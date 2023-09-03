using System.Collections.Generic;
using UnityEngine;
using System;
using Aarthificial.Typewriter;
using Aarthificial.Typewriter.References;
using Aarthificial.Typewriter.Blackboards;
using Aarthificial.Typewriter.Entries;

public enum SpeakerType {
    Skeleton = 0,
    Slime = 1,
	System = 2
}

public enum ChatBubbleAlignment {
    TopLeft,
    TopMiddle,
    TopRight,
    BottomLeft,
    BottomMiddle,
    BottomRight,
    Left,
    Right,
    Center
}

public class TypewriterEvents : MonoBehaviour {
	[SerializeField] internal Dictionary<int, Speaker> speakerLookup;

    [field: Header("Speakers")]
    [field: SerializeField] public EntryReference speaker_nothing { get; private set; }

    [field: Header("Facts")]
	[field: SerializeField] public EntryReference fact_nearSkeleton1 { get; private set; }
	[field: SerializeField] public EntryReference fact_moveCount { get; private set; }
    [field: SerializeField] public EntryReference fact_twistCount { get; private set; }
    [field: SerializeField] public EntryReference fact_actCount { get; private set; }
    [field: SerializeField] public EntryReference fact_isWebCut { get; private set; }
    [field: SerializeField] public EntryReference fact_isKeyFound { get; private set; }
    [field: SerializeField] public EntryReference fact_isDoorOpen { get; private set; }
    [field: SerializeField] public EntryReference fact_playerHealth { get; private set; }

    [field: Header("Events")]
	[field: SerializeField] public EntryReference event_nothing { get; private set; }
	[field: SerializeField] public EntryReference event_onLevelStart { get; private set; }
    [field: SerializeField] public EntryReference event_onTellPlayerToMove { get; private set; }

	//[field: Header("Triggers")]
	//[field: SerializeField] public EntryReference trigger_updateNearSkeleton1_true { get; private set; }
	//[field: SerializeField] public EntryReference trigger_updateNearSkeleton1_false { get; private set; }
	//[field: SerializeField] public EntryReference trigger_updateMoveCount { get; private set; }
	//[field: SerializeField] public EntryReference trigger_updateTwistCount { get; private set; }

	[SerializeField] private float throttle_updateMoveCount = 1f;
	private float lastUpdateMoveCount;

	public static TypewriterEvents instance { get; private set; }
	private void Awake() {
		if (instance != null) {
			Debug.LogError("Found more than one FMOD Events in the scene.");
		}

		instance = this;
		this.speakerLookup = new Dictionary<int, Speaker>();
    }

	private void Update() {
		this.lastUpdateMoveCount -= Time.deltaTime;
	}

	//public void UpdateFact(FactBoolean factName, bool state = true) {
 //       EntryReference factReference;
	//	switch (factName) {
	//		case FactBoolean.NearSkeleton1:
	//			factReference = (state ? this.trigger_updateNearSkeleton1_true : this.trigger_updateNearSkeleton1_false);
	//			break;

	//		default:
	//			throw new System.Exception("Unknown boolean fact '" + factName + "'");
	//	}

	//	GameManager.instance.player.typewriterContext.TryInvoke(factReference);
	//}

	//public void UpdateFact(FactInteger factName, bool increment = true) {
	//	EntryReference factReference;
	//	switch (factName) {
	//		case FactInteger.MoveCount:
	//			if (this.lastUpdateMoveCount > 0.01f) {
	//				return;
	//			}

	//			this.lastUpdateMoveCount = this.throttle_updateMoveCount;
	//			factReference = this.trigger_updateMoveCount;
	//			break;

	//		case FactInteger.TwistCount:
	//			factReference = this.trigger_updateTwistCount;
	//			break;

	//		default:
	//			throw new System.Exception("Unknown integer fact '" + factName + "'");
	//	}

	//	GameManager.instance.player.typewriterContext.TryInvoke(factReference);
 //   }

    public int GetFact(MyContext context, EntryReference factReference) {
        factReference.TryGetEntry(out FactEntry factEntry);
        return context.Get(factEntry);
    }

    public void SetFact(MyContext context, EntryReference factReference, int value, bool isSilent=false) {
        factReference.TryGetEntry(out FactEntry factEntry);
        context.Set(factEntry, value);

		if (!isSilent) {
			TypewriterDatabase.Instance.MarkChange();
		}
    }
    public void IncrementFact(MyContext context, EntryReference factReference, int value, bool isSilent = false) {
		if (factReference.ID == this.fact_moveCount.ID) {
            if (this.lastUpdateMoveCount > 0.01f) {
                return;
            }

            this.lastUpdateMoveCount = this.throttle_updateMoveCount;
        }

		factReference.TryGetEntry(out FactEntry factEntry);
        context.Add(factEntry, value);

        if (!isSilent) {
            TypewriterDatabase.Instance.MarkChange();
        }
    }

    public Speaker LookupSpeaker(DialogueEntry currentEntry) {
		return this.speakerLookup[currentEntry.Speaker.ID];
	}
}


/// <summary>
/// A sample implementation of <see cref="ITypewriterContext"/>.
/// </summary>
/// <remarks>
/// In this example, the context is made up of two blackboards. The global
/// blackboard is shared between all contexts, while the context blackboard is
/// local to the given conversation.
/// </remarks>
[Serializable]
public class MyContext: ITypewriterContext {
	private const int _globalScope = 1388552;
	private const int _contextScope = 1388553;

	private static readonly Blackboard _global = new();
	private readonly Blackboard _context = new();

	public bool TryGetBlackboard(int scope, out IBlackboard blackboard) {
		switch (scope) {
			case _globalScope:
				blackboard = _global;
				return true;

			case _contextScope:
				blackboard = this._context;
				return true;

			default:
				blackboard = default;
				return false;
		}
	}
}