using System.Runtime.CompilerServices;
using UnityEngine;

public class CameraController : MonoBehaviour {

  [SerializeField] private Camera cam;
  [SerializeField] private float smooth;
  [SerializeField] private int cameraMovementRange;
  [SerializeField] private bool cam2D;

  private float defaultFOV;
  private float targetFOV;

  private Vector3 target;

  private Vector3 maxPos;
  private Vector3 minPos;

  private bool lockPosition;
  private Vector3 positionToLockTo;

  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start() {
    Player p = GameObject.Find("Player").GetComponent<Player>();
    p.CameraRef(this);

    Transform playerPos = p.transform;

    target = new Vector3(playerPos.position.x, playerPos.position.y, cam2D ? transform.position.z : playerPos.position.z);

    defaultFOV = cam.fieldOfView;

    //TEMP
    //minPos = new Vector3(-1, 1.0f, -10);
    //maxPos = new Vector3(100, 100, -10);
  }

  // Update is called once per frame
  void LateUpdate() {
    if (transform.position != target) {
      Vector3 targetPos = new Vector3(target.x, target.y, target.z);

      //targetPos.x = Mathf.Clamp(targetPos.x, minPos.x, maxPos.x);
      //targetPos.y = Mathf.Clamp(targetPos.y, minPos.y, maxPos.y);
      //targetPos.z = Mathf.Clamp(targetPos.z, minPos.z, maxPos.z);

      transform.position = Vector3.Lerp(transform.position, targetPos, smooth);
    }

    if (targetFOV != 0 && cam.fieldOfView != targetFOV) {
      cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, smooth);
    }
  }

  public void SetBounds(Vector3 min, Vector3 max) {
    lockPosition = false;
    minPos = min;
    maxPos = max;
  }

  public void SetFOV(float newFOV) {
    if (newFOV != 0)
      targetFOV = newFOV;
    else
      targetFOV = defaultFOV;
  }

  public void SetTarget(Vector3 playerTarget, Vector3 input) {
    if (!lockPosition) {
      Vector3 focusChange = input * cameraMovementRange;

      Vector3 newTarget = new Vector3(playerTarget.x + focusChange.x, playerTarget.y + focusChange.y,
        cam2D ? transform.position.z : playerTarget.z + focusChange.z);

      target = newTarget;
    }
  }

  public void LockPosition(bool lockPos, Vector3 newTarget) {
    lockPosition = lockPos;

    if (lockPos)
      target = newTarget;
  }
}
