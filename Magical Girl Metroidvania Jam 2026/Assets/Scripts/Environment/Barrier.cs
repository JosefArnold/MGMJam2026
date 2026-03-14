using System.Collections;
using UnityEngine;

public class Barrier : Destructible {

  SpriteRenderer sr;

  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start() {
    sr = GetComponent<SpriteRenderer>();
  }

  // Update is called once per frame
  void Update() {

  }

  protected override void DamageEffect() {
    StartCoroutine(DamageFlash());
  }

  IEnumerator DamageFlash() {
    sr.color = new Color(1.0f, 0.41f, 0.41f);
    yield return new WaitForSeconds(0.5f);
    sr.color = Color.white;
  }

  protected override void Death() {
    Destroy(gameObject);
  }
}
