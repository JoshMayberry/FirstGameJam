using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	internal LayerMask groundLayer;
	internal Player player;

	public static GameManager instance { get; private set; }
	private void Awake() {
		if (instance != null) {
			Debug.LogError("Found more than one GameManager in the scene.");
		}

		instance = this;

		this.groundLayer = LayerMask.GetMask("Ground");
        this.player = FindAnyObjectByType<Player>();
    }
}
