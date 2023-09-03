using UnityEngine;

public class Gate : MonoBehaviour {

    Collider2D myCollider;

    private void Awake() {
        this.myCollider = GetComponent<Collider2D>();
    }

    private void OnEnable() {
        GameManager.instance.gateRegistry.Add(this);
    }

    private void OnDisable() {
        GameManager.instance.gateRegistry.Remove(this);
    }

    internal void ToggleSlipThrough(bool state) {
        this.myCollider.enabled = !state;
    }
}
