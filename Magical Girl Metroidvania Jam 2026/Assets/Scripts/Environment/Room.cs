using Unity.VisualScripting;
using UnityEngine;

public class Room : MonoBehaviour {

  [Header("Variables")]
  [SerializeField] private int[] roomIndices;
  [SerializeField] private bool lockCamera;
  [SerializeField] private Vector3 minBound;
  [SerializeField] private Vector3 maxBound;

  CameraController cam;

  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start() {
    cam = GameObject.Find("Main Camera").GetComponent<CameraController>();
  }

  // Update is called once per frame
  void Update() {

  }

  private void OnTriggerEnter2D(Collider2D collision) {
    if (collision != null && collision.gameObject.CompareTag("Player")) {
      if (!lockCamera)
        cam.SetBounds(minBound, maxBound);
      else
        cam.LockPosition(lockCamera, minBound);

      if (roomIndices != null)
        GameManager.ptr.SetSeenRooms(roomIndices[0], null);
    }
  }
}
