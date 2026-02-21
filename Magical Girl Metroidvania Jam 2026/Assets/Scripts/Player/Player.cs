using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, Controls.IPlayerActions {

  [Header("Player Variables")]
  [SerializeField] private int health;
  [SerializeField] private float baseMoveSpeed;
  [SerializeField] private float jumpHeight;
  [SerializeField] Vector2 boxSize; // For the ground raycasting
  [SerializeField] float groundDistance; // Also for the ground raycasting
  [SerializeField] LayerMask groundLayer;

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
  }

  // Update is called once per frame
  void FixedUpdate() {
    if (moveValue != Vector2.zero)
      rb.linearVelocityX = moveValue.x * baseMoveSpeed;
  }

  public bool IsGrounded() {
    if (Physics2D.BoxCast(transform.position, boxSize, transform.rotation.z, -transform.up, groundDistance, groundLayer))
      return true;
    else
      return false;
  }

  private void OnCollisionEnter2D(Collision2D collision) {
    if (collision != null)
      if (collision.gameObject.CompareTag("Ground"))
        IsGrounded();
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
  }

  public void OnInteract(InputAction.CallbackContext ctx) {

  }

  public void OnJump(InputAction.CallbackContext ctx) {
    if (ctx.performed && IsGrounded())
      rb.linearVelocityY = jumpHeight;

    if (ctx.canceled && !IsGrounded() && rb.linearVelocityY > 0)
      rb.linearVelocityY = rb.linearVelocityY / 2;
  }

  public void OnAttack(InputAction.CallbackContext ctx) {

  }

  public void OnLook(InputAction.CallbackContext ctx) {

  }

  public void TakeDamage() {
    Debug.Log("Owie :(");
  }

  public void Death() {

  }

}
