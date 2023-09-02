using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransition : MonoBehaviour {

    Animator anim;
    string levelName;

    void Start() {
        this.anim = this.GetComponent<Animator>();
    }

    public void ChangeScene(string sceneName) {
        this.levelName = sceneName;
        Debug.Log("@ChangeScene");
        this.anim.SetTrigger("fadeIn");
    }

    // Called by animation
    public void GoToScene() {
        Debug.Log("@GoToScene");
        SceneManager.LoadScene(this.levelName);
    }
}
