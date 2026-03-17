using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : Destructible {

  [Header("References")]
  [SerializeField] private Transform[] triggers;

  [Header("Base Enemy Variables")]
  [SerializeField] private bool moving;
  [SerializeField] private bool flying;
  [SerializeField] private float baseMoveSpeed;
  [SerializeField] private float attackCooldown; // TEMPORARY UNTIL ANIMATIONS, OTHERWISE I COULD HAVE THE ANIMATION SCRIPT TOGGLE THE ATTACKING BOOL ON OR OFF
  [SerializeField] private bool damagePlayerOnContact;
  [SerializeField] private bool stopMovingWhenPlayerInAttackRange;
  [SerializeField] private bool attacksInterruptMovement;
  [SerializeField] private bool turnToFacePlayer;
  [SerializeField] private float minDistance;
  [SerializeField] private GameObject[] attacks; // TEMPORARY UNTIL ANIMATIONS
  [SerializeField] private GameObject projSpawnPoint; // TEMPORARY

  [Header("Enemy Pathing")]
  [SerializeField] private GameObject[] pathMarkers;
  [SerializeField] private bool showPath; // This is just to toggle the editor visuals of the path on or off so it doesn't clutter the screen
  [SerializeField] private Transform[] leashMarkers;
  [SerializeField] private bool showBounds;
  private int currentMarker;
  private Vector2 velocity;

  // References to set in script
  private Player p;

  // Components
  private SFX sfx;
  private Rigidbody2D rb;
  private Animator anim;
  private SpriteRenderer sr;

  // Variables to actually change during gameplay
  private bool playerSpotted;
  private bool playerInAttackRange;
  private bool attacking;
  private int direction = 0;
  private bool stunned;

  private void Start() {
    sfx = GetComponent<SFX>();
    rb = transform.parent.GetComponent<Rigidbody2D>();
    anim = transform.parent.GetComponent<Animator>();
    sr = GetComponent<SpriteRenderer>();

    health = maxHealth;

    anim.SetBool("Flying", flying);

    if (flying)
      rb.bodyType = RigidbodyType2D.Kinematic;

    currentMarker = 1;

    if (moving)
      FollowPath(currentMarker);
  }

  private void FixedUpdate() {
    if (moving) {

      FaceDirection();
      Move();

    }
  }

  private void OnTriggerEnter2D(Collider2D collision) {
    if (collision != null) {
      if (collision.gameObject.CompareTag("Player") && damagePlayerOnContact) {
        Player player = collision.gameObject.GetComponent<Player>();

        player.TakeDamage(1);
      }
    }
  }

  private void Move() {
    if (!stunned) {
      if (!playerSpotted) {
        if (flying) {

          if (Vector2.Distance(pathMarkers[currentMarker].transform.position, transform.position) < 0.3f) {
            currentMarker = (currentMarker < pathMarkers.Length - 1 ? currentMarker + 1 : 0);
            FollowPath(currentMarker);
          }
          rb.linearVelocity = velocity;

        } else {

          if (Mathf.Abs(pathMarkers[currentMarker].transform.position.x - transform.position.x) < 0.3f) {
            currentMarker = (currentMarker < pathMarkers.Length - 1 ? currentMarker + 1 : 0);
            FollowPath(currentMarker);
          }
          rb.linearVelocityX = velocity.x;

        }
      } else if (playerSpotted && p != null) {

        if ((!playerInAttackRange || !stopMovingWhenPlayerInAttackRange) &&
        (!attacksInterruptMovement && !attacking) &&
        (minDistance == 0 || (minDistance != 0 && Vector2.Distance(transform.position, p.gameObject.transform.position) > minDistance))) {

          if (flying) {

            Vector2 playerCoords = p.gameObject.transform.position - transform.position;
            playerCoords.Normalize();

            velocity = playerCoords * baseMoveSpeed;

            rb.linearVelocity = velocity;

          } else {

            rb.linearVelocityX = p.gameObject.transform.position.x - transform.position.x < 0 ? -baseMoveSpeed : baseMoveSpeed;

          }

        } else if (minDistance > 0 && (minDistance - 0.25f) < Vector2.Distance(transform.position, p.gameObject.transform.position)
          && Vector2.Distance(transform.position, p.gameObject.transform.position) < (minDistance + 0.25f)) {
          if (flying) {
            Vector2 playerWaypoint = p.gameObject.transform.position - transform.position;
            playerWaypoint.Normalize();

            velocity = playerWaypoint * (-baseMoveSpeed / 1.5f);
          } else
            rb.linearVelocityX = p.gameObject.transform.position.x - transform.position.x < 0 ? baseMoveSpeed / 1.5f : -baseMoveSpeed / 1.5f;
        }

        if (playerInAttackRange && stopMovingWhenPlayerInAttackRange)
          rb.linearVelocity = new Vector2(0.0f, flying ? 0.0f : rb.linearVelocityY);

      }

      if (!flying && rb.linearVelocityX != 0.0f) {
        sfx.SetCycleIndices(new int[] { 0, 1, 2, 3 }, false);
      }
    }
  }

  private void FaceDirection() {
    if (!stunned) {
      if (playerSpotted && turnToFacePlayer && p != null) {
        direction = p.transform.position.x - transform.position.x < 0 ? -1 : 1;
      } else if (moving && rb.linearVelocityX != 0) {
        direction = rb.linearVelocityX > 0 ? 1 : -1;
      } else if (rb.linearVelocityX == 0)
        direction = 0;

      if (direction != 0) {
        foreach (Transform t in triggers) {
          t.localScale = new Vector3(direction, t.localScale.y, t.localScale.z);
        }
        sr.flipX = direction == -1.0f ? true : false;
      }
    }
  }

  private void FollowPath(int nextMarker) {
    if (!stunned) {
      if (flying) {
        Vector2 markerCoords = pathMarkers[nextMarker].transform.position - transform.position;
        markerCoords.Normalize();

        velocity = markerCoords * baseMoveSpeed;
      } else {
        velocity = new Vector2((pathMarkers[nextMarker].transform.position.x - transform.position.x < 0 ? -baseMoveSpeed : baseMoveSpeed), 0.0f);
      }
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

  protected override void DamageEffect() {
    StartCoroutine(DamageFlash());
  }

  IEnumerator DamageFlash() {
    sr.color = new Color(1.0f, 0.41f, 0.41f);
    Debug.Log("Color SHOULD be changed");
    yield return new WaitForSeconds(0.5f);
    sr.color = Color.white;
    Debug.Log("Color should be normal again");
  }

  protected override void Death() {
    sfx.SetSFX(4, false, true);
    anim.SetBool("Flying", flying);
    anim.SetTrigger("Death");
    moving = false;
    rb.linearVelocity = Vector2.zero;
    damagePlayerOnContact = false;

    StartCoroutine(DestroyObj());
  }

  IEnumerator DestroyObj() {
    while (sfx.IsPlaying()) {
      yield return null;
    }

    Destroy(transform.parent.parent.gameObject);
  }

  protected virtual void EnemyAttack() {
    if (!stunned) {
      attacking = true;
      // TEMPORARY
      //attacks[i].SetActive(true);

      // CUT AND PASTE FOR RANGED ENEMY CHILD SCRIPT
      if (p != null) {
        Vector2 projAngle = p.transform.position - projSpawnPoint.transform.position;
        projAngle.Normalize();

        GameObject proj = Instantiate(attacks[0], projSpawnPoint.transform.position, Quaternion.identity);
        proj.GetComponent<Attack>().SetProjectileDirection(projAngle);
      }
    }
  }

  public void SpotPlayer(bool spotted, bool inRange, Player player) {

    p = player;
    playerSpotted = spotted;
    playerInAttackRange = inRange;

    if (!spotted)
      FollowPath(currentMarker);

    if (inRange)
      InvokeRepeating("EnemyAttack", 0.5f, 1.5f);

    if (!inRange && !stunned)
      CancelInvoke();
  }

  public void Stun() {
    stunned = true;

    CancelInvoke();

    if (flying)
      rb.linearVelocity = Vector2.zero;
    else {
      rb.linearVelocityX = 0;
      anim.SetBool("Stunned", true);
    }

    StartCoroutine(StunWearOff());

    InvokeRepeating("StunDamage", 0.0f, 1.0f);
  }

  private void StunDamage() {
    TakeDamage(1);
  }

  IEnumerator StunWearOff() {
    yield return new WaitForSeconds(3.0f);

    CancelInvoke();
    anim.SetBool("Stunned", false);
    stunned = false;
  }
}
