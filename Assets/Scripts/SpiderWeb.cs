using UnityEngine;

public class SpiderWeb : MonoBehaviour {
    public SpriteRenderer myRenderer;
    public Collider2D myCollider;
    public Collider2D myDetector;
    public MiniMapIcon miniMapIcon;
    public Sprite brokenWebSprite;
  
    void OnTriggerEnter2D(Collider2D other) {
        switch(other.tag) {
            case "Player":
                if (GameManager.instance.player.isCutting) {
                    myCollider.enabled = false;
                    myDetector.enabled = false;
                    miniMapIcon.gameObject.SetActive(false);
                    myRenderer.sprite = brokenWebSprite;
                    GameManager.instance.ChangeLevelIntegrity(-1);
                }
                break;
        }
    }
}
