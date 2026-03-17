using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Player : Destructible, Controls.IPlayerActions {

  [Header("Movement Variables")]
  [SerializeField] private float baseMoveSpeed;
  [SerializeField] private float jumpHeight;

  [Header("Ability Variables")]
  [SerializeField] private float flightSpeed;
  [SerializeField] private float flightDuration;
  [SerializeField] private float projectileChargeTime;
  [SerializeField] private float dashDistance;
  [SerializeField] private float dashCooldown;
  [SerializeField] private float flashbangCooldown;
  [SerializeField] GameObject melee;
  [SerializeField] GameObject aimObj;
  [SerializeField] SpriteRenderer aimSprite;
  [SerializeField] Animator aimAnim;
  [SerializeField] GameObject basicProjectile;
  [SerializeField] GameObject chargeBlast;
  [SerializeField] GameObject flashbang;

  [Header("Ground Detection")]
  [SerializeField] Vector2 boxSize; // For the ground raycasting
  [SerializeField] float groundDistance; // Also for the ground raycasting
  [SerializeField] LayerMask groundLayer;

  private Vector2 cameraFocusPoint;
  private Vector2 projectileSpawnPoint;
  private Vector2 dashTarget;
  private float facingDirection = 1;
  private bool projectileCharging;
  private float chargeTime = 0.0f;

  // 0: Jump
  // 1: Attack
  // 2: Shoot
  // 3: Charge Shot
  // 4: Flight
  // 5: Fullscreen Flash Stun
  private bool[] abilities = { false, false, false, false, false, false };
  private bool holdingFlight = false;
  private float flightMeter;
  private float flashCooldown;
  private bool attacking = false;

  // References to change in script
  CameraController cam;

  // Interact Variables
  private bool interactableInRange;
  private Interactable interactable;

  // Components
  private SFX sfx;
  private Rigidbody2D rb;
  private Animator anim;
  private SpriteRenderer sr;

  // Input Controls
  private Controls controls;
  private Vector2 moveValue;
  private Vector2 lookValue;

  void OnEnable() {
    if (controls == null) {
      controls = new Controls();
      controls.Player.SetCallbacks(this);
    }
  }

  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start() {
    health = maxHealth;
    sfx = GetComponent<SFX>();
    rb = GetComponent<Rigidbody2D>();
    anim = GetComponent<Animator>();
    sr = GetComponent<SpriteRenderer>();

    if (SceneManager.GetActiveScene().name.Equals("BeginningArea"))
      anim.SetBool("IntroOver", false);
    else
      anim.SetBool("IntroOver", true);

    flightMeter = flightDuration;
  }

  // Update is called once per frame
  void FixedUpdate() {
    if (moveValue != Vector2.zero)
      rb.linearVelocityX = moveValue.x * baseMoveSpeed;

    if (moveValue.x != 0 && IsGrounded()) // Walking SFX
      sfx.SetCycleIndices(new int[] { 0, 1, 2, 3 }, false);

    if (rb.linearVelocityY != 0 && !IsGrounded())
      anim.SetBool("Midair", true);

    MoveProjectileSpawnPoint();
    MoveCameraFocusPoint();

    if (holdingFlight && !IsGrounded() && flightMeter > 0.0f) {
      flightMeter -= Time.deltaTime;
      rb.linearVelocityY = flightSpeed;
      LevelHUD.ptr.UpdateUIMeters(0, flightMeter);
    } else
      anim.SetBool("Flying", false);

    if (projectileCharging && chargeTime < projectileChargeTime)
      chargeTime += Time.deltaTime;

    if (flashCooldown > 0.0f) {
      flashCooldown -= Time.deltaTime;
      LevelHUD.ptr.UpdateUIMeters(1, -flashCooldown);
    }

    anim.SetFloat("xSpeed", Mathf.Abs(rb.linearVelocityX));
    aimAnim.SetFloat("TimeCharged", chargeTime);
  }

  public bool IsGrounded() {
    if (Physics2D.BoxCast(transform.position, boxSize, transform.rotation.z, -transform.up, groundDistance, groundLayer))
      return true;
    else
      return false;
  }

  private void OnCollisionEnter2D(Collision2D collision) {
    if (collision != null && collision.gameObject.CompareTag("Ground")) {
      if (IsGrounded()) {
        flightMeter = flightDuration;
        LevelHUD.ptr.UpdateUIMeters(0, flightDuration);
        LevelHUD.ptr.ToggleUISlider(0, false);
        anim.SetBool("Midair", false);
        anim.SetTrigger("Landed");
      } else {
        anim.ResetTrigger("Landed");
      }
    }
  }

  // This is just so we can see the raycast in the Editor if we wanna make edits
  private void OnDrawGizmos() {
    Gizmos.matrix = Matrix4x4.identity;

    Gizmos.DrawWireCube(transform.position - transform.up * groundDistance, boxSize);
  }

  /********* Animation Triggers and Resets ************/

  public void IntroCutsceneFinished() {
    ToggleControls(true);
    anim.SetBool("StartGame", true);
  }

  public void MagicalGirlTransformation() {
    anim.SetBool("IntroOver", true);
  }

  public void ResetLanded() {
    anim.ResetTrigger("Landed");
  }

  public void ResetAttack() {
    attacking = false;
    anim.ResetTrigger("Attacking");
  }

  private void ResetShoot() {
    aimAnim.ResetTrigger("Pressed");
  }

  private void ResetFired() {
    aimAnim.ResetTrigger("Fired");
  }

  private void ResetBeginningSceneRespawn() {
    anim.ResetTrigger("BeginningSceneRespawn");
  }

  /********* Player Input Controls ************/

  void OnDisable() => controls.Player.Disable();

  public void OnPause(InputAction.CallbackContext ctx) {
    UIManager.ptr.Pause();
  }

  public void OnMove(InputAction.CallbackContext ctx) {
    moveValue = ctx.ReadValue<Vector2>();

    if (moveValue.x != 0) {
      facingDirection = moveValue.x < 0 ? -1.0f : 1.0f;
      sr.flipX = facingDirection == -1.0f ? true : false;
      melee.transform.localScale = new Vector3(facingDirection == -1 ? -1.0f : 1.0f, 1.0f, 1.0f);
    } else
      sfx.StopSFX();
  }

  public void OnInteract(InputAction.CallbackContext ctx) {
    if (interactableInRange)
      interactable.Interact(this);
  }

  public void OnJump(InputAction.CallbackContext ctx) {
    if (abilities[0] && ctx.performed) {
      if (!attacking && IsGrounded()) {
        sfx.StopSFX();
        anim.SetTrigger("Jump");
        anim.ResetTrigger("Landed");
        anim.SetBool("Midair", true);
        rb.linearVelocityY = jumpHeight;
      } else if (!IsGrounded() && abilities[4]) {
        anim.SetBool("Flying", true);
        holdingFlight = true;
        LevelHUD.ptr.ToggleUISlider(0, true);
      }
    }

    if (ctx.canceled) {
      if (!IsGrounded()) {
        holdingFlight = false;
        anim.SetBool("Flying", false);
        LevelHUD.ptr.ToggleUISlider(0, false);

        if (rb.linearVelocityY > 0)
          rb.linearVelocityY = rb.linearVelocityY / 2;
      }
      anim.ResetTrigger("Jump");
    }
  }

  public void OnAttack(InputAction.CallbackContext ctx) {
    if (abilities[1] && !attacking && !projectileCharging) {
      attacking = true;
      sfx.SetCycleIndices(new int[] { 4, 5, 6, 7 }, true);
      anim.SetTrigger("Attacking");
    }
  }

  public void OnLook(InputAction.CallbackContext ctx) {
    Vector2 inputVector = ctx.ReadValue<Vector2>();

    lookValue = new Vector2 (Mathf.Clamp(inputVector.x, -1, 1), Mathf.Clamp(inputVector.y, -1, 1));
  }

  private void MoveProjectileSpawnPoint() {
    Vector3 playerPos = gameObject.transform.position;

    if (lookValue != Vector2.zero)
      projectileSpawnPoint = new Vector2((playerPos.x + (lookValue.x * 1.25f)), playerPos.y + (lookValue.y * 1.25f));
    else
      projectileSpawnPoint = new Vector2(playerPos.x + ( (moveValue.y <= 0  || moveValue.x != 0) ? (facingDirection * 1.25f) : 0.0f),
        playerPos.y + (moveValue.y * 1.25f));

    aimSprite.flipX = facingDirection == -1.0f ? true : false;

    float zRot = 0.0f;

    if (moveValue.y > 0.0f || lookValue.y > 0.0f) {
      zRot = (moveValue.x != 0.0f || lookValue.x != 0.0f) ? 45.0f : 90.0f;
    } else if (moveValue.y < 0.0f || lookValue.y < 0.0f)
      zRot = -45f;

    zRot = aimSprite.flipX ? -zRot : zRot;

    Vector3 aimRot = new Vector3(0.0f, 0.0f, zRot);

    aimObj.transform.localEulerAngles = aimRot;
  }

  private void MoveCameraFocusPoint() {
    if (lookValue != Vector2.zero)
      cam.SetTarget(gameObject.transform.position, lookValue);
    else 
      cam.SetTarget(gameObject.transform.position, moveValue / 2);
  }

  public void OnShoot(InputAction.CallbackContext ctx) {
    if (abilities[2] && !attacking && ctx.performed) {
      Vector2 projAngle = new Vector2((moveValue.y > 0 && moveValue.x == 0) ? 0.0f : facingDirection, // If the player is "aiming" up but not moving
        moveValue.y); // If the player is aiming up

      aimAnim.SetTrigger("Pressed");
      Invoke("ResetShoot", 0.05f);

      projAngle.Normalize();

      /*
      float zRot = 0.0f;

      if (projectileSpawnPoint.y > 0)
        zRot = projectileSpawnPoint.x > 0 ? 45.0f : 90.0f;

      Quaternion projRot = new Quaternion(0.0f, 0.0f, zRot, 0.0f);
      */

      GameObject proj = Instantiate(basicProjectile, projectileSpawnPoint, Quaternion.identity);

      proj.GetComponent<Attack>().SetProjectileDirection(projAngle);

      if (abilities[3] && !attacking)
        projectileCharging = true;
    }

    if (ctx.canceled && abilities[3]) {
      if (chargeTime >= projectileChargeTime) {
        Vector2 projAngle = new Vector2((moveValue.y > 0 && moveValue.x == 0) ? 0.0f : facingDirection, // If the player is "aiming" up but not moving
        moveValue.y);

        projAngle.Normalize();

        /*
        Quaternion projRot = new Quaternion(0.0f, 0.0f, Vector2.SignedAngle(gameObject.transform.position, projectileSpawnPoint), 0.0f);
        */

        GameObject proj = Instantiate(chargeBlast, projectileSpawnPoint, Quaternion.identity);

        proj.GetComponent<Attack>().SetProjectileDirection(projAngle);
      }

      projectileCharging = false;
      chargeTime = 0.0f;
    }

    if (ctx.canceled) {
      aimAnim.SetTrigger("Fired");
      Invoke("ResetFired", 0.05f);
    }
  }

  public void OnFlashbang(InputAction.CallbackContext ctx) {
    if (abilities[5] && flashCooldown <= 0) {
      ToggleIFrames();
      rb.linearVelocity = Vector2.zero;
      rb.gravityScale = 0.0f;
      controls.Disable();
      flashCooldown = flashbangCooldown + 1.0f;

      Invoke("Flashbang", 1.0f);
      Invoke("ToggleIFrames", 1.0f);
    }
  }

  private void Flashbang() {
    flashbang.SetActive(true);
    sfx.SetSFX(11, false, true);
    LevelHUD.ptr.UpdateUIMeters(1, -flashbangCooldown);

    rb.gravityScale = 3.5f;
    controls.Enable();
  }

  /********* Player Damage ************/

  protected override void DamageEffect() {
    ToggleIFrames();
    sfx.SetCycleIndices(new int[] { 8, 9 }, true);
    StartCoroutine(DamageFlash());
    Invoke("ToggleIFrames", 0.5f);
    Debug.Log("Player Health: " + health);
    LevelHUD.ptr.UpdateHealth(health);
  }

  private void ToggleIFrames() {
    iFrames = !iFrames;

    /*
    if (!iFrames) {
      List<Collider2D> list = new List<Collider2D>();
      Physics2D.OverlapCollider(gameObject.GetComponent<BoxCollider2D>(), list);

      for (int i = 0; i < list.Count; i++) {
        if (list[i].gameObject.CompareTag("Enemy")) {
          TakeDamage(1);
          break;
        }
      }
    }
    */
  }

  IEnumerator DamageFlash() {
    sr.color = new Color(1.0f, 0.41f, 0.41f);
    yield return new WaitForSeconds(0.5f);
    sr.color = Color.white;
  }

  protected override void Death() {
    LevelHUD.ptr.UpdateHealth(health);
    sfx.SetSFX(9, false, true);
    anim.SetTrigger("Death");
    controls.Disable();
    //Destroy(gameObject);
  }

  public void DeathAnimFinished() {
    string sceneName = SceneManager.GetActiveScene().name;

    if (!sceneName.Equals("BeginningArea"))
      GameManager.ptr.PrepSceneTransition(SceneManager.GetActiveScene().name);
    else {
      UIManager.ptr.onFade += BeginningAreaLoad;
      UIManager.ptr.ToggleElement(0);
      UIManager.ptr.SetFadeImage(null, 1.0f);
      UIManager.ptr.Fade();
    }
  }

  public void BeginningAreaLoad() {
    transform.position = GameManager.ptr.GetSavePoint(0).position;
    health = 5;
    LevelHUD.ptr.UpdateHealth(health);
    anim.SetTrigger("BeginningSceneRespawn");
    UIManager.ptr.onFade += controls.Enable;
    UIManager.ptr.onFade += ResetBeginningSceneRespawn;
    UIManager.ptr.SetFadeImage(null, 0.0f);
    UIManager.ptr.Fade();
  }

  public void InteractableInRange(Interactable i) {
    interactable = i;

    if (interactable == null)
      interactableInRange = false;
    else
      interactableInRange = true;
  }

  /********* Setting & Getting Variables ************/

  public void CameraRef(CameraController c) {
    cam = c;
  }

  public void ToggleAbility(int index, bool b) {
    abilities[index] = b;

    if (abilities[5] && !SceneManager.GetActiveScene().name.Equals("BeginningArea"))
      LevelHUD.ptr.ToggleUISlider(1, true);
  }

  public void SetAbilities(bool[] b) {
    abilities = b;

    if (abilities[5] && !SceneManager.GetActiveScene().name.Equals("BeginningArea"))
      LevelHUD.ptr.ToggleUISlider(1, true);
  }

  public bool[] GetAbilities() {
    return abilities;
  }

  public void ToggleControls(bool enabled) {
    if (enabled)
      controls.Enable();
    else
      controls.Disable();
  }

  public void SetHealth(int h) {
    health = h;
    LevelHUD.ptr.UpdateHealth(health);
  }

  public int GetHealth() {
    return health;
  }

}
