using UnityEngine;

public class AbilityItem : Interactable {

  [SerializeField] private int abilityIndex; // Which ability this should toggle for the player
  [SerializeField] private GameObject tutorialText;

  public override void Interact(Player p) {
    p.ToggleAbility(abilityIndex);

    //TEMP
    Invoke("EndInteraction", 1.0f);
  }

  public void EndInteraction() {
    if (abilityIndex == 0)
      LevelHUD.ptr.ToggleHealth(true);

    tutorialText.SetActive(true);
    Destroy(gameObject);
  }
}
