using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teddy : MonoBehaviour {
    void OnTriggerEnter2D(Collider2D other) {
        switch (other.tag) {
            case "Player":
                GameManager.instance.TriggerGameOver(GameOverMethod.FinishedGame);
                break;
        }
    }
}
