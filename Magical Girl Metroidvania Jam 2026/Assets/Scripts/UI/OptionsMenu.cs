using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionsMenu : Menu {

  private bool initialized;

  private float masterVol;
  private float musicVol;
  private float sfxVol;

  [Header("UI References")]
  [SerializeField] private Slider[] soundSliders;
  [SerializeField] private Button[] buttons;

  private bool switchedListener;

  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start() {
    
  }

  private void OnEnable() {
    if (UIManager.ptr.CheckNewGame() && !switchedListener) {
      NewGameRemap();
      switchedListener = true;
    }

    if (!initialized) {
      masterVol = SaveManager.ptr.settings.GetMasterVolume();
      musicVol = SaveManager.ptr.settings.GetMusicVolume();
      sfxVol = SaveManager.ptr.settings.GetSFXVolume();

      soundSliders[0].value = masterVol;
      soundSliders[1].value = musicVol;
      soundSliders[2].value = sfxVol;

      soundSliders[0].onValueChanged.AddListener((float value) => masterVol = value);
      soundSliders[0].onValueChanged.AddListener(delegate { SaveManager.ptr.settings.SetMasterVolume(masterVol); });
      soundSliders[0].onValueChanged.AddListener(delegate { MusicManager.ptr.SetVolume(musicVol, masterVol); });
      soundSliders[0].onValueChanged.AddListener(delegate { uiSFX.SetSFX(1, false, true); });

      soundSliders[1].onValueChanged.AddListener((float value) => musicVol = value);
      soundSliders[1].onValueChanged.AddListener(delegate { MusicManager.ptr.SetVolume(musicVol, masterVol); });
      soundSliders[1].onValueChanged.AddListener(delegate { SaveManager.ptr.settings.SetMusicVolume(musicVol); });
      soundSliders[1].onValueChanged.AddListener(delegate { uiSFX.SetSFX(1, false, true); });

      soundSliders[2].onValueChanged.AddListener((float value) => sfxVol = value);
      soundSliders[2].onValueChanged.AddListener(delegate { SaveManager.ptr.settings.SetSFXVolume(sfxVol); });
      soundSliders[2].onValueChanged.AddListener(delegate { uiSFX.SetSFX(1, false, true); });

      UpdateSFXSources(MusicManager.ptr.GetSFXObjs());

      buttons[0].onClick.AddListener(delegate { SwapMenus(0); });
      buttons[0].onClick.AddListener(delegate { uiSFX.SetSFX(4, false, true); });

      initialized = true;
    }
  }

  // Update is called once per frame
  void Update() {

  }

  public void NewGameRemap() {
    buttons[0].onClick.RemoveAllListeners();
    buttons[0].onClick.AddListener(delegate { SwapMenus(1); });
    buttons[0].onClick.AddListener(delegate { uiSFX.SetSFX(4, false, true); });
  }

  public void UpdateSFXSources(List<SFX> sfxObjs) {
    foreach (SFX sfx in sfxObjs) {
      soundSliders[0].onValueChanged.AddListener(delegate { sfx.SetVolume(sfxVol, masterVol); });
      soundSliders[2].onValueChanged.AddListener(delegate { sfx.SetVolume(sfxVol, masterVol); });
    }
  }

  public float[] GetVolumeSettings() {
    float[] f = new float[] { soundSliders[0].value, soundSliders[1].value, soundSliders[2].value };
    return f;
  }
}
