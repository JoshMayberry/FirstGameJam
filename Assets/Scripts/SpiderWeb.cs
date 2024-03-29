using UnityEngine;

public class SpiderWeb : MonoBehaviour {
    public SpriteRenderer myRenderer;
    public Collider2D myCollider;
    public Collider2D myDetector;
    public MiniMapIcon miniMapIcon;
    public Sprite brokenWebSprite;
  
    void OnTriggerStay2D(Collider2D other) {
        switch(other.tag) {
            case "Player":
                if (GameManager.instance.player.isCutting) {
                    myCollider.enabled = false;
                    myDetector.enabled = false;
                    miniMapIcon.gameObject.SetActive(false);
                    myRenderer.sprite = brokenWebSprite;
                    GameManager.instance.ChangeLevelIntegrity(-1);
                    TypewriterEvents.instance.SetFact(GameManager.instance.player.typewriterContext, TypewriterEvents.instance.fact_isWebCut, 1);
                }
                break;
        }
    }
}
