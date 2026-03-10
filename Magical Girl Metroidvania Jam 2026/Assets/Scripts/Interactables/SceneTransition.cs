using UnityEngine;

public class SceneTransition : Interactable {

  [SerializeField] private string nextScene;
  [SerializeField] private int spawnIndex;

  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start() {

  }

  // Update is called once per frame
  void Update() {

  }

  public override void Interact(Player p) {
    SaveManager.ptr.Save(spawnIndex, p);
    GameManager.ptr.PrepSceneTransition(nextScene);
  }
}
