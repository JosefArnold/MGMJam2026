using System.Collections;
using UnityEditor.UIElements;
using UnityEngine;

public class Attack : MonoBehaviour {

  [Header("References")]
  [SerializeField] private string targetTag;

  [Header("General Variables")]
  [SerializeField] private int damage;
  [SerializeField] private bool continuousDamage;

  [Header("Projectile Variables")]
  [SerializeField] private bool projectile;
  [SerializeField] private float projectileSpeed;
  [SerializeField] private float lifespan; // How long until this destroys itself?
  [SerializeField] private int destroyAfterHits;
  [SerializeField] private bool pierceWalls;
  [SerializeField] private bool tracking;
  [SerializeField] private bool destroyBarrier;
  private Vector2 projectileDirection;
  private int hits;

  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start() {

    if (lifespan > 0)
      StartCoroutine(DestroyProjectile());

  }

  // Update is called once per frame
  void Update() {

  }

  private void OnTriggerEnter2D(Collider2D collision) {
    if (collision != null) {

      if (collision.gameObject.CompareTag(targetTag) || (destroyBarrier && collision.gameObject.CompareTag("Barrier"))) {
        collision.gameObject.GetComponent<Destructible>().TakeDamage(damage);

        hits++;

        Debug.Log(hits);

        if (projectile && hits >= destroyAfterHits) {
          Destroy(gameObject);
        }
      } else if (projectile && !collision.gameObject.CompareTag("Player") && !pierceWalls) {
        Destroy(gameObject);
      }

    }
  }

  private void OnTriggerStay2D(Collider2D collision) {
    if (continuousDamage) {
      if (collision != null && collision.gameObject.CompareTag(targetTag))
        collision.gameObject.GetComponent<Destructible>().TakeDamage(damage);
    }
  }

  public void SetProjectileDirection(Vector2 projDir) {
    gameObject.GetComponent<Rigidbody2D>().linearVelocity = projDir * projectileSpeed;
  }

  IEnumerator DestroyProjectile() {
    yield return new WaitForSeconds(lifespan);

    Destroy(gameObject);
  }
}
