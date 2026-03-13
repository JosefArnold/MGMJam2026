using UnityEngine;

public class AbilityItem : Interactable {

  [SerializeField] private int abilityIndex; // Which ability this should toggle for the player
  [SerializeField] private GameObject tutorialText;
  private Player player;

  public override void Interact(Player p) {
    player = p;
    //TEMP
    Invoke("EndInteraction", 1.0f);
  }

  public void EndInteraction() {
    player.ToggleAbility(abilityIndex);

    if (abilityIndex == 0)
      LevelHUD.ptr.ToggleHealth(true);

    tutorialText.SetActive(true);
    Destroy(gameObject);
  }
}
