using UnityEngine;

public class SavePoint : Interactable {
  [SerializeField] private int index;

  private SFX sfx;
  private Animator anim;

  private void Start() {
    sfx = GetComponent<SFX>();
    anim = GetComponent<Animator>();
  }

  public override void Interact(Player p) {
    SaveManager.ptr.Save(index, p);
    p.TakeDamage(-5);

    sfx.SetCycleIndices(new int[] { 0, 1, 2, 3 }, true);

    int i = Random.Range(1, 6);
    anim.SetTrigger("Save" + i);
  }
}
