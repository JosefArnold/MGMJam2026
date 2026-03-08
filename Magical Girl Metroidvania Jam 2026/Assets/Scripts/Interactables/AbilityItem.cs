using UnityEngine;

public class AbilityItem : Interactable {

  [SerializeField] private int abilityIndex; // Which ability this should toggle for the player

  public override void Interact(Player p) {
    p.ToggleAbility(abilityIndex);
    Destroy(gameObject);
  }
}
