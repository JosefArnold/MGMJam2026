using UnityEngine;

public class SavePoint : Interactable {
  [SerializeField] private int index;

  private Animator anim;

  private void Start() {
    anim = GetComponent<Animator>();
  }

  public override void Interact(Player p) {
    SaveManager.ptr.Save(index, p);
    p.TakeDamage(-5);

    int i = Random.Range(1, 6);
    anim.SetTrigger("Save" + i);
  }
}
