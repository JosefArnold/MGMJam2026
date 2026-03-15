using System;
using UnityEngine;

public class Flashbang : MonoBehaviour {
  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start() {
    Camera cam = transform.parent.GetComponent<Camera>();
    gameObject.GetComponent<BoxCollider2D>().size = new Vector2(cam.orthographicSize * cam.aspect * 2.75f, cam.orthographicSize * 2.75f);
  }

  private void OnTriggerEnter2D(Collider2D collision) {
    if (collision != null) {
      if (collision.gameObject.CompareTag("Enemy")) {
        try {
          collision.gameObject.GetComponent<Enemy>().Stun();
        } catch {
          try {
            collision.gameObject.GetComponent<Barrier>().TakeDamage(5);
          } catch {
            collision.gameObject.GetComponent<Boss>().Stun();
          }
        }
      }
      
      if (collision.gameObject.CompareTag("RustWall")) {
        collision.gameObject.GetComponent<RustWall>().TakeDamage(3);
      }
    }

    gameObject.SetActive(false);
  }
}
