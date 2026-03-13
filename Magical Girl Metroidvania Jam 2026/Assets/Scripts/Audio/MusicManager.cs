using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour {

  public static MusicManager ptr;

  [SerializeField] private AudioSource audioSource;
  private List<SFX> sfxObjs = new List<SFX>();
  private float defaultVolume;

  private void Awake() {
    ptr = this;
  }

  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start() {
    defaultVolume = audioSource.volume;
    audioSource.volume = SaveManager.ptr.settings.GetMusicVolume() * SaveManager.ptr.settings.GetMasterVolume() * defaultVolume;
    DelayPlayTrack(1.0f);
  }

  // Update is called once per frame
  void Update() {

  }

  public void AddSFXObj(SFX sfx) {
    sfxObjs.Add(sfx);
  }

  public List<SFX> GetSFXObjs() {
    return sfxObjs;
  }

  public void SetVolume(float value1, float value2) {
    audioSource.volume = value1 * value2 * defaultVolume;
  }

  public void SetTrack(AudioClip newTrack) {
    audioSource.clip = newTrack;
  }

  // Wait some time, THEN play the track
  public void DelayPlayTrack(float time) => Invoke(nameof(PlayTrack), time);

  public void PlayTrack() {
    audioSource.Play();
  }

  public void StartFade(float fadeSpeed, bool fadeIn) {
    StartCoroutine(FadeTrack(fadeSpeed, fadeIn));
  }

  private IEnumerator FadeTrack(float fadeSpeed, bool fadeIn) {
    if (fadeIn) {
      while (audioSource.volume < 1) {
        audioSource.volume += fadeSpeed * Time.deltaTime;

        yield return new WaitForSeconds(Time.deltaTime);
      }
    } else {
      while (audioSource.volume > 0) {
        audioSource.volume -= fadeSpeed * Time.deltaTime;

        if (audioSource.volume <= 0)
          audioSource.Stop();

        yield return new WaitForSeconds(Time.deltaTime);
      }
    }
  }
}
