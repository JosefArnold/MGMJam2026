using UnityEngine;

public class SavePoint : Interactable {
  [SerializeField] private int index;

  public override void Interact(Player p) {
    SaveManager.ptr.Save(index, p);
  }
}
