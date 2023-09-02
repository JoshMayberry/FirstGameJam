using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

internal enum GameOverMethod {
    LevelIntegrity, ZeroHealth
}

public class GameManager : MonoBehaviour {

	internal LayerMask groundLayer;
	internal Player player;
    internal Scene currentScene;

	[SerializeField] private Slider levelIntegrityBar;

    [SerializeField] private GameObject gameOver_levelIntegrity;
    [SerializeField] private GameObject gameOver_hpZero;
    [SerializeField] private LevelTransition levelTransition;

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

        this.maxLevelIntegrity = this.levelIntegrityBar.maxValue;
    }

    public void Start() {
        this.isGameOver = false;
        DialogManager.instance.TriggerChat(TypewriterEvents.instance.event_onLevelStart, this.player, true);
    }

    public void ChangeLevelIntegrity(int amount) {
        levelIntegrityBar.value = Mathf.Max(Mathf.Min(levelIntegrityBar.value + amount, this.maxLevelIntegrity), 0);

    }

    public void OnLevelIntegrityChanged() {
        if (levelIntegrityBar.value <= 0) {
            this.TriggerGameOver(GameOverMethod.LevelIntegrity);
        }
    }

    internal void TriggerGameOver(GameOverMethod how) {
        this.isGameOver = true;
        switch (how) {
            case GameOverMethod.LevelIntegrity:
                this.gameOver_levelIntegrity.SetActive(true);
                break;

            case GameOverMethod.ZeroHealth:
                this.gameOver_hpZero.SetActive(true);
                break;

            default:
                throw new System.Exception("Unknown game over method '" + how + "'");

        }
    }

    public void ChangeScene(string sceneName) {
        levelTransition.ChangeScene(sceneName);
    }

    public void Restart() {
        Debug.Log("@Restart");
        this.ChangeScene(this.currentScene.name);
    }
}
