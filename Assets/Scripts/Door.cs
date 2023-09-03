using UnityEngine;

public class Door : MonoBehaviour {
    public SpriteRenderer myRenderer;
    public Collider2D myCollider;
    public Collider2D myDetector;
    public MiniMapIcon miniMapIcon;
    public Sprite openDoor;

    void OnTriggerEnter2D(Collider2D other) {
        switch (other.tag) {
            case "Player":
                if (TypewriterEvents.instance.GetFact(GameManager.instance.player.typewriterContext, TypewriterEvents.instance.fact_isKeyFound) == 1) {
                    myCollider.enabled = false;
                    myDetector.enabled = false;
                    miniMapIcon.gameObject.SetActive(false);
                    myRenderer.sprite = openDoor;
                    GameManager.instance.ChangeLevelIntegrity(-1);
                    TypewriterEvents.instance.SetFact(GameManager.instance.player.typewriterContext, TypewriterEvents.instance.fact_isDoorOpen, 1);
                }
                break;
        }
    }
}
