using UnityEngine;

public class RustWall : Destructible {
  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start() {
    health = maxHealth;
  }

  // Update is called once per frame
  void Update() {

  }

  protected override void DamageEffect() {
    
  }

  protected override void Death() {
    Destroy(gameObject);
  }
}
