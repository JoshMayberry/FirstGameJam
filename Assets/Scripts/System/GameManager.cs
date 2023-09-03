using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

internal enum GameOverMethod {
	LevelIntegrity, ZeroHealth, FinishedGame
}

public class GameManager : MonoBehaviour {

	internal LayerMask groundLayer;
	internal Player player;
	internal Speaker system;
	internal Scene currentScene;

	[SerializeField] private Slider levelIntegrityBar;

	[SerializeField] private GameObject gameOver_levelIntegrity;
	[SerializeField] private GameObject gameOver_hpZero;
	[SerializeField] private GameObject gameWon;
    [SerializeField] private LevelTransition levelTransition;

	internal List<Gate> gateRegistry;

	private float maxLevelIntegrity;
	internal bool isGameOver;

	public static GameManager instance { get; private set; }
	private void Awake() {
		this.levelTransition.gameObject.SetActive(true);

		if (instance != null) {
			Debug.LogError("Found more than one GameManager in the scene.");
		}

		instance = this;

		this.currentScene = SceneManager.GetActiveScene();
		this.groundLayer = LayerMask.GetMask("Ground");
		this.player = FindAnyObjectByType<Player>();

		var temp = player.transform.Find("SubSpeaker").gameObject;
		this.system = temp.GetComponent<Speaker>();

        this.maxLevelIntegrity = this.levelIntegrityBar.maxValue;
		this.gateRegistry = new List<Gate>();
	}

	public void Start() {
		this.isGameOver = false;
	}

	public void ChangeLevelIntegrity(int amount) {
		this.levelIntegrityBar.value = Mathf.Max(Mathf.Min(this.levelIntegrityBar.value + amount, this.maxLevelIntegrity), 0);
    }

    public void OnLevelIntegrityChanged() {
        if (this.levelIntegrityBar.value <= 0) {
            this.TriggerGameOver(GameOverMethod.LevelIntegrity);
        }
    }

    internal void TriggerGameOver(GameOverMethod how) {
        this.isGameOver = true;

        this.player.rb.AddForce(-this.player.rb.velocity, ForceMode2D.Impulse); // Remove all momentum

        switch (how) {
            case GameOverMethod.LevelIntegrity:
                this.gameOver_levelIntegrity.SetActive(true);
                break;

            case GameOverMethod.ZeroHealth:
                this.gameOver_hpZero.SetActive(true);
                break;

            case GameOverMethod.FinishedGame:
                this.gameWon.SetActive(true);
                break;

            default:
                throw new System.Exception("Unknown game over method '" + how + "'");

        }
    }

    public void ChangeScene(string sceneName) {
		levelTransition.ChangeScene(sceneName);
	}

	public void Restart() {
		this.ChangeScene(this.currentScene.name);
	}

	internal void ToggleGates(bool state) {
		foreach (Gate gate in this.gateRegistry) {
			gate.ToggleSlipThrough(state);
		}
	}
}
