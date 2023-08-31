using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAespriteController : MonoBehaviour {
    void OnTwistInFinished() {
        GameManager.instance.player.AnimationFinished(AnimationState.TwistIn);
    }
    void OnTwistOutFinished() {
        GameManager.instance.player.AnimationFinished(AnimationState.TwistOut);
    }
    void OnHurtFinished() {
        GameManager.instance.player.AnimationFinished(AnimationState.Hurt);
    }
}
