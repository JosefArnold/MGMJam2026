using UnityEngine;

public class SFX : MonoBehaviour {

  [SerializeField] private AudioSource audioSource;
  [SerializeField] private AudioClip[] clips;
  private float defaultVolume;

  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start() {
    defaultVolume = audioSource.volume;
    audioSource.volume = SaveManager.ptr.settings.GetSFXVolume() * SaveManager.ptr.settings.GetMasterVolume() * defaultVolume;

    MusicManager.ptr.AddSFXObj(this);
  }

  // Update is called once per frame
  void Update() {

  }

  public void SetSFX(int index, bool loop) {
    audioSource.Stop();
    audioSource.clip = clips[index];
    audioSource.loop = loop;
    audioSource.Play();
  }

  public void SetVolume(float value1, float value2) {
    audioSource.volume = value1 * value2 * defaultVolume;
  }
}
