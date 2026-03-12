using UnityEngine;
using UnityEngine.UI;

public class Map : Menu {

  [SerializeField] private GameObject[] rooms;

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
  }
}
