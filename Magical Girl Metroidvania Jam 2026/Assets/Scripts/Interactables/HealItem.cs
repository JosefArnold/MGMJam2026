using UnityEngine;

public class HealItem : Interactable {
  [SerializeField] private int healAmount;

  public override void Interact(Player p) {
    p.TakeDamage(-healAmount); // I realized that making the player just take negative damage is actually a heal lol
    Destroy(gameObject);
  }
}
