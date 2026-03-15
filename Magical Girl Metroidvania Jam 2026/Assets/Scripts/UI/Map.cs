using UnityEngine;
using UnityEngine.UI;

public class Map : Menu {

  [SerializeField] private GameObject[] rooms;
  [SerializeField] private GameObject playerIcon;

  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start() {

  }

  // Update is called once per frame
  void Update() {

  }

  public void OnEnable() {
    for (int i = 0; i < rooms.Length; i++) {
      rooms[i].SetActive(GameManager.ptr.GetSeenRooms()[i]);
    }
    playerIcon.transform.position = rooms[GameManager.ptr.currentRoom].transform.GetChild(0).position;
  }

  private void OnDisable() {
    PlayReturnSFX();
  }
}
