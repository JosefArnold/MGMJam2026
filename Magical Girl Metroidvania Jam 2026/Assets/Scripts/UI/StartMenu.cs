using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class StartMenu : Menu {

  [SerializeField] private Image logo;
  [SerializeField] private Button[] uiElements;
  [SerializeField] private PlayableDirector director;
  private bool confirm;

  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start() {

  }

  // Update is called once per frame
  void Update() {

  }

  public void LoadGame() {
    SaveManager.ptr.BeginningSceneSaveFile();
    GameManager.ptr.PrepSceneTransition(GameManager.ptr.nextScene);
  }

  public void NewGame() {
    if (confirm) {
      NewGameSFX();
      SaveManager.ptr.WipeSave();

      eventSystem.firstSelectedGameObject = elementsToSwapTo[2].gameObject;

      UIManager.ptr.NewGame();
      UIManager.ptr.onFade += IntroSequence;

      for (int i = 0; i < uiElements.Length; i++) {
        uiElements[i].interactable = false;
        UIManager.ptr.SetFadeImage(uiElements[i].image, 0.0f);
      }

      UIManager.ptr.SetFadeImage(logo, 0.0f);

      UIManager.ptr.Fade();
    } else
      SwapMenus(1);
  }

  public void ConfirmNewGame() {
    confirm = true;
    NewGame();
  }

  public void NewGameSFX() {
    uiSFX.SetSFX(0, false, true);
  }

  public void IntroSequence() {
    gameObject.SetActive(false);
    director.Play();
  }

  public void NoLoadedGame() {
    confirm = true;

    uiElements[0].interactable = false;

    Navigation nav1 = new Navigation();
    Navigation nav2 = new Navigation();

    nav1.mode = Navigation.Mode.Explicit;
    nav2.mode = Navigation.Mode.Explicit;

    nav1.selectOnUp = uiElements[3];
    nav1.selectOnDown = uiElements[2];
    nav2.selectOnUp = uiElements[2];
    nav2.selectOnDown = uiElements[1];

    uiElements[1].navigation = nav1;
    uiElements[3].navigation = nav2;

    eventSystem.SetSelectedGameObject(uiElements[1].gameObject);
  }
}
