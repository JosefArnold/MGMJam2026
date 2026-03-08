using UnityEngine;

public class UIManager : MonoBehaviour {

  public static UIManager ptr;

  //Object references
  [SerializeField] private Player p;

  //UI Elements
  [SerializeField] private GameObject pauseMenu;

  //Variables
  private bool paused = false;

  private void Awake() {
    ptr = this;
  }

  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start() {

  }

  // Update is called once per frame
  void Update() {

  }

  public void Pause() {
    paused = !paused;

    pauseMenu.SetActive(paused);

    if (paused)
      Time.timeScale = 0.0f;
    else
      Time.timeScale = 1.0f;
  }
}
