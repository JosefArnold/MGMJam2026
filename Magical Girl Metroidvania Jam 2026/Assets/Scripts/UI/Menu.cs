using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour {

  [Header("Swapping Menus")]
  [SerializeField] private EventSystem eventSystem;
  [SerializeField] private Selectable[] elementsToSwapTo;
  [SerializeField] private bool disableOnSwitch;

  // Called when script is dragged onto object in the Editor
  private void Reset() {
    eventSystem = FindFirstObjectByType<EventSystem>();

    if (eventSystem == null)
      Debug.Log("There's no event system, the fuck did you do? Get one in here ya little shit");
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
