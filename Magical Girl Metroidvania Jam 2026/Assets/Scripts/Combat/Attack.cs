using System.Collections;
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

  Rigidbody2D rb;
  SpriteRenderer sr;
  Animator anim;
  BoxCollider2D boxCollider;

  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start() {

    if (lifespan > 0)
      StartCoroutine(ProjectileEndAnim());

  }

  // Update is called once per frame
  void Update() {

  }

  private void OnTriggerEnter2D(Collider2D collision) {
    if (collision != null) {

      if (collision.gameObject.CompareTag(targetTag) || (destroyBarrier && collision.gameObject.CompareTag("Barrier"))) {
        collision.gameObject.GetComponent<Destructible>().TakeDamage(damage);

        hits++;

        if (projectile && hits >= destroyAfterHits) {
          rb.linearVelocity = Vector2.zero;
          boxCollider.enabled = false;
          anim.SetTrigger("Destroy");
        }
      } else if (projectile && !collision.gameObject.CompareTag("Player") && !pierceWalls) {
        rb.linearVelocity = Vector2.zero;
        boxCollider.enabled = false;
        anim.SetTrigger("Destroy");
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
    rb = GetComponent<Rigidbody2D>();
    rb.linearVelocity = projDir * projectileSpeed;

    sr = GetComponent<SpriteRenderer>();
    anim = GetComponent<Animator>();
    boxCollider = GetComponent<BoxCollider2D>();

    sr.flipX = rb.linearVelocityX > 0 ? false : true;
  }

  IEnumerator ProjectileEndAnim() {
    yield return new WaitForSeconds(lifespan);
    rb.linearVelocity = Vector2.zero;
    boxCollider.enabled = false;
    anim.SetTrigger("Destroy");
  }

  public void AssignSprite(Sprite s) {
    sr.sprite = s;
  }

  public void DestroyProjectile() {
    Destroy(gameObject);
  }
}
