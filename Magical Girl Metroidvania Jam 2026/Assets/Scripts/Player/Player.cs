using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Destructible, Controls.IPlayerActions {

  [Header("Player Variables")]
  [SerializeField] private float baseMoveSpeed;
  [SerializeField] private float jumpHeight;
  [SerializeField] private float flightSpeed;
  [SerializeField] private float flightDuration;
  [SerializeField] private float projectileChargeTime;
  [SerializeField] Vector2 boxSize; // For the ground raycasting
  [SerializeField] float groundDistance; // Also for the ground raycasting
  [SerializeField] LayerMask groundLayer;
  [SerializeField] GameObject temporaryAttackObject; // DELETE LATER
  [SerializeField] GameObject basicProjectile;
  [SerializeField] GameObject chargeBlast;
  private Vector2 cameraFocusPoint;
  private Vector2 projectileSpawnPoint;
  private float facingDirection = 1;
  private bool projectileCharging;
  private float chargeTime;

  // 0: Attack
  // 1: Shoot
  // 2: Flight
  // 3: Charge Shoot
  // 4: Whatever we make this ability lol
  private bool[] abilities = { true, true, true, false };
  private bool holdingFlight = false;
  private float flightMeter;
  private bool holdingShoot = false;

  // References to change in script
  CameraController cam;

  // Interact Variables
  private bool interactableInRange;
  private Interactable interactable;

  // Components
  private Rigidbody2D rb;
  private Animator anim;

  // Input Controls
  private Controls controls;
  private Vector2 moveValue;

  void OnEnable() {
    if (controls == null) {
      controls = new Controls();
      controls.Player.SetCallbacks(this);
    }

    controls.Player.Enable();
  }

  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start() {
    rb = GetComponent<Rigidbody2D>();

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

  }

  public void OnMove(InputAction.CallbackContext ctx) {
    moveValue = ctx.ReadValue<Vector2>();

    if (moveValue.x != 0) 
      facingDirection = moveValue.x;
  }

  public void OnInteract(InputAction.CallbackContext ctx) {
    if (interactableInRange)
      interactable.Interact();
  }

  public void OnJump(InputAction.CallbackContext ctx) {
    if (ctx.performed) {
      if (IsGrounded()) {
        rb.linearVelocityY = jumpHeight;
      } else if (abilities[2])
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

    if (abilities[0])
      temporaryAttackObject.SetActive(true);
  }

  public void OnLook(InputAction.CallbackContext ctx) {

  }

  private void MoveProjectileSpawnPoint() {
    Vector3 playerPos = gameObject.transform.position;

    if (moveValue.x != 0 || moveValue.y != 0) 
      projectileSpawnPoint = new Vector2(playerPos.x + moveValue.x, playerPos.y + moveValue.y);
    else
      projectileSpawnPoint = new Vector2(playerPos.x + facingDirection, playerPos.y + moveValue.y);
  }

  private void MoveCameraFocusPoint() {
    cam.SetTarget(gameObject.transform.position, moveValue);
  }

  public void OnShoot(InputAction.CallbackContext ctx) {
    if (abilities[1] && ctx.performed) {
      Vector2 projAngle = new Vector2(moveValue.y > 0 && moveValue.x == 0 ? 0.0f : facingDirection, // If the player is "aiming" up but not moving
        moveValue.y > 0 ? moveValue.y : 0.0f); // If the player is aiming up

      projAngle.Normalize();

      Quaternion projRot = new Quaternion(0.0f, 0.0f, Vector2.SignedAngle(gameObject.transform.position, projectileSpawnPoint), 0.0f);

      GameObject proj = Instantiate(basicProjectile, projectileSpawnPoint, projRot);

      proj.GetComponent<Attack>().SetProjectileDirection(projAngle);

      projectileCharging = true;
    }

    if (ctx.canceled) {
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

  protected override void Death() {
    Destroy(gameObject);
  }

  public void InteractableInRange(Interactable i) {
    interactable = i;

    if (interactable == null)
      interactableInRange = false;
    else
      interactableInRange = true;
  }

}
