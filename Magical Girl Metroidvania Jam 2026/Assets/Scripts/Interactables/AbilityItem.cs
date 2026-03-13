using UnityEngine;

public class AbilityItem : Interactable {

  [SerializeField] private int abilityIndex; // Which ability this should toggle for the player
  [SerializeField] private GameObject tutorialText;
  private Player player;
  private bool buffer = false;

  public override void Interact(Player p) {
    if (!buffer) {
      player = p;
      player.ToggleAbility(abilityIndex, true);
      p.ToggleControls(false);
      buffer = true;
      //TEMP
      Invoke("EndInteraction", 1.0f);
    }
  }

  public void EndInteraction() {
    player.ToggleControls(true);

    if (abilityIndex == 0)
      LevelHUD.ptr.ToggleHealth(true);

    tutorialText.SetActive(true);
    Destroy(gameObject);
  }
}
