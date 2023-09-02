using Aarthificial.Typewriter.References;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugPanel : MonoBehaviour {
	public EntryReference fact1;
	public EntryReference fact2;
	public EntryReference fact3;
	public EntryReference fact4;
	public EntryReference fact5;

    public TextMeshProUGUI textFact1;
	public TextMeshProUGUI textFact2;
	public TextMeshProUGUI textFact3;
	public TextMeshProUGUI textFact4;
	public TextMeshProUGUI textFact5;

    void Update() {
        if (fact1 != null) {
            textFact1.text = TypewriterEvents.instance.GetFact(GameManager.instance.player.typewriterContext, fact1).ToString();
        }
        if (fact2 != null) {
            textFact2.text = TypewriterEvents.instance.GetFact(GameManager.instance.player.typewriterContext, fact2).ToString();
        }
        if (fact3 != null) {
            textFact3.text = TypewriterEvents.instance.GetFact(GameManager.instance.player.typewriterContext, fact3).ToString();
        }
        if (fact4 != null) {
            textFact4.text = TypewriterEvents.instance.GetFact(GameManager.instance.player.typewriterContext, fact4).ToString();
        }
        if (fact5 != null) {
            textFact5.text = TypewriterEvents.instance.GetFact(GameManager.instance.player.typewriterContext, fact5).ToString();
        }
    }
}
