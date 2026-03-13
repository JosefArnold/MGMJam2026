using UnityEngine;
using UnityEngine.UI;

public class LevelHUD : MonoBehaviour {

  public static LevelHUD ptr;

  [Header("UI Elements")]
  [SerializeField] private GameObject healthBar;
  [SerializeField] private Image[] health;
  [SerializeField] private Slider[] abilityCooldowns;

  [Header("Sprites")]
  [SerializeField] private Sprite[] healthSprites;

  private Camera cam;

  private void Awake() {
    ptr = this;
  }

  // Start is called once before the first execution of Update after the MonoBehaviour is created
  void Start() {
    cam = Camera.main;
  }

  // Update is called once per frame
  void FixedUpdate() {
    Player p = GameManager.ptr.p;
    Vector3 pos = new Vector3(p.transform.position.x - 1, p.transform.position.y, p.transform.position.z);
    abilityCooldowns[0].transform.position = cam.WorldToScreenPoint(pos);
  }

  public void UpdateHealth(int currentHealth) {
    for (int i = 0; i < health.Length; i++) {
      if (i < currentHealth)
        health[i].sprite = healthSprites[0];
      else
        health[i].sprite = healthSprites[1];
    }
  }

  public void ToggleHealth(bool b) {
    healthBar.SetActive(b);
  }

  public void ToggleUISlider(int index, bool b) {
    abilityCooldowns[index].gameObject.SetActive(b);
  }

  public void UpdateUIMeters(int index, float f) {
    abilityCooldowns[index].value = Mathf.Clamp(f, abilityCooldowns[index].minValue, abilityCooldowns[index].maxValue);
  }
}
