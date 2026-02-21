using UnityEngine;

public class Enemy : MonoBehaviour {

  [Header("Base Enemy Variables")]
  [SerializeField] private int health;
  [SerializeField] private bool moving;
  [SerializeField] private bool flying;
  [SerializeField] private float baseMoveSpeed;
  [SerializeField] private bool damagePlayerOnContact;

  [Header("Enemy Pathing")]
  [SerializeField] private GameObject[] pathMarkers;
  [SerializeField] private bool showPath; // This is just to toggle the editor visuals of the path on or off so it doesn't clutter the screen
  private int currentMarker;
  private Vector2 velocity;

  // Components
  private Rigidbody2D rb;
  private Animator anim;

  // Variables to actually change during gameplay
  private bool PlayerSpotted;

  private void Start() {
    rb = transform.parent.GetComponent<Rigidbody2D>();

    if (flying)
      rb.bodyType = RigidbodyType2D.Kinematic;

    if (moving)
      FollowPath(1);
  }

  private void FixedUpdate() {
    if (moving) {

      if (flying) {

        if (Vector2.Distance(pathMarkers[currentMarker].transform.position, transform.position) < 0.25f) {
          currentMarker = (currentMarker < pathMarkers.Length - 1 ? currentMarker + 1 : 0);
          FollowPath(currentMarker);
        }
        rb.linearVelocity = velocity;

      } else {

        if (Mathf.Abs(pathMarkers[currentMarker].transform.position.x - transform.position.x) < 0.25f) {
          currentMarker = (currentMarker < pathMarkers.Length - 1 ? currentMarker + 1 : 0);
          FollowPath(currentMarker);
        }
        rb.linearVelocityX = velocity.x;

      }

    }
  }

  private void OnTriggerEnter2D(Collider2D collision) {
    if (collision != null) {
      if (collision.gameObject.CompareTag("Player") && damagePlayerOnContact) {
        Player p = collision.gameObject.GetComponent<Player>();

        p.TakeDamage();
      }
    }
  }

  private void FollowPath(int nextMarker) {
    if (flying) {
      Vector2 markerCoords = pathMarkers[nextMarker].transform.position - transform.position;
      markerCoords.Normalize();

      velocity = markerCoords * baseMoveSpeed;
    } else {
      velocity = new Vector2((pathMarkers[nextMarker].transform.position.x - transform.position.x < 0 ? -baseMoveSpeed : baseMoveSpeed), 0.0f);
    }
  }

  private void OnDrawGizmos() {
    if (!transform.parent.gameObject.GetComponent<Rigidbody2D>() || !transform.parent.gameObject.GetComponent<BoxCollider2D>()) {
      Debug.Log("Enemy needs to have a parent object with a rigidbody and a box collider");
      Destroy(gameObject);
    }

    if (showPath) {
      for (int i = 0; i < pathMarkers.Length; i++) {
        Gizmos.DrawWireSphere(pathMarkers[i].transform.position, 0.5f);

        if (i < pathMarkers.Length - 1)
          Gizmos.DrawLine(pathMarkers[i].transform.position, pathMarkers[i + 1].transform.position);
        else
          Gizmos.DrawLine(pathMarkers[i].transform.position, pathMarkers[0].transform.position);
      }
    }
  }

  public void TakeDamage(int damage) {
    health -= damage;

    if (health <= 0) 
      Death();
  }

  private void Death() {
    Destroy(transform.parent.gameObject);
  }

}
