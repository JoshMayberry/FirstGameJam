using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour {
	public Collider2D skeletonBlockade;
	public Door door;

	void OnTriggerEnter2D(Collider2D other) {
		switch (other.tag) {
			case "Player":
				skeletonBlockade.enabled = false;
                TypewriterEvents.instance.SetFact(GameManager.instance.player.typewriterContext, TypewriterEvents.instance.fact_isKeyFound, 1);
				this.gameObject.SetActive(false);
				break;
		}
	}
}
