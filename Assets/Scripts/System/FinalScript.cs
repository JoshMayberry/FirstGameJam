using UnityEngine;

public class FinalScript : MonoBehaviour {

	void Start() {
        DialogManager.instance.TriggerChat(TypewriterEvents.instance.event_onLevelStart, GameManager.instance.system, true);
    }
}
