using UnityEngine;

public class PlayerAespriteController : MonoBehaviour {
    void OnMove() {
		GameManager.instance.player.AnimationStart(AnimationState.MoveUp);
    }

    void OnHurt() {
        GameManager.instance.player.AnimationStart(AnimationState.Hurt);
    }

    void OnTwistIn() {
        GameManager.instance.player.AnimationStart(AnimationState.TwistIn);
    }

    void OnActionStarting() {
        GameManager.instance.player.AnimationStart(AnimationState.ActionStarting);
    }

    void OnActionLooping() {
        GameManager.instance.player.AnimationStart(AnimationState.ActionLooping);
    }

    void OnActionEnding() {
        GameManager.instance.player.AnimationStart(AnimationState.ActionEnding);
    }

    void OnTwistInFinished() {
        GameManager.instance.player.AnimationFinished(AnimationState.TwistIn);
    }
    void OnTwistOutFinished() {
        GameManager.instance.player.AnimationFinished(AnimationState.TwistOut);
    }
    void OnHurtFinished() {
        GameManager.instance.player.AnimationFinished(AnimationState.Hurt);
    }

    void OnActionStartingFinished() {
        GameManager.instance.player.AnimationFinished(AnimationState.ActionStarting);
    }

    void OnActionLoopingFinished() {
        GameManager.instance.player.AnimationFinished(AnimationState.ActionLooping);
    }

    void OnActionEndingFinished() {
        GameManager.instance.player.AnimationFinished(AnimationState.ActionEnding);
    }
}
