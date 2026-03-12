using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

[CreateAssetMenu(fileName = "SettingsData", menuName = "Scriptable Objects/SettingsData")]
public class SettingsData : ScriptableObject {

  private float masterVolume = 1.0f;
  private float musicVolume = 1.0f;
  private float sfxVolume = 1.0f;

  public void SetMasterVolume(float volume) {
    masterVolume = volume;
  }

  public float GetMasterVolume() {
    return masterVolume;
  }

  public void SetMusicVolume(float volume) {
    musicVolume = volume;
  }

  public float GetMusicVolume() {
    return musicVolume;
  }

  public void SetSFXVolume(float volume) {
    sfxVolume = volume;
  }

  public float GetSFXVolume() {
    return sfxVolume;
  }

}
