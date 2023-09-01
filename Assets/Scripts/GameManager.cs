using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class GameManager : MonoBehaviour {

	internal LayerMask groundLayer;
	internal Player player;

	[SerializeField] private EventReference gameMusic;

    public static GameManager instance { get; private set; }
	private void Awake() {
		if (instance != null) {
            UnityEngine.Debug.LogError("Found more than one GameManager in the scene.");
		}

		instance = this;

		this.groundLayer = LayerMask.GetMask("Ground");
        this.player = FindAnyObjectByType<Player>();
    }

    public void Start() {
        // TODO: let's transition between songs using a collider- we can handle that logic in FMOD using parameters
        RuntimeManager.PlayOneShot(this.gameMusic, this.gameObject.transform.position);
    }

    public void PlayOneShot(EventReference sound, Vector3? worldPosition=null) {
        RuntimeManager.PlayOneShot(sound, (worldPosition.HasValue ? worldPosition.Value : this.gameObject.transform.position));
    }
}
