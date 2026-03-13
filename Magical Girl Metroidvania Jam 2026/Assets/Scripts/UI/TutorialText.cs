using UnityEngine;

public class TutorialText : MonoBehaviour {
  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void OnEnable() {
    Invoke("SelfDestruct", 10.0f);
  }

  // Update is called once per frame
  void Update() {

  }

  private void SelfDestruct() {
    Destroy(gameObject);
  }
}
