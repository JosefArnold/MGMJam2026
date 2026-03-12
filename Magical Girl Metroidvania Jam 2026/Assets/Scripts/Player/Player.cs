using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
  [SerializeField] GameObject basicProjectile;
  [SerializeField] GameObject chargeBlast;
  [SerializeField] GameObject flashbang;

  [Header("Ground Detection")]
  [SerializeField] Vector2 boxSize; // For the ground raycasting
  [SerializeField] float groundDistance; // Also for the ground raycasting
  [SerializeField] LayerMask groundLayer;
  [SerializeField] GameObject temporaryAttackObject; // DELETE LATER

  private Vector2 cameraFocusPoint;
  private Vector2 projectileSpawnPoint;
  private Vector2 dashTarget;
  private float facingDirection = 1;
  private bool projectileCharging;
  private float chargeTime;

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
  private bool holdingShoot = false;

  // References to change in script
  CameraController cam;

  // Interact Variables
  private bool interactableInRange;
  private Interactable interactable;

  // Components
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
    rb = GetComponent<Rigidbody2D>();
    //anim = GetComponent<Animator>();
    //sr = GetComponent<SpriteRenderer>();

    flightMeter = flightDuration;
  }

  // Update is called once per frame
  void FixedUpdate() {
    if (moveValue != Vector2.zero)
      rb.linearVelocityX = moveValue.x * baseMoveSpeed;

    MoveProjectileSpawnPoint();
    MoveCameraFocusPoint();

    if (holdingFlight && !IsGrounded() && flightMeter > 0.0f) {
      flightMeter -= Time.deltaTime;
      rb.linearVelocityY = flightSpeed;
    }

    if (projectileCharging && chargeTime < projectileChargeTime)
      chargeTime += Time.deltaTime;

    if (flashCooldown > 0.0f)
      flashCooldown -= Time.deltaTime;
  }

  public void CameraRef(CameraController c) {
    cam = c;
  }

  public bool IsGrounded() {
    if (Physics2D.BoxCast(transform.position, boxSize, transform.rotation.z, -transform.up, groundDistance, groundLayer))
      return true;
    else
      return false;
  }

  private void OnCollisionEnter2D(Collision2D collision) {
    if (collision != null && collision.gameObject.CompareTag("Ground"))
      if (IsGrounded())
        flightMeter = flightDuration;
  }

  // This is just so we can see the raycast in the Editor if we wanna make edits
  private void OnDrawGizmos() {
    Gizmos.matrix = Matrix4x4.identity;

    Gizmos.DrawWireCube(transform.position - transform.up * groundDistance, boxSize);
  }

  void OnDisable() => controls.Player.Disable();

  public void OnPause(InputAction.CallbackContext ctx) {
    UIManager.ptr.Pause();
  }

  public void OnMove(InputAction.CallbackContext ctx) {
    moveValue = ctx.ReadValue<Vector2>();

    if (moveValue.x != 0) 
      facingDirection = moveValue.x;

    if (ctx.canceled)
      Debug.Log("Check");
  }

  public void OnInteract(InputAction.CallbackContext ctx) {
    if (interactableInRange)
      interactable.Interact(this);
  }

  public void OnJump(InputAction.CallbackContext ctx) {
    if (abilities[0] && ctx.performed) {
      if (IsGrounded()) {
        rb.linearVelocityY = jumpHeight;
      } else if (abilities[4])
        holdingFlight = true;
    }

    if (ctx.canceled && !IsGrounded()) {
      holdingFlight = false;

      if (rb.linearVelocityY > 0)
        rb.linearVelocityY = rb.linearVelocityY / 2;
    }
  }

  public void OnAttack(InputAction.CallbackContext ctx) {
    // Just note to self for later: when we have the attack animation, I can use it to toggle the attack trigger on and off,
    // so in here I would just tell the animator to play that animation

    if (abilities[1])
      temporaryAttackObject.SetActive(true);
  }

  public void OnLook(InputAction.CallbackContext ctx) {
    Vector2 inputVector = ctx.ReadValue<Vector2>();

    lookValue = new Vector2 (Mathf.Clamp(inputVector.x, -1, 1), Mathf.Clamp(inputVector.y, -1, 1));
  }

  private void MoveProjectileSpawnPoint() {
    Vector3 playerPos = gameObject.transform.position;

    if (lookValue != Vector2.zero)
      projectileSpawnPoint = new Vector2(playerPos.x + (lookValue.x * 1.25f), playerPos.y + (lookValue.y * 1.25f));
    /*else if (moveValue != Vector2.zero) 
      projectileSpawnPoint = new Vector2(playerPos.x + moveValue.x, playerPos.y + moveValue.y);*/
    else
      projectileSpawnPoint = new Vector2(playerPos.x + (facingDirection * 1.25f), playerPos.y + (moveValue.y * 1.25f));
  }

  private void MoveCameraFocusPoint() {
    if (lookValue != Vector2.zero)
      cam.SetTarget(gameObject.transform.position, lookValue);
    else
      cam.SetTarget(gameObject.transform.position, moveValue / 2);
  }

  public void OnShoot(InputAction.CallbackContext ctx) {
    if (abilities[2] && ctx.performed) {
      Vector2 projAngle = new Vector2(moveValue.y > 0 && moveValue.x == 0 ? 0.0f : facingDirection, // If the player is "aiming" up but not moving
        moveValue.y > 0 ? moveValue.y : 0.0f); // If the player is aiming up

      projAngle.Normalize();

      Quaternion projRot = new Quaternion(0.0f, 0.0f, Vector2.SignedAngle(gameObject.transform.position, projectileSpawnPoint), 0.0f);

      GameObject proj = Instantiate(basicProjectile, projectileSpawnPoint, projRot);

      proj.GetComponent<Attack>().SetProjectileDirection(projAngle);

      if (abilities[3])
        projectileCharging = true;
    }

    if (ctx.canceled && abilities[3]) {
      if (chargeTime >= projectileChargeTime) {
        Vector2 projAngle = new Vector2(moveValue.y > 0 && moveValue.x == 0 ? 0.0f : facingDirection, // If the player is "aiming" up but not moving
        moveValue.y > 0 ? moveValue.y : 0.0f); // If the player is aiming up

        projAngle.Normalize();

        Quaternion projRot = new Quaternion(0.0f, 0.0f, Vector2.SignedAngle(gameObject.transform.position, projectileSpawnPoint), 0.0f);

        GameObject proj = Instantiate(chargeBlast, projectileSpawnPoint, projRot);

        proj.GetComponent<Attack>().SetProjectileDirection(projAngle);
      }

      projectileCharging = false;
      chargeTime = 0.0f;
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

    rb.gravityScale = 3.5f;
    controls.Enable();
  }

  protected override void DamageEffect() {
    ToggleIFrames();
    Invoke("ToggleIFrames", 0.5f);
  }

  private void ToggleIFrames() {
    iFrames = !iFrames;
  }

  protected override void Death() {
    controls.Disable();
    //Destroy(gameObject);
  }

  public void InteractableInRange(Interactable i) {
    interactable = i;

    if (interactable == null)
      interactableInRange = false;
    else
      interactableInRange = true;
  }

  public void ToggleAbility(int index) {
    abilities[index] = !abilities[index];
  }

  public void SetAbilities(bool[] b) {
    abilities = b;
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
  }

  public int GetHealth() {
    return health;
  }

}
