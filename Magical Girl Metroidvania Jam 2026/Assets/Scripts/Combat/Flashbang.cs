using System;
using UnityEngine;

public class Flashbang : MonoBehaviour {

  private Camera cam;
  private BoxCollider2D boxCollider;

  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start() {
    cam = transform.parent.GetComponent<Camera>();
    boxCollider = GetComponent<BoxCollider2D>();
    boxCollider.size = new Vector2(cam.orthographicSize * cam.aspect * (cam.fieldOfView / 70) * 3.0f, cam.orthographicSize * 3.0f * (cam.fieldOfView / 70));
  }

  private void OnEnable() {
    boxCollider.size = new Vector2(cam.orthographicSize * cam.aspect * (cam.fieldOfView / 70) * 3.0f, cam.orthographicSize * 3.0f * (cam.fieldOfView / 70));
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
