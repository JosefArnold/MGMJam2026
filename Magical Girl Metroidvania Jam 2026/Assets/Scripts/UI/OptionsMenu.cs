using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : Menu {

  [Header("UI References")]
  [SerializeField] private Slider[] soundSliders;

  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start() {

  }

  // Update is called once per frame
  void Update() {

  }

  public void UpdateSettingsValues(float[] volume) {
    for (int i = 0; i < soundSliders.Length; i++)
      soundSliders[i].value = volume[i];
  }

  public void UpdateSoundSource(AudioSource sound, int index) {
    soundSliders[index].onValueChanged.AddListener((float value) => sound.volume = value);
  }
}
