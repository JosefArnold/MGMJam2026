using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

  public static GameManager ptr;

  [SerializeField] private Transform[] savePoints;

  public string nextScene; // Name of the next scene to load

  private bool[] roomsSeen = new bool[15];
  public int currentRoom;

  [SerializeField] public Player p;

  private void Awake() {
    ptr = this;
  }

  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start() {

  }

  // Update is called once per frame
  void Update() {

  }

  public void PrepSceneTransition(string scene) {
    nextScene = scene;
    p.ToggleControls(false);
    MusicManager.ptr.StartFade(2.0f, false);
    UIManager.ptr.onFade += ChangeScene;
    UIManager.ptr.ToggleElement(0);
    UIManager.ptr.SetFadeImage(null, 1.0f);
    UIManager.ptr.Fade();
  }

  public void ChangeScene() {
    SceneManager.LoadScene(nextScene);
  }

  public void Quit() {
    p.ToggleControls(false);
    UIManager.ptr.onFade += Application.Quit;
    UIManager.ptr.ToggleElement(0);
    UIManager.ptr.SetFadeImage(null, 1.0f);
    UIManager.ptr.Fade();
  }

  public Transform GetSavePoint(int index) {
    return savePoints[index];
  }

  public void SetSeenRooms(int index, bool[] b) {
    if (index != -1) {
      roomsSeen[index] = true;
      currentRoom = index;
    } else
      roomsSeen = b;
  }

  public bool[] GetSeenRooms() {
    return roomsSeen;
  }

}
