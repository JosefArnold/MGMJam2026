using UnityEngine;

public class SaveManager : MonoBehaviour {

  public static SaveManager ptr;

  [SerializeField] public SettingsData settings;

  private void Awake() {
    if (ptr != null) {
      Destroy(gameObject);
      return;
    }

    ptr = this;
  }

  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start() {
    DontDestroyOnLoad(gameObject);
  }

  // Update is called once per frame
  void Update() {

  }

  public void UpdateSounds() {

  }
}
