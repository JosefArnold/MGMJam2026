using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, Controls.IPlayerActions {

  [Header("Player Variables")]
  [SerializeField] private int baseMoveSpeed;
  [SerializeField] private int jumpHeight;

  //Components
  private Rigidbody2D rb;
  private Animator anim;

  //Input Controls
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

  public void OnMove(InputAction.CallbackContext ctx) {
    moveValue = ctx.ReadValue<Vector2>();
  }

  public void OnInteract(InputAction.CallbackContext ctx) {

  }

  public void OnJump(InputAction.CallbackContext ctx) {
    if (ctx.performed)
      rb.linearVelocityY = jumpHeight;
  }

  public void OnAttack(InputAction.CallbackContext ctx) {

  }

  public void OnLook(InputAction.CallbackContext ctx) {

  }

}
