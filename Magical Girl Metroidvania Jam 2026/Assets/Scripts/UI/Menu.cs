using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour {

  [Header("Swapping Menus")]
  [SerializeField] protected EventSystem eventSystem;
  [SerializeField] protected Selectable[] elementsToSwapTo;
  [SerializeField] private bool disableOnSwitch;

  [Header("SFX Reference")]
  [SerializeField] protected SFX uiSFX;
  //[SerializeField] protected Selectable[] allSelectables;

  // Called when script is dragged onto object in the Editor
  private void Reset() {
    eventSystem = FindFirstObjectByType<EventSystem>();

    if (eventSystem == null)
      Debug.Log("There's no event system, the fuck did you do? Get one in here ya little shit");
  }

  private void Start() {
    /*
    foreach (Selectable s in allSelectables) {
      s.OnMove()
    }
    */
  }

  public void PlaySelectSFX() {
    uiSFX.SetSFX(2, false, true);
  }

  public void PlayReturnSFX() {
    uiSFX.SetSFX(4, false, true);
  }

  public void LoadScene(string sceneName) {
    SceneManager.LoadSceneAsync(sceneName);
  }

  public void SwapMenus(int m) {
    Selectable nextElement = elementsToSwapTo[m];

    nextElement.transform.parent.gameObject.SetActive(true);
    eventSystem.SetSelectedGameObject(nextElement.gameObject);

    if (disableOnSwitch)
      ToggleMenuOff();
  }

  public void ToggleMenuOff() {
    gameObject.SetActive(false);
  }
}
