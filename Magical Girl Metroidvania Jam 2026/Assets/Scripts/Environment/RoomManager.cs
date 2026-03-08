using UnityEngine;

public class RoomManager : MonoBehaviour {

  public static RoomManager ptr;

  [Header("References")]
  [SerializeField] private GameObject[] rooms;

  private void Awake() {
    ptr = this;
  }

  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start() {

  }

  // Update is called once per frame
  void Update() {

  }

  public void UpdateRooms(int[] indices) {
    for (int i = 0; i < indices.Length; i++) {
      if (i != indices[i])
        rooms[i].SetActive(false);
      else
        rooms[i].SetActive(true);
    }
  }
}
