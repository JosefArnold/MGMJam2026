using UnityEngine;

public class CameraController : MonoBehaviour {

  [SerializeField] private float smooth;
  [SerializeField] private int cameraMovementRange;
  [SerializeField] private bool cam2D;

  private Vector3 target;

  private Vector3 maxPos;
  private Vector3 minPos;

  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start() {
    Player p = GameObject.Find("Player").GetComponent<Player>();
    p.CameraRef(this);

    Transform playerPos = p.transform;

    target = new Vector3(playerPos.position.x, playerPos.position.y, cam2D ? transform.position.z : playerPos.position.z);

    //TEMP
    minPos = new Vector3(-100, 1.0f, -10);
    maxPos = new Vector3(100, 100, -10);
  }

  // Update is called once per frame
  void LateUpdate() {
    if (transform.position != target) {
      Vector3 targetPos = new Vector3(target.x, target.y, target.z);

      targetPos.x = Mathf.Clamp(targetPos.x, minPos.x, maxPos.x);
      targetPos.y = Mathf.Clamp(targetPos.y, minPos.y, maxPos.y);
      targetPos.z = Mathf.Clamp(targetPos.z, minPos.z, maxPos.z);

      transform.position = Vector3.Lerp(transform.position, targetPos, smooth);
    }
  }

  public void SetBounds(Vector3 min, Vector3 max) {
    minPos = min;
    maxPos = max;
  }

  public void SetTarget(Vector3 playerTarget, Vector3 input) {
    Vector3 focusChange = input * cameraMovementRange;

    Vector3 newTarget = new Vector3(playerTarget.x + focusChange.x, playerTarget.y + focusChange.y,
      cam2D ? transform.position.z : playerTarget.z + focusChange.z);

    target = newTarget;
  }
}
