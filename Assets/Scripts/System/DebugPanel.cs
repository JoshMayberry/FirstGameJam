using Aarthificial.Typewriter;
using Aarthificial.Typewriter.References;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugPanel : MonoBehaviour {
	public EntryReference fact1;
	public EntryReference fact2;
	public EntryReference fact3;
	public EntryReference wouldTrigger1;
	public EntryReference wouldTrigger2;

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
        if (wouldTrigger1 != null) {
            textFact4.text = GameManager.instance.player.typewriterContext.WouldInvoke(wouldTrigger1).ToString();
        }
        if (wouldTrigger2 != null) {
            textFact5.text = GameManager.instance.player.typewriterContext.WouldInvoke(wouldTrigger2).ToString();

            //var provider = GameManager.instance.player.typewriterContext;
            //var entry = wouldTrigger2.GetEntry();
            //Debug.Log("@1: " + entry);
            //Debug.Log("@2: " + entry.Test(provider));
            //Debug.Log("@3: " + entry.ID);
            //Debug.Log("@4: " + provider.HasMatchingRule(entry.ID));

            //var reference = (EntryReference)(entry.ID);
            //Debug.Log("@5: " + reference);
            //Debug.Log("@6: " + !reference.TryGetEntry(out var entry2));

            //for (var i = 0; i < entry.Entries.Count; i++) {
            //    var response = entry.Entries[i];
            //    Debug.Log("@7." + i + ": " + response + "; " + response.Test(provider));
            //}
        }
    }
}
