using UnityEngine;
using UnityEngine.Playables;

public class CutsceneTrigger : MonoBehaviour {
  [SerializeField] private PlayableDirector director;

  private void OnTriggerEnter2D(Collider2D collision) {
    if (collision != null && collision.gameObject.CompareTag("Player")) {
      GameManager.ptr.p.ToggleControls(false);
      director.Play();
    }
  }

  public void StopCutscene() {
    GameManager.ptr.p.ToggleControls(true);
    director.Stop();
    Destroy(gameObject);
  }
}
