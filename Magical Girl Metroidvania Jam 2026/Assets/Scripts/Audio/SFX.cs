using System.Collections;
using UnityEngine;

public class SFX : MonoBehaviour {

  [SerializeField] private AudioSource audioSource;
  [SerializeField] private AudioClip[] clips;
  private float defaultVolume;
  private int[] cycleSounds;
  private int lastSFXIndex = 0;

  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start() {
    defaultVolume = audioSource.volume;
    audioSource.volume = SaveManager.ptr.settings.GetSFXVolume() * SaveManager.ptr.settings.GetMasterVolume() * defaultVolume;

    MusicManager.ptr.AddSFXObj(this);
  }

  // Some sounds we want to play once with a random index, and every time it's played we want a random index. For sounds like the footsteps, we want
  // them to be randomized but it needs to be constant

  // Update is called once per frame
  void Update() {

  }

  public void SetSFX(int index, bool loop, bool interrupt) {
    if (interrupt)
      StopSFX();

    Debug.Log("SFX: " + index);

    audioSource.clip = clips[index];
    audioSource.loop = loop;
    audioSource.Play();
  }
  
  public void StopSFX() {
    audioSource.Stop();
    StopAllCoroutines();
  }

  public void SetCycleIndices(int[] indices, bool interrupt) { // Gameobject gives sfx the array of indices to be randomized
    if (interrupt) // If we want this sound to interrupt whatever sound is playing - this makes sense for attacks and damage sounds, for walking sounds this'll be set to false
      StopSFX();

    if (!audioSource.isPlaying) { // If the audiosource isn't playing (or has been interrupted)
      cycleSounds = indices; // Assign
      int index = Random.Range(cycleSounds[0], cycleSounds[cycleSounds.Length - 1] + 1); // Generate random index
      SetSFX(index, false, interrupt); // Play
      lastSFXIndex = index;
    } else { // If the audiosource is playing something and we don't want this sound to interrupt
      StopAllCoroutines();
      cycleSounds = indices; // Assign
      StartCoroutine(CycleSounds()); // Wait for sound to finish
    }
  }

  public void SetVolume(float value1, float value2) {
    audioSource.volume = value1 * value2 * defaultVolume;
  }

  public bool IsPlaying() {
    return audioSource.isPlaying;
  }

  IEnumerator CycleSounds() {
    while (audioSource.isPlaying) { // Wait for audiosource to finish
      yield return null;
    }

    int index = lastSFXIndex;

    while (index == lastSFXIndex) {
      index = Random.Range(cycleSounds[0], cycleSounds[cycleSounds.Length - 1] + 1); // Generate random index
    }

    SetSFX(index, false, false); // Play
  }

  public void PlayUINavSFX() {
    SetSFX(1, false, true);
  }

}
