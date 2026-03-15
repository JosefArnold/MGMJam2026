using System.Collections;
using UnityEngine;

public class SFX : MonoBehaviour {

  [SerializeField] private AudioSource audioSource;
  [SerializeField] private AudioClip[] clips;
  private float defaultVolume;
  private bool cyclingSounds;
  private int[] cycleSounds;
  private int lastSFXIndex;

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
  
  public void StopSFX() {
    audioSource.Stop();
    StopAllCoroutines();
  }

  public void SetCycleIndices(int[] indices) {
    if (!cyclingSounds) {
      cycleSounds = indices;
      SetSFX(0, false);
      lastSFXIndex = 0;
      StartCoroutine(CycleSounds());
      cyclingSounds = true;
    }
  }

  public void SetVolume(float value1, float value2) {
    audioSource.volume = value1 * value2 * defaultVolume;
  }

  IEnumerator CycleSounds() {
    while (audioSource.isPlaying) {
      yield return null;
    }

    int index = lastSFXIndex;

    while (index == lastSFXIndex) {
      index = Random.Range(0, cycleSounds.Length - 1);
    }

    SetSFX(index, false);
  }

}
