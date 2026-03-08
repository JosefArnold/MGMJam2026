using UnityEngine;

public class MusicManager : MonoBehaviour {

  public static MusicManager ptr;

  [SerializeField] private AudioSource source;

  private void Awake() {
    ptr = this;
  }

  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start() {

  }

  // Update is called once per frame
  void Update() {

  }
}
