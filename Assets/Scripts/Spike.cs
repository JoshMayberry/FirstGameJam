using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour {
    public Collider2D myCollider;

    void OnTriggerEnter2D(Collider2D other) {
        switch (other.tag) {
            case "Player":
                if (GameManager.instance.player.currentForm.id != SlimeFormId.Defend) {
                    GameManager.instance.player.TakeDamage(1);
                }
                break;
        }
    }
}
