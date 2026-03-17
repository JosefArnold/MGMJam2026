using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

  public static UIManager ptr;

  //Object references
  [SerializeField] private EventSystem eventSystem;
  [SerializeField] private Player p;
  [SerializeField] private SFX uiSFX;

  //UI Elements
  [SerializeField] private GameObject[] UIGroups;
  [SerializeField] private Image sceneTransition; // The black image used to fade in and out of scenes
  [SerializeField] private GameObject pauseBtn;

  //Variables
  private bool paused = false;
  private bool newGameStarted;

  //Fade stuff
  private List<Image> fadeImage = new List<Image>(); // The image that's supposed to fade
  bool startFade = false;
  bool imageFaded = false; // Whether or not the image has finished fading
  private List<float> currentAlphas = new List<float>(); // Current alpha value of image
  private List<float> targetAlphas = new List<float>(); // Target alpha value of image

  // Event for when an image finishes fading
  public delegate void FinishedFade();
  public FinishedFade onFade;

  // Called when script is dragged onto object in the Editor
  private void Reset() {
    eventSystem = FindFirstObjectByType<EventSystem>();

    if (eventSystem == null)
      Debug.Log("There's no event system, the fuck did you do? Get one in here ya little shit");
  }

  private void Awake() {
    ptr = this;
  }

  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start() {
    fadeImage.Add(sceneTransition);
    currentAlphas.Add(1.0f);
    targetAlphas.Add(0.0f);
    Invoke(nameof(Fade), 0.5f);
  }

  // Update is called once per frame
  void FixedUpdate() {
    if (startFade)
      Fade();

    if (imageFaded) {
      if (onFade != null) {
        onFade();
        onFade = null;
      }

      if (fadeImage[0] == sceneTransition && targetAlphas[0] == 0.0f) {
        ToggleElement(0);

        if (!SceneManager.GetActiveScene().name.Equals("BeginningArea"))
          p.ToggleControls(true);
      }

      fadeImage.Clear();
      currentAlphas.Clear();
      targetAlphas.Clear();

      imageFaded = false;
    }
  }

  public void ToggleElement(int newGroupIndex) {
    UIGroups[newGroupIndex].SetActive(!UIGroups[newGroupIndex].activeSelf);
  }

  public void Pause() {
    paused = !paused;

    for (int i = 0; i < UIGroups.Length; i++) {
      UIGroups[i].SetActive(false);
    }

    if (paused) {
      ToggleElement(1);
      eventSystem.SetSelectedGameObject(pauseBtn);
    }

    if (paused) {
      Time.timeScale = 0.0f;
      Debug.Log("Paused");
      uiSFX.SetSFX(3, false, true);
    } else {
      Time.timeScale = 1.0f;
      Debug.Log("Unpaused");
      uiSFX.SetSFX(4, false, true);
    }
  }

  public void ResetTimescale() {
    Time.timeScale = 1.0f;
  }

  public void Fade() {
    if (!startFade)
      startFade = true;

    for (int i = 0; i < fadeImage.Count; i++) {
      currentAlphas[i] = Mathf.MoveTowards(currentAlphas[i], targetAlphas[i], 0.8f * Time.deltaTime);

      fadeImage[i].color = new Color(fadeImage[i].color.r, fadeImage[i].color.g, fadeImage[i].color.b, currentAlphas[i]);
    }

    if (targetAlphas[0] == 1.0f && currentAlphas[0] >= targetAlphas[0]) {
      imageFaded = true;
      startFade = false;
    } else if (targetAlphas[0] == 0.0f && currentAlphas[0] <= targetAlphas[0]) {
      imageFaded = true;
      startFade = false;
    }
  }

  // What image is being faded, whether it's fading in or out
  public void SetFadeImage(Image image, float alpha) {
    if (image != null) {
      fadeImage.Add(image);
      currentAlphas.Add(image.color.a);
    } else {
      fadeImage.Add(sceneTransition);
      currentAlphas.Add(sceneTransition.color.a);
    }

    targetAlphas.Add(alpha);
  }

  public void NewGame() {
    newGameStarted = true;
  }

  public bool CheckNewGame() {
    return newGameStarted;
  }
}
